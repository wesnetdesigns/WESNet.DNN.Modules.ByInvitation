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
using System.Text.RegularExpressions;
using System.Threading;
using System.Web.UI;
using System.Web.UI.WebControls;
using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Framework.JavaScriptLibraries;
using DotNetNuke.Services.Localization;
using DotNetNuke.Services.SystemDateTime;
using DotNetNuke.Web.UI.WebControls;
using Telerik.Web.UI;

namespace WESNet.DNN.Modules.ByInvitation
{
    public partial class ManageInvitations : PortalModuleBase
    {

        private Configuration _MyConfiguration = null;
        private Security _ModuleSecurity = null;
        private InvitationStatus _unmoderated = InvitationStatus.Approved | InvitationStatus.Disapproved;
        
        public Configuration MyConfiguration
        {
            get
            {
                if (_MyConfiguration == null)
                {
                    _MyConfiguration = InvitationController.GetConfiguration(PortalId);
                }
                return _MyConfiguration;
            }
        }

        public Security ModuleSecurity
        {
            get
            {
                if (_ModuleSecurity == null)
                {
                    _ModuleSecurity = new Security(ModuleConfiguration);
                }
                return _ModuleSecurity;
            }
        }

        public string CultureCode
        {
            get
            {
                return Thread.CurrentThread.CurrentUICulture.Name;
            }
        }

        public string InvitingUserFilter
        {
            get
            {
                if (ddlInvitingUser.Visible)
                {
                    if (ddlInvitingUser.SelectedIndex != -1)
                    {
                        return ddlInvitingUser.SelectedValue;
                    }
                    else
                    {
                        return "0";
                    }
                }
                else
                {
                    return UserId.ToString();
                }
            }
            set
            {
                if (ddlInvitingUser.Visible)
                {
                    ListItem li = ddlInvitingUser.Items.FindByValue(value);
                    if (li != null)
                    {
                        ddlInvitingUser.SelectedIndex = ddlInvitingUser.Items.IndexOf(li);
                    }
                    else ddlInvitingUser.SelectedIndex = -1;
                }
            }
        }

        public string InvitingUserDisplayName
        {
            get
            {
                return ViewState["InvitingUserDisplayName"] == null ? LocalizeString("NoneSelected") : (string)ViewState["InvitingUserDisplayName"];
            }
            set
            {
                ViewState["InvitingUserDisplayName"] = value;
            }
        }

        public DateTime StartDate
        {
            get
            {
                return(tbFromDate.SelectedDate == null ? ModuleConfiguration.DesktopModule.CreatedOnDate : (DateTime)tbFromDate.SelectedDate);
            }
            set
            {
                if (value < ModuleConfiguration.DesktopModule.CreatedOnDate)
                {
                    value = ModuleConfiguration.DesktopModule.CreatedOnDate;
                }
                tbFromDate.SelectedDate = value;
            }
        }

        public DateTime EndDate
        {
            get
            {
                return (tbThroughDate.SelectedDate == null ? SystemDateTime.GetCurrentTime() : ((DateTime)tbThroughDate.SelectedDate) + new TimeSpan(23,59,59));
            }
            set
            {
                if (value > SystemDateTime.GetCurrentTime())
                {
                    value = SystemDateTime.GetCurrentTime();
                }
                tbThroughDate.SelectedDate = value;
            }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            JavaScript.RequestRegistration(CommonJs.DnnPlugins);
            lnkSearchAgain.Click += lnkSearchAgain_Click;
        }
        
        protected void Page_Load(object sender, EventArgs e)
        {
            if (ModuleSecurity.CanManageOwnInvitations)
            {
                dgUnmoderatedInvitations.ClientSettings.Selecting.AllowRowSelect = true;
                dgPendingInvitations.ClientSettings.Selecting.AllowRowSelect = true;
                dgRetractedInvitations.ClientSettings.Selecting.AllowRowSelect = true;
                dgDeclinedInvitations.ClientSettings.Selecting.AllowRowSelect = true;
                dgExpiredInvitations.ClientSettings.Selecting.AllowRowSelect = true;
                dgAcceptedInvitations.ClientSettings.Selecting.AllowRowSelect = true;

                dgUnmoderatedInvitations.ItemDataBound += new GridItemEventHandler(dgInvitations_ItemDataBound);
                dgUnmoderatedInvitations.ItemCommand += new GridCommandEventHandler(dgInvitations_ItemCommand);

                dgPendingInvitations.ItemDataBound +=new GridItemEventHandler(dgInvitations_ItemDataBound);
                dgPendingInvitations.ItemCommand += new GridCommandEventHandler(dgInvitations_ItemCommand);

                dgRetractedInvitations.ItemDataBound += new GridItemEventHandler(dgInvitations_ItemDataBound);
                dgRetractedInvitations.ItemCommand += new GridCommandEventHandler(dgInvitations_ItemCommand);

                dgDeclinedInvitations.ItemDataBound += new GridItemEventHandler(dgInvitations_ItemDataBound);
                dgDeclinedInvitations.ItemCommand += new GridCommandEventHandler(dgInvitations_ItemCommand);

                dgExpiredInvitations.ItemDataBound += new GridItemEventHandler(dgInvitations_ItemDataBound);
                dgExpiredInvitations.ItemCommand += new GridCommandEventHandler(dgInvitations_ItemCommand);

                dgAcceptedInvitations.ItemDataBound += new GridItemEventHandler(dgInvitations_ItemDataBound);
                dgAcceptedInvitations.ItemCommand += new GridCommandEventHandler(dgInvitations_ItemCommand);

                cmdReturn.Click += cmdReturn_Click;

                if (!Page.IsPostBack)
                {
                    BindInvitingUsers();
                    StartDate = ModuleConfiguration.DesktopModule.CreatedOnDate;
                    EndDate = SystemDateTime.GetCurrentTime();
                    LoadInvitingUserStats();
                    BindAllGrids();
                }
            }
            else
            {
                Response.Redirect(DotNetNuke.Common.Globals.AccessDeniedURL(), true);
            }
        }

        private void BindInvitingUsers()
        {
            if (ModuleSecurity.CanModerate)
            {
                var invitingUsers = InvitationController.GetInvitingUsers(PortalId, Utilities.UserToUTCTime(StartDate, UserInfo), Utilities.UserToUTCTime(EndDate, UserInfo), cbShowArchived.Checked);
                if (invitingUsers.Count == 0)
                {
                    divInvitingUserSelector.Visible = false;
                }
                else
                {
                    ddlInvitingUser.DataSource = invitingUsers;
                    ddlInvitingUser.DataBind();
                    ddlInvitingUser.Items.Insert(0, new ListItem(LocalizeString("AllUsers"), "0"));
                    if (invitingUsers.Where(u => u.InvitedByUserID == -1).Count() > 0)
                    {
                        ddlInvitingUser.Items.Insert(1, new ListItem(LocalizeString("AllAnonymousUsers"), "-1"));
                    }
                    ddlInvitingUser.SelectedIndex = 0;
                    divInvitingUserSelector.Visible = true;
                }
            }
            else
            {
                divInvitingUserSelector.Visible = false;
            }
        }

        private void BindGrid(DnnGrid grid, string invitingUserFilter, bool hasStatus, InvitationStatus statusFilter)
        {
            int userid;
            IEnumerable<InvitationInfo> invitations;
            
            if (int.TryParse(invitingUserFilter, out userid))
            {
                invitations = InvitationController.GetInvitationsByInvitingUser(PortalId, userid, Utilities.UserToUTCTime(StartDate, UserInfo), Utilities.UserToUTCTime(EndDate, UserInfo), cbShowArchived.Checked);
            }
            else
            {
                invitations = InvitationController.GetInvitationsByInvitingUserEmail(PortalId, invitingUserFilter, Utilities.UserToUTCTime(StartDate, UserInfo), Utilities.UserToUTCTime(EndDate, UserInfo), cbShowArchived.Checked);
            }

            if (hasStatus)
            {
                invitations = invitations.Where(i => i.HasStatus(statusFilter));
            }
            else
            {
                invitations = invitations.Where(i => i.HasNotStatus(statusFilter));
            }
            grid.DataSource = invitations;
            grid.DataBind();
        }

        private void BindAllGrids()
        {
            BindGrid(dgUnmoderatedInvitations, InvitingUserFilter, false, _unmoderated);
            BindGrid(dgPendingInvitations, InvitingUserFilter, true, InvitationStatus.Pending);
            BindGrid(dgRetractedInvitations, InvitingUserFilter, true, InvitationStatus.Retracted);
            BindGrid(dgDeclinedInvitations, InvitingUserFilter, true, InvitationStatus.Declined);
            BindGrid(dgExpiredInvitations, InvitingUserFilter, true, InvitationStatus.Expired);
            BindGrid(dgAcceptedInvitations, InvitingUserFilter, true, InvitationStatus.Accepted);
        }

        private void LoadInvitingUserStats()
        {

            var invitingUserStats = InvitationController.GetInvitingUserStats(PortalId, Utilities.UserToUTCTime(StartDate, UserInfo), Utilities.UserToUTCTime(EndDate, UserInfo), cbShowArchived.Checked, InvitingUserFilter);
            
            if (invitingUserStats == null)
            {
                divInvitingUserStats.Visible = false;
            }
            else
            {
                divInvitingUserStats.Visible = true;
                string userDisplayName;
                if (ddlInvitingUser.Visible && ddlInvitingUser.SelectedIndex != -1)
                {
                    userDisplayName = ddlInvitingUser.SelectedItem.Text;
                }
                else
                {
                    userDisplayName = UserInfo.DisplayName;
                }
                lblUserDisplayName.Text = InvitingUserDisplayName = userDisplayName;
                lblTotalInvitationCount.Text = invitingUserStats.TotalInvitationCount.ToString();
                lblApprovedInvitationCount.Text = invitingUserStats.ApprovedInvitationCount.ToString();
                lblDisapprovedInvitationCount.Text = invitingUserStats.DisapprovedInvitationCount.ToString();
                lblSentInvitationCount.Text = invitingUserStats.SentInvitationCount.ToString();
                lblRetractedInvitationCount.Text = invitingUserStats.RetractedInvitationCount.ToString();
                lblAcceptedInvitationCount.Text = invitingUserStats.AcceptedInvitationCount.ToString();
                lblDeclinedInvitationCount.Text = invitingUserStats.DeclinedInvitationCount.ToString();
                lblExpiredInvitationCount.Text = invitingUserStats.ExpiredInvitationCount.ToString();
                lblPendingInvitationCount.Text = invitingUserStats.PendingInvitationCount.ToString();
                lblLastInvitedOnDate.Text = Utilities.FormattedUTCDate(invitingUserStats.LastInvitedOnDate, UserInfo);
            }
        }

        protected void dgInvitations_ItemDataBound(object sender, GridItemEventArgs e)
        {
            var grid = (DnnGrid)sender;
            var gridName = Regex.Replace(grid.ID, @"dg(\w+)Invitations","$1");
            var invitation = e.Item.DataItem as InvitationInfo;
            if (e.Item.ItemType == GridItemType.Item || e.Item.ItemType == GridItemType.AlternatingItem)
            {
                if (invitation != null)
                {
                    switch (gridName)
                    {
                        case "Unmoderated":
                            var btnApprove = e.Item.FindControl("btnApprove") as DnnImageButton;
                            if (btnApprove != null) btnApprove.Visible = ModuleSecurity.CanModerate;
                            var btnDisapprove = e.Item.FindControl("btnDisapprove") as DnnImageButton;
                            if (btnDisapprove != null) btnDisapprove.Visible = ModuleSecurity.CanModerate;
                            var btnEditApprove = e.Item.FindControl("btnEditApprove") as DnnImageButton;
                            if (btnEditApprove != null) btnEditApprove.Visible = ModuleSecurity.CanModerate;
                            var btnDeleteUnmoderated = e.Item.FindControl("btnDeleteUnmoderated") as DnnImageButton;
                            if (btnDeleteUnmoderated != null) btnDeleteUnmoderated.Visible = ModuleSecurity.CanRetractInvitation;
                            break;
                        case "Pending":
                            var btnResendPending = e.Item.FindControl("btnResendPending") as DnnImageButton;
                            if (btnResendPending != null) btnResendPending.Visible = (ModuleSecurity.CanModerate || ((MyConfiguration.MaxResends > invitation.ResentCount) 
                                                                                       && (invitation.LastResentOnDate.Add(MyConfiguration.ResendInterval) < DateUtils.GetDatabaseTime())));
                            
                            var btnRetractPending = e.Item.FindControl("btnRetractPending") as DnnImageButton;
                            if (btnRetractPending != null) btnRetractPending.Visible = ModuleSecurity.CanRetractInvitation;
                            break;
                        case "Retracted":
                            var btnUnRetractRetracted = e.Item.FindControl("btnUnRetractRetracted") as DnnImageButton;
                            if (btnUnRetractRetracted != null) btnUnRetractRetracted.Visible = ModuleSecurity.CanRetractInvitation;
                            break;
                        case "Expired":
                            var btnExtendExpired = e.Item.FindControl("btnExtendExpired") as DnnImageButton;
                            if (btnExtendExpired != null) btnExtendExpired.Visible = ModuleSecurity.CanExtendInvitation;
                            
                            var btnDeleteExpired = e.Item.FindControl("btnDeleteExpired") as DnnImageButton;
                            if (btnDeleteExpired != null) btnDeleteExpired.Visible = MyConfiguration.AutoDeleteArchiveExpiredInvitations == ExpiredInvitationActions.Delete
                                                                                              || ModuleSecurity.IsAdministrator;
                            break;
                    }
                }
            }
        }

        public string FormatedUTCDate(DateTime value)
        {
            return Utilities.FormattedUTCDate(value, UserInfo, "g");
        }

        void dgInvitations_ItemCommand(object sender, GridCommandEventArgs e)
        {
            var grid = (DnnGrid)sender;
            var gridName = Regex.Replace(grid.ID, @"dg(\w+)Invitations","$1");
            var invitationID = (int)grid.MasterTableView.DataKeyValues[e.Item.ItemIndex]["InvitationID"];
            var invitedByUserID = (int)grid.MasterTableView.DataKeyValues[e.Item.ItemIndex]["InvitedByUserID"];
            InvitationInfo currentInvitation = null;

            switch (e.CommandName.ToLowerInvariant())
            {
                case "select":
                    break;
                case "edit":
                    Response.Redirect(Globals.NavigateURL(TabId, "", new string[] {"id=" + invitationID.ToString(), "mode=2", "returnurl=" + UrlUtils.EncodeParameter(Request.RawUrl) }), true);
                    break;
                case "archive":
                    InvitationController.UpdateInvitationStatus (invitationID, "archived", UserId);
                    BindGrid(grid, InvitingUserFilter, true, (InvitationStatus)Enum.Parse(typeof(InvitationStatus), gridName));
                    break;
                case "resend":
                    var mailer = new MailManager(invitationID);
                    var errMsg = mailer.SendBulkMail("send");
                    if (string.IsNullOrEmpty(errMsg))
                    {
                        InvitationController.UpdateInvitationStatus(invitationID, "resent", UserId);
                        currentInvitation = InvitationController.GetInvitation(invitationID);
                        NotificationsHelper.SendNotifications(currentInvitation, Notifications.Resent);
                    }
                    BindGrid(grid, InvitingUserFilter, true, (InvitationStatus)Enum.Parse(typeof(InvitationStatus), gridName));
                    break;
                case "retract":
                    InvitationController.UpdateInvitationStatus(invitationID, "retracted", UserId);
                    BindGrid(grid, InvitingUserFilter, true, (InvitationStatus)Enum.Parse(typeof(InvitationStatus), gridName));
                    BindGrid(dgRetractedInvitations, InvitingUserFilter, true, InvitationStatus.Retracted);
                    currentInvitation = InvitationController.GetInvitation(invitationID);
                    NotificationsHelper.SendNotifications(currentInvitation, Notifications.Retracted);
                    break;
                case "unretract":
                    InvitationController.UpdateInvitationStatus(invitationID, "unretracted", UserId);
                    BindGrid(grid, InvitingUserFilter, true, (InvitationStatus)Enum.Parse(typeof(InvitationStatus), gridName));
                    BindGrid(dgPendingInvitations, InvitingUserFilter, true, InvitationStatus.Pending);

                    // TO DO: Optionally resend invitation ?

                    break;
                case "extend":
                    currentInvitation = InvitationController.GetInvitation(invitationID);
                    var newExpiresOnDate = DateUtils.GetDatabaseTime() + MyConfiguration.ValidityPeriod;
                    InvitationController.UpdateInvitationStatus(invitationID, "extended", UserId, newExpiresOnDate);
                    BindGrid(grid, InvitingUserFilter, true, (InvitationStatus)Enum.Parse(typeof(InvitationStatus), gridName));
                    BindGrid(dgPendingInvitations, InvitingUserFilter, true, InvitationStatus.Pending);

                    // TO DO: Optionally resend invitation ?
                    break;

                case "delete":
                    InvitationController.DeleteInvitation(invitationID);
                    if (gridName == "Unmoderated")
                    {
                        BindGrid(grid, InvitingUserFilter, false, _unmoderated);
                    }
                    else
                    {
                        BindGrid(grid, InvitingUserFilter, true, (InvitationStatus)Enum.Parse(typeof(InvitationStatus), gridName));
                    }
                    break;

                case "approve":
                    if (ModuleSecurity.CanModerate)
                    {
                        currentInvitation = InvitationController.GetInvitation(invitationID);
                        Utilities.ModerateInvitation(currentInvitation, true, UserInfo.UserID);
                    }
                    else Response.Redirect(DotNetNuke.Common.Globals.AccessDeniedURL(), true);
                    break;

                case "disapprove":
                    if (ModuleSecurity.CanModerate)
                    {
                        currentInvitation = InvitationController.GetInvitation(invitationID);
                        Utilities.ModerateInvitation(currentInvitation, false, UserInfo.UserID);
                    }
                    else Response.Redirect(DotNetNuke.Common.Globals.AccessDeniedURL(), true);
                    break;

                case "editapprove":
                    if (ModuleSecurity.CanModerate)
                    {
                        Response.Redirect(Globals.NavigateURL(TabId, "", new string[] { "id=" + invitationID.ToString(), "mode=3", "returnurl=" + UrlUtils.EncodeParameter(Request.RawUrl) }), true);
                    }
                    else Response.Redirect(DotNetNuke.Common.Globals.AccessDeniedURL(), true);
                    break;

                case "sort":
                    if (gridName == "Unmoderated")
                    {
                        BindGrid(grid, InvitingUserFilter, false, _unmoderated);
                    }
                    else
                    {
                        BindGrid(grid, InvitingUserFilter, true, (InvitationStatus)Enum.Parse(typeof(InvitationStatus), gridName));
                    }                
                    break;
            }
        }

        protected void lnkSearchAgain_Click(object sender, EventArgs e)
        {
            LoadInvitingUserStats();
            BindAllGrids();
        }

        protected void cmdReturn_Click(object sender, EventArgs e)
        {
            Response.Redirect(Globals.NavigateURL(), true);
        }

    }
}