//Copyright (c) 2014, William Severance, Jr., WESNet Designs
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
using System.Text;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Collections.Generic;
using System.Linq;
using DotNetNuke.Entities.Users;
using DotNetNuke.Instrumentation;
using DotNetNuke.Security;
using DotNetNuke.Web.Api;
using DotNetNuke.Web.Api.Internal;
using DotNetNuke.Services.Social.Messaging.Internal;
using DotNetNuke.Services.Social.Notifications;
using DotNetNuke.Security.Permissions;
using DotNetNuke.Common.Utilities;


namespace WESNet.DNN.Modules.ByInvitation.Services
{
    public class ByInvitationServiceController : DnnApiController
    {
        #region "Private Fields"

        private static readonly ILog _logger = LoggerSource.Instance.GetLogger(typeof(ByInvitationServiceController));

        #endregion

        public class NotificationDTO
        {
            public int NotificationId { get; set; }
        }

        private Security GetSecurity()
        {
            var moduleID = ActiveModule.ModuleID;
            var tabID = ActiveModule.TabID;
            var userID = UserInfo.UserID;
            return new Security(tabID, moduleID, userID);
        }

        #region Notification Action Handling

        private HttpResponseMessage ModerateInvitation(NotificationDTO postData, bool approve)
        {
            try
            {
                var notification = NotificationsController.Instance.GetNotification(postData.NotificationId);
                var notificationContext = new NotificationContext(notification.Context);
                var recipient = InternalMessagingController.Instance.GetMessageRecipient(postData.NotificationId, UserInfo.UserID);
                if (recipient == null) return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Unable to locate recipient");
                var invitationID = notificationContext.ItemID;
                var invitation = InvitationController.GetInvitation(invitationID);      
                if (invitation != null)
                { 
                    var security = new Security(invitation.TabId, invitation.ModuleId, UserInfo.UserID);
                    if (!security.CanModerate)
                    {
                        return Request.CreateErrorResponse(HttpStatusCode.Unauthorized, "Not Authorized!");
                    }

                    //var myConfiguration = WESNet.DNN.Modules.ByInvitation.Configuration.GetConfiguration(PortalSettings.PortalId);
                    //var newExpiresOnDate = DateUtils.GetDatabaseTime().Add(myConfiguration.ValidityPeriod);

                    //var moderationAction = approve ? "approved" : "disapproved";
                    
                    //InvitationController.UpdateInvitationStatus(invitationID, moderationAction, UserInfo.UserID, newExpiresOnDate);
                    //var currentInvitation = InvitationController.GetInvitation(invitation.PortalId, invitation.InvitationID);

                    //// send approved/rejected post notification to inviting user and if approved email the invition to recipient and any copy recipients.
                        
                    //var mailManager = new MailManager(currentInvitation);
                    //if (approve) mailManager.SendBulkMail("sent");

                    //if (currentInvitation.InvitedByUserID == -1) //anonymous inviting user?
                    //{
                    //    mailManager.SendBulkMail(moderationAction);
                    //}
                    //else
                    //{
                    //    NotificationsHelper.SendNotifications(currentInvitation, approve ? Notifications.Approved : Notifications.Disapproved);
                    //}
   
                    //NotificationsController.Instance.DeleteAllNotificationRecipients(postData.NotificationId);

                    Utilities.ModerateInvitation(invitation, approve, UserInfo.UserID);

                    return Request.CreateResponse(HttpStatusCode.OK, new { Result = "success" });
                }
                else
                {
                    _logger.Error(string.Format("Unable to load WESNet_By Invitation invitation object having InvitationID = {0}.", invitationID));
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Invitation was deleted or otherwise is not available");
                }
            }
            catch (Exception exc)
            {
                _logger.Error(exc);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, exc);
            }
        }

        [HttpPost]
        [DnnAuthorize(StaticRoles = "Registered Users")]
        [ValidateAntiForgeryToken]
        public HttpResponseMessage Approve(NotificationDTO postData)
        {
            return ModerateInvitation(postData, true);
        }

        [HttpPost]
        [DnnAuthorize(StaticRoles = "Registered Users")]
        [ValidateAntiForgeryToken]
        public HttpResponseMessage Disapprove(NotificationDTO postData)
        {
            return ModerateInvitation(postData, false);
        }

        [HttpPost]
        [DnnAuthorize(StaticRoles = "Registered Users")]
        [ValidateAntiForgeryToken]
        public HttpResponseMessage EditApprove(NotificationDTO postData)
        {
            try
            {
                var notification = NotificationsController.Instance.GetNotification(postData.NotificationId);
                var notificationContext = new NotificationContext(notification.Context);
                var recipient = InternalMessagingController.Instance.GetMessageRecipient(postData.NotificationId, UserInfo.UserID);
                if (recipient == null)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Unable to locate recipient");
                }
                var invitationID = notificationContext.ItemID;
                var invitation = InvitationController.GetInvitation(invitationID);
                if (invitation != null)
                {
                    var security = new Security(invitation.TabId, invitation.ModuleId, UserInfo.UserID);
                    if (!security.CanModerate)
                    {
                        return Request.CreateErrorResponse(HttpStatusCode.Unauthorized, "Not Authorized!");
                    }

                    NotificationsController.Instance.DeleteAllNotificationRecipients(postData.NotificationId);

                    var args = new List<string>();
                    args.Add("mid=" + invitation.ModuleId.ToString());
                    args.Add("id=" + invitation.InvitationID.ToString());
                    args.Add("mode=EditApprove");
                    var url = DotNetNuke.Common.Globals.NavigateURL(invitation.TabId, "", args.ToArray());

                    return Request.CreateResponse(HttpStatusCode.OK, new { Result = "success", Link = url });
                }
                else
                {
                    _logger.Error(string.Format("Unable to load invitation object having InvitationID = {0}.", invitationID));
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Invitation was deleted or otherwise is not available");
                }
            }
            catch (Exception exc)
            {
                _logger.Error(exc);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, exc);
            }
        }

        #endregion
    }
}