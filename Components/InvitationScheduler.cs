// Copyright (c) 2015, William Severance, Jr., WESNet Designs
// All rights reserved.

// Redistribution and use in source and binary forms, with or without modification, are permitted
// provided that the following conditions are met:

// Redistributions of source code must retain the above copyright notice, this list of conditions
// and the following disclaimer.

// Redistributions in binary form must reproduce the above copyright notice, this list of conditions
// and the following disclaimer in the documentation and/or other materials provided with the distribution.

// Neither the name of William Severance, Jr. or WESNet Designs may be used to endorse or promote
// products derived from this software without specific prior written permission.

// Disclaimer: THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS
//             OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY
//             AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER BE LIABLE
//             FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT
//             LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS
//             INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
//             OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN
//             IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

// Although I will try to answer questions regarding the installation and use of this software when
// such questions are submitted via e-mail to the below address or through the Issue Tracker on this project's
// CodePlex project site (http://dnnbyinvitation.codeplex.com  no promise of further support or enhancement
// is made nor should be assumed.

// Developer Contact Information:
//     William Severance, Jr.
//     WESNet Designs
//     679 Upper Ridge Road
//     Bridgton, ME 04009
//     Phone: 207-317-1365
//     E-Mail: bill@wesnetdesigns.com
//     Website: www.wesnetdesigns.com

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using DotNetNuke;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Users;
using DotNetNuke.Entities.Host;
using DotNetNuke.Services.Messaging;
using DotNetNuke.Services.Log.EventLog;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Scheduling;

namespace WESNet.DNN.Modules.ByInvitation
{
    public class InvitationScheduler : SchedulerClient
    {
        private readonly PortalController _pController = new PortalController();
        private readonly UserController _uController = new UserController();

        public InvitationScheduler(ScheduleHistoryItem objScheduleHistoryItem)
        {
            ScheduleHistoryItem = objScheduleHistoryItem;
        }

        public override void DoWork()
        {
            try
            {
                var schedulerInstance = Guid.NewGuid();
                var recordsRead = 0;
                var recordsSkipped = 0;
                var invitationsCreated = 0;
                var invitationsSent = 0;
                var notificationsSent = 0;
                var aborted = false;
                var errorCount = 0;

                var totalRecordsRead = 0;
                var totalRecordsSkipped = 0;
                var totalInvitationsCreated = 0;
                var totalInvitationsFailedToCreate = 0;
                var totalInvitationsSent = 0;
                var totalNotificationsSent = 0;
                var totalErrorCount = 0;

                var reportSB = new StringBuilder();

                ScheduleHistoryItem.AddLogNote("WESNet_ByInvitation Invitation Scheduler DoWork Starting " + schedulerInstance);

                if (string.IsNullOrEmpty(Host.SMTPServer)) // In DNN > 7.3.0, returns portal SMTPServer if portal SMTP enabled otherwise host SMTPServer
                {
                    ScheduleHistoryItem.AddLogNote("\r\nNo SMTP Servers have been configured for this host. Terminating task.");
                    ScheduleHistoryItem.Succeeded = true; //not false so that task is not repeatedly retried as it will fail again
                    var schedulerException = new SchedulerException("WESNet_ByInvitation module is unable to send invitations as no SMTP servers have been configured for this host.");
                    var elc = new EventLogController();
                    Exceptions.LogException(schedulerException);
                    return;
                }

                Progressing();

                // Process any unfinished batch imports - batch read, create,  and/or send invitations
                var databaseTime = DateUtils.GetDatabaseTime();

                var bulkImports = InvitationController.GetBulkImports(-1, -1, true).Where (bi => bi.SendMethod == SendMethods.Scheduled && bi.IsWizardFinished && bi.ScheduledJobStartsAt <= databaseTime);
                foreach (var bulkImport in bulkImports)
                {
                    var portalID = bulkImport.PortalID;
                    var portalName = new PortalSettings(bulkImport.PortalID).PortalName;
                    var configuration = Configuration.GetConfiguration(portalID);
                    reportSB.AppendFormat("<br />Processing Bulk Import created on {0} by user '{1}' in website '{2}'.",
                                                                    bulkImport.ImportedOnDate, bulkImport.ImportedByUser, portalName);
                    
                    // Update BatchNumber and next batch start time
                    bulkImport.BatchNumber++;
                    bulkImport.ScheduledJobStartsAt = databaseTime.AddMinutes(bulkImport.SendBatchInteval);
                    InvitationController.SaveBulkImport(bulkImport);

                    if (!bulkImport.IsDoneReading)
                    {
                        reportSB.AppendLine("<br />" + bulkImport.CreateInvitations(out recordsRead, out recordsSkipped, out invitationsCreated, out notificationsSent, out errorCount, out aborted));
                        totalRecordsRead += recordsRead;
                        totalRecordsSkipped += recordsSkipped;
                        totalInvitationsCreated += invitationsCreated;
                        totalInvitationsFailedToCreate += Math.Max(0, recordsRead - recordsSkipped - invitationsCreated);
                        totalErrorCount += errorCount;
                    }

                    if (!bulkImport.IsDoneSending)
                    {
                        reportSB.AppendLine("<br />" + bulkImport.SendInvitations(out invitationsSent, out notificationsSent, out errorCount));
                        totalInvitationsSent += invitationsSent;
                        totalNotificationsSent += notificationsSent;
                        totalErrorCount += errorCount;
                    }
                }

                if (totalRecordsRead > 0 || totalInvitationsCreated > 0 || totalInvitationsSent > 0 || totalNotificationsSent > 0 || totalErrorCount > 0)
                {
                    ScheduleHistoryItem.AddLogNote(string.Format("<br />WESNet_ByInvitation Invitation Scheduler '{0}' read {1} imported invitation records, created {2} invitations, sent {3} invitations and/or {4} notifications with {5} errors.",
                        schedulerInstance, totalRecordsRead, totalInvitationsCreated, totalInvitationsSent, totalNotificationsSent, totalErrorCount));
                }

                // Process individual invitations created but not yet sent
                invitationsSent = 0;
                notificationsSent = 0;
                errorCount = 0;

                var invitationsToSend = InvitationController.GetInvitations(-1).Where(i => i.IsApproved && !i.HasBeenSent);
                foreach (var invitation in invitationsToSend)
                {
                    var mailer = new MailManager(invitation);
                    var errMsg = mailer.SendBulkMail("send");
                    if (string.IsNullOrEmpty(errMsg))
                    {
                        InvitationController.UpdateInvitationStatus(invitation.InvitationID, "sent", invitation.InvitedByUserID); 
                        invitationsSent++;
                        var currentInvitation = InvitationController.GetInvitation(invitation.InvitationID);
                        notificationsSent += NotificationsHelper.SendNotifications(currentInvitation, Notifications.Sent);
                    }
                    else
                    {
                        errorCount++;
                        notificationsSent += NotificationsHelper.SendNotifications(invitation, Notifications.Errored, errMsg);
                    }
                }
                if (invitationsSent > 0 || notificationsSent > 0 || errorCount > 0)
                {
                    ScheduleHistoryItem.AddLogNote(string.Format("<br />WESNet_ByInvitation Invitation Scheduler '{0}' sent {1} invitations and/or {2} notifications with {3} errors.", schedulerInstance, invitationsSent, notificationsSent, errorCount));
                }

                // Resend any invitations still pending acceptance that past the resend interval and have not already been
                // resent MaxResends times.

                var invitationsToReSend = InvitationController.GetInvitations(-1, true, InvitationStatus.Pending);
                var invitationsResent = 0;
                errorCount = 0;

                foreach (var invitation in invitationsToReSend)
                {
                    var configuration = Configuration.GetConfiguration(invitation.PortalId);
                    var lastSentDate = invitation.LastResentOnDate != DateTime.MinValue ? invitation.LastResentOnDate : invitation.SentOnDate;
                    if (configuration.EnableAutoResend && (invitation.ResentCount < configuration.MaxResends) && (lastSentDate.Add(configuration.ResendInterval) < databaseTime))
                    {
                        var mailer = new MailManager(invitation);
                        var errMsg = mailer.SendBulkMail("resend");
                        if (string.IsNullOrEmpty(errMsg))
                        {
                            invitationsResent++;
                            var currentInvitation = InvitationController.UpdateInvitationStatus(invitation.InvitationID, "resent", -1);
                            notificationsSent += NotificationsHelper.SendNotifications(currentInvitation, Notifications.Resent);
                        }
                        else
                        {
                            errorCount++;
                            notificationsSent += NotificationsHelper.SendNotifications(invitation, Notifications.Errored, errMsg);
                        }
                    }
                }
                if (invitationsResent > 0 || notificationsSent > 0 || errorCount > 0)
                {
                    ScheduleHistoryItem.AddLogNote(string.Format("<br />Resent {0} invitations successfully and or {1} notifications with {2} errors", invitationsResent, notificationsSent, errorCount));
                }

                // Automatically delete or archive any pending invitations that have expired and have not been previously archived

                var invitationsToProcess = InvitationController.GetInvitations(-1, true, InvitationStatus.Expired).Where(i => !i.IsArchived);
                var invitationsDeleted = 0;
                var invitationsArchived = 0;
                notificationsSent = 0;

                foreach (var invitation in invitationsToProcess)
                {
                    var configuration = InvitationController.GetConfiguration(invitation.PortalId);
                    if (configuration.AutoDeleteArchiveExpiredInvitations != ExpiredInvitationActions.None && invitation.ExpiresOnDate.AddDays(configuration.DaysPastExpiration) > databaseTime)
                    {
                        InvitationInfo currentInvitation = null;
                        if (configuration.AutoDeleteArchiveExpiredInvitations == ExpiredInvitationActions.Archive)
                        {
                            currentInvitation = InvitationController.UpdateInvitationStatus(invitation.InvitationID, "archive", -1);
                            invitationsArchived++;
                        }
                        else
                        {
                            InvitationController.DeleteInvitation(invitation.InvitationID);
                            invitationsDeleted++;
                            currentInvitation = invitation;
                        }
                        
                        notificationsSent += NotificationsHelper.SendNotifications(currentInvitation, Notifications.Expired);
                    }
                }
                if (invitationsDeleted > 0 || invitationsArchived > 0 || notificationsSent > 0)
                {
                    ScheduleHistoryItem.AddLogNote(string.Format("<br />{0} expired invitations were deleted, {1} expired invitations were archived and/or {2} notifications were sent.", invitationsDeleted, invitationsArchived, notificationsSent));
                }

                ScheduleHistoryItem.Succeeded = true;
            } //Try
            catch (Exception ex)
            {
                ScheduleHistoryItem.Succeeded = false;
                ScheduleHistoryItem.AddLogNote("<br />WESNet_ByInvitation Notification Scheduler Failed:<br />" + ex.ToString());
                Errored(ref ex);
            }
        }

    }
}