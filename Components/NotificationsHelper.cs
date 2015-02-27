//Copyright (c) 2007-2014, William Severance, Jr., WESNet Designs
//All rights reserved.

//Redistribution and use in source and binary forms, with or without modification, are permitted
//provided that the following conditions are met:

//Redistributions of source code must retain the above copyright notice, this list of conditions
//and the following disclaimer.

//Redistributions in binary form must reproduce the above copyright notice, this list of conditions
//and the following disclaimer in the documentation and/or other materials provided with the distribution.

//Neither the name of William Severance, Jr. or WESNet Designs may be used to endorse or promote
//products derived from this software without specific prior written permission.

//Disclaimer: THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS
//            OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY
//            AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER BE LIABLE
//            FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT
//            LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS
//            INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
//            OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN
//            IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

//Although I will try to answer questions regarding the installation and use of this software when
//such questions are submitted via e-mail to the below address, no promise of further
//support or enhancement is made nor should be assumed.

//Developer Contact Information:
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
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Users;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Social.Notifications;
using DotNetNuke.Services.Social.Messaging.Internal;
using DotNetNuke.Services.Localization;

namespace WESNet.DNN.Modules.ByInvitation
{
    public class NotificationsHelper
    {
        // This method should be run only once (via IUpgradable) during installation of ByInvitation version 1.00.00

        public static void AddNotificationTypes()
        {
            var desktopModuleID = DesktopModuleController.GetDesktopModuleByFriendlyName(Configuration.FriendlyName).DesktopModuleID;

            var actions = new List<NotificationTypeAction>();
            AddAction(actions, "Approve", true, 1);
            AddAction(actions, "Disapprove", true, 3);
            AddAction(actions, "EditApprove", false, 5);

            AddNotificationType(Consts.ModerationRequestedNotification, actions, desktopModuleID);

            //AddNotificationType(Consts.SentNotification, null, desktopModuleID);

            AddNotificationType(Consts.InvitationStatusChangeNotification, null, desktopModuleID);

            AddNotificationType(Consts.BulkImportStatusChangeNotification, null, desktopModuleID);
        }

        private static void AddNotificationType(NotificationDescriptor notificationDescriptor, List<NotificationTypeAction> actions, int desktopModuleId)
        {
            var objNotificationType = new NotificationType();

            objNotificationType.Name = Configuration.ModuleName + "_" + notificationDescriptor.Name;
            objNotificationType.Description = notificationDescriptor.Description;
            objNotificationType.DesktopModuleId = desktopModuleId;

            if (DotNetNuke.Services.Social.Notifications.NotificationsController.Instance.GetNotificationType(objNotificationType.Name) == null)
            {
                NotificationsController.Instance.CreateNotificationType(objNotificationType);
                if (actions != null && actions.Count > 0)
                {
                    NotificationsController.Instance.SetNotificationTypeActions(actions, objNotificationType.NotificationTypeId);
                }
            }
        }

        private static void AddAction(List<NotificationTypeAction> actions, string actionName, bool confirmAction, int order)
        {
            var objAction = new NotificationTypeAction();
            objAction.NameResourceKey = actionName + ".Name";
            objAction.DescriptionResourceKey = actionName + ".Desc";
            if (confirmAction) objAction.ConfirmResourceKey = actionName + ".Confirm";
            objAction.APICall = Configuration.ModulePath + "API/ByInvitationService/" + actionName;
            objAction.Order = order;
            actions.Add(objAction);
        }

        public static int GetRecipientsLimit(int portalId)
        {
            return InternalMessagingController.Instance.RecipientLimit(portalId);
        }

        public static Notification SendNotification(string notificationTypeName, string subject, string body, bool includeDismissAction,
                                                     List<DotNetNuke.Entities.Users.UserInfo> recipientUsers, int sendingUserID, object obj)
        {
            if (recipientUsers == null || recipientUsers.Count == 0) return null;

            var notificationType = NotificationsController.Instance.GetNotificationType(Configuration.ModuleName + "_" + notificationTypeName);

            if (notificationType == null) throw new ArgumentException("Invalid notification type name " + Configuration.ModuleName + "_" + notificationTypeName);

            var context = new NotificationContext(obj);

            var notification = new Notification
            {
                NotificationTypeID = notificationType.NotificationTypeId,
                Subject = subject,
                Body = body,
                IncludeDismissAction = includeDismissAction,
                SenderUserID = sendingUserID,
                Context = context.ToString()
            };

            var portalID = context.PortalID;

            var recipientsLimit = GetRecipientsLimit(portalID);
            while (recipientUsers.Count > recipientsLimit)
            {
                var tmpRecipientUsers = new List<DotNetNuke.Entities.Users.UserInfo>(recipientUsers.Take(recipientsLimit));
                recipientUsers = new List<DotNetNuke.Entities.Users.UserInfo>(recipientUsers.Skip(recipientsLimit));
                NotificationsController.Instance.SendNotification(notification, portalID, null, tmpRecipientUsers);
            }
            if (recipientUsers.Count > 0)
            {
                NotificationsController.Instance.SendNotification(notification, portalID, null, recipientUsers);
            }

            return notification;
        }

        public static void DeleteAllNotificationRecipients(string notificationTypeName, string notificationContext)
        {
            var notificationType = NotificationsController.Instance.GetNotificationType(Configuration.ModuleName + "_" + notificationTypeName);
            if (notificationType != null)
            {
                var notifications = NotificationsController.Instance.GetNotificationByContext(notificationType.NotificationTypeId, notificationContext);
                foreach (Notification notification in notifications)
                {
                    NotificationsController.Instance.DeleteAllNotificationRecipients(notification.NotificationID);
                }
            }
        }

        public static void DeleteAllNotificationRecipients(string notificationTypeName, int invitationID)
        {
            var invitation = InvitationController.GetInvitation(invitationID);
            if (invitation != null)
            {
                var accessingUser = DotNetNuke.Entities.Users.UserController.Instance.GetCurrentUserInfo();
                var notificationContext = new NotificationContext(invitation).ToString();
                DeleteAllNotificationRecipients(notificationTypeName, notificationContext);
            }
        }

        public static int SendNotifications(InvitationInfo invitation, Notifications reason)
        {
            return SendNotifications(invitation, reason, null);
        }

        public static int SendNotifications(InvitationInfo invitation, Notifications reasons, string message)
        {
            var portalId = invitation.PortalId;
            var configuration = invitation.MyConfiguration;
            var ps = new PortalSettings(portalId);
            var portalAdministratorId = ps.AdministratorId;
            var portalAdministratorUser = new List<UserInfo>() { UserController.GetUserById(portalId, portalAdministratorId) };
            
            var notificationsSent = 0;
            
            var uniqueReasons = reasons.GetUniqueFlags();


            foreach (Notifications reason in uniqueReasons)
            {
                var resourceKey = reason.ToString();
                var subject = Localization.GetString(resourceKey + ".Subject", Configuration.LocalSharedResourceFile);
                var body = Localization.GetString(resourceKey + ".Body", Configuration.LocalSharedResourceFile);

                if (!string.IsNullOrEmpty(subject) && !string.IsNullOrEmpty(body))
                {
                    if (!string.IsNullOrEmpty(message))
                    {
                        body += "<hr />" + message;
                    }

                    var tokenizer = new Tokenizer(invitation);
                    subject = tokenizer.ReplaceInvitationTokens(subject);
                    body = tokenizer.ReplaceInvitationTokens(body);

                    if (reason == Notifications.PendingModeration && configuration.RequireModeration && !invitation.IsApproved)
                    {
                        var moderators = new Security(invitation.TabId, invitation.ModuleId).Moderators;
                        if (moderators.Count == 0)
                        {
                            moderators = portalAdministratorUser;
                        }
                        SendNotification(Consts.ModerationRequestedNotification.Name, subject, body, true, moderators, portalAdministratorId, invitation);
                        notificationsSent += moderators.Count;
                    }

                    var sentToInvitingUser = false;

                    if (configuration.EnabledInvitingUserNotifications.HasFlag(reason))
                    {
                        if (invitation.InvitedByUserID == -1)
                        {
                            if (reason == Notifications.Approved || reason == Notifications.Disapproved)
                            {
                                var mailManager = new MailManager(invitation);
                                var errMsg = mailManager.SendBulkMail(reason.ToString().ToLowerInvariant());
                                if (errMsg != "")
                                {
                                    notificationsSent += NotificationsHelper.SendNotifications(invitation, Notifications.Errored, errMsg);
                                }
                            }
                        }
                        else
                        {
                            var invitingUser = new List<UserInfo>() { UserController.GetUserById(portalId, invitation.InvitedByUserID) };
                            SendNotification(Consts.InvitationStatusChangeNotification.Name, subject, body, true, invitingUser, portalAdministratorId, invitation);
                            sentToInvitingUser = true;
                            notificationsSent++;
                        }
                    }

                    if (configuration.EnabledAdminUserNotifications.HasFlag(reason) && (invitation.InvitedByUserID != portalAdministratorId || !sentToInvitingUser))
                    {
                        SendNotification(Consts.InvitationStatusChangeNotification.Name, subject, body, true, portalAdministratorUser, portalAdministratorId, invitation);
                        notificationsSent++;
                    }
                }
            }
            return notificationsSent;
        }

        public static int SendNotifications(BulkImportInfo bulkImport, Notifications reasons)
        {
            var portalId = bulkImport.PortalID;
            var configuration = bulkImport.MyConfiguration;
            var ps = new PortalSettings(portalId);
            var portalAdministratorId = ps.AdministratorId;
            var portalAdministratorUser = new List<UserInfo>() { UserController.GetUserById(portalId, portalAdministratorId) };
            
            var notificationsSent = 0;
            var uniqueReasons = reasons.GetUniqueFlags();

            foreach (Notifications reason in uniqueReasons)
            {
                var resourceKey = reason.ToString();
                var subject = Localization.GetString(resourceKey + ".Subject", Configuration.LocalSharedResourceFile);
                var body = Localization.GetString(resourceKey + ".Body", Configuration.LocalSharedResourceFile);

                if (!string.IsNullOrEmpty(subject) && !string.IsNullOrEmpty(body))
                {
                    var tokenizer = new Tokenizer(bulkImport);
                    subject = tokenizer.ReplaceInvitationTokens(subject);
                    body = tokenizer.ReplaceInvitationTokens(body);

                    var sentToImportingUser = false;

                    if (configuration.EnabledInvitingUserNotifications.HasFlag(reason))
                    {
                        var importingUser = new List<UserInfo>() { UserController.GetUserById(portalId, bulkImport.ImportedByUserID) };
                        SendNotification(Consts.BulkImportStatusChangeNotification.Name, subject, body, true, importingUser, portalAdministratorId, bulkImport);
                        sentToImportingUser = true;
                        notificationsSent++;
                    }

                    if (configuration.EnabledAdminUserNotifications.HasFlag(reason) && (bulkImport.ImportedByUserID != portalAdministratorId || !sentToImportingUser))
                    {
                        SendNotification(Consts.InvitationStatusChangeNotification.Name, subject, body, true, portalAdministratorUser, portalAdministratorId, bulkImport);
                        notificationsSent++;
                    }
                }
            }
            return notificationsSent;
        }

    }
}