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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Profile;
using DotNetNuke.Entities.Tabs;
using DotNetNuke.Entities.Users;
using DotNetNuke.Framework.JavaScriptLibraries;
using DotNetNuke.Security.Membership;
using DotNetNuke.Security.Roles;
using DotNetNuke.Services.Localization;
using DotNetNuke.Services.SystemDateTime;
using DotNetNuke.UI.Skins.Controls;
using Telerik.Web.UI;

namespace WESNet.DNN.Modules.ByInvitation
{
    public partial class SendInvitation : ByInvitationModuleBase
    {
        public bool AddingInvitation
        {
            get
            {
                return (CurrentInvitationID == -1);
            }
        }

        public override string CultureCode
        {
            get
            {
                if (divCultureCode.Visible && ctlCultureCode.SelectedValue != "")
                {
                    return ctlCultureCode.SelectedValue;
                }
                else
                {
                    return ControlMode == Mode.Edit || ControlMode == Mode.EditApprove ? CurrentInvitation.RecipientCultureCode : base.CultureCode;
                }
            }
            set
            {
                if (divCultureCode.Visible && LocaleController.Instance.GetLocales(PortalId).ContainsKey(value))
                {
                    ctlCultureCode.SetLanguage(value);
                }
            }
        }

        public int RedirectOnFirstLoginTabID
        {
            get
            {
                if (divRedirection.Visible && ddlRedirectOnFirstLogin.SelectedValue != "")
                {
                    return Convert.ToInt32(ddlRedirectOnFirstLogin.SelectedValue);
                }
                else
                {
                    return ControlMode == Mode.Edit || ControlMode == Mode.EditApprove ? CurrentInvitation.RedirectOnFirstLogin : PortalSettings.HomeTabId;
                }
            }
            set
            {
                if (divRedirection.Visible)
                {
                    var strValue = value.ToString();
                    if (value != -1 && ddlRedirectOnFirstLogin.Items.FindByValue(strValue) != null)
                    {
                        ddlRedirectOnFirstLogin.SelectedValue = strValue;
                    }
                    else
                    {
                        ddlRedirectOnFirstLogin.SelectedIndex = 0;
                    }
                }
                else
                {
                    CurrentInvitation.RedirectOnFirstLogin = value;
                }
            }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            JavaScript.RequestRegistration(CommonJs.DnnPlugins);

            cmdSend.Click += new EventHandler(cmdSend_Click);
            cmdUpdate.Click += new EventHandler(cmdUpdate_Click);
            cmdCancel.NavigateUrl = ReturnUrl;
            dgAssignedRoles.ItemCommand += new GridCommandEventHandler(dgAssignedRoles_ItemCommand);
            btnAddRole.Click += new ImageClickEventHandler(btnAddRole_Click);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                var testObject = new System.IdentityModel.Tokens.ConfigurationBasedIssuerNameRegistry();
            }
            catch
            {
                divInvitationHtml.InnerText = "ASP.Net 4.5 application pool is required for this module";
            }

            if (!ModuleSecurity.CanInvite)
            {
                Response.Redirect(DotNetNuke.Common.Globals.AccessDeniedURL(), true);
            }

            if (!Page.IsPostBack) MyConfiguration.VerifyOrAddInvitationProcessor();

            Mode mode = Mode.View;
            if (Request.QueryString["mode"] != null)
            {
                mode = (Mode)Enum.Parse(typeof(Mode), Request.QueryString["mode"]);
                ControlMode = mode;
            }

            if (mode == Mode.EditApprove && !ModuleSecurity.CanModerate)
            {
                Response.Redirect(DotNetNuke.Common.Globals.AccessDeniedURL(), true);
            }

            lnkManageInvitations.NavigateUrl = EditUrl("manage");
            lnkManageInvitations.Visible = ModuleSecurity.CanManageOwnInvitations && GetInvitationCount(UserId) > 0;

            lnkBulkImportInvitations.NavigateUrl = EditUrl("bulkimport");
            lnkBulkImportInvitations.Visible = ModuleSecurity.CanBulkImportInvitations;

            if ((ControlMode == Mode.View || ControlMode == Mode.Add) && ModuleSecurity.HasExceededAllocatedInvitations())
            {
                divInvitationHtml.InnerText = string.Format(LocalizeString("InvitationAllocationExceeded.Error"), ModuleSecurity.AllocatedInvitations, ModuleSecurity.AllocationPeriod);
                btnSendInvitation.Visible = false;
                divSendInvitation.Visible = true;
                divEditInvitation.Visible = false;
                return;
            }

            switch (ControlMode)
            {
                case Mode.View:
                    divSendInvitation.Visible = true;
                    divEditInvitation.Visible = false;

                    var invitationHtml = HttpUtility.HtmlDecode(MyLocalizedConfiguration.SendInvitationHtml);
                    divInvitationHtml.InnerHtml = Globals.ManageUploadDirectory(invitationHtml, PortalSettings.HomeDirectory);

                    var buttonImageUrl = Utilities.ResolveImageUrl(PortalId, MyLocalizedConfiguration.SendInvitationButtonImage);
                    if (string.IsNullOrEmpty(buttonImageUrl))
                    {
                        btnSendInvitation.Text = MyLocalizedConfiguration.SendInvitationButtonText;
                        btnSendInvitation.Image.ImageUrl = "";
                    }
                    else
                    {
                        btnSendInvitation.ToolTip = MyLocalizedConfiguration.SendInvitationButtonText;
                        btnSendInvitation.Image.ImageUrl = buttonImageUrl;
                        btnSendInvitation.Image.IsBackgroundImage = true;

                        var width = new System.Web.UI.WebControls.Unit(MyLocalizedConfiguration.ImageButtonSize.Width);
                        var height = new System.Web.UI.WebControls.Unit(MyLocalizedConfiguration.ImageButtonSize.Height);
                        btnSendInvitation.Width = width;
                        btnSendInvitation.Height = height;
                        btnSendInvitation.Style.Add("background-size", width.ToString() + " " + height.ToString());
                    }

                    btnSendInvitation.NavigateUrl = Globals.NavigateURL(TabId, "", "mode=Add");
                    break;

                case Mode.Add:
                    cmdSend.Visible = true;
                    cmdSend.Text = LocalizeString("cmdSend");
                    cmdUpdate.Visible = false;
                    lnkManageInvitations.Visible = false;
                    lnkBulkImportInvitations.Visible = false;
                    divSendInvitation.Visible = false;
                    divEditInvitation.Visible = true;
                    InitializeEditControls();
                    break;

                case Mode.Edit:
                case Mode.EditApprove:
                    int id = -1;
                    if (Request.QueryString["id"] != null)
                    {
                        int.TryParse(Request.QueryString["id"], out id);
                    }

                    if (id == -1)
                    {
                        Response.Redirect(DotNetNuke.Common.Globals.NavigateURL(TabId), true);
                    }
                    else
                    {
                        InvitationInfo invitation = InvitationController.GetInvitation(id);
                        if (invitation == null)
                        {
                            Response.Redirect(DotNetNuke.Common.Globals.NavigateURL(TabId), true);
                        }

                        if (ModuleSecurity.IsAdministrator || ModuleSecurity.CanModerate || (ModuleSecurity.IsInvitingUser(invitation) && ModuleSecurity.CanManageOwnInvitations))
                        {
                            CurrentInvitationID = id;
                            CurrentInvitation = invitation;
                            if (!Page.IsPostBack)
                            {
                                if (Request.QueryString["returnurl"] != null)
                                {
                                    var returnUrl = (string)Request.QueryString["returnurl"];
                                    ReturnUrl = DotNetNuke.Common.Utilities.UrlUtils.DecodeParameter(returnUrl);
                                }
                            }
                        }
                        else
                        {
                            Response.Redirect(Globals.AccessDeniedURL(), true);
                        }
                    }

                    lnkManageInvitations.Visible = false;
                    lnkBulkImportInvitations.Visible = false;
                    divSendInvitation.Visible = false;
                    divEditInvitation.Visible = true;
                    cmdSend.Visible = true;
                    cmdSend.Text = mode == Mode.EditApprove ? LocalizeString("cmdUpdateAndApprove") : LocalizeString("cmdUpdateAndResend");
                    cmdUpdate.Visible = true;
                    cmdUpdate.Text = LocalizeString("cmdUpdateOnly");
                    InitializeEditControls();

                    break;
                default:
                    Response.Redirect(Globals.AccessDeniedURL(), true);
                    break;
            }
            cmdCancel.NavigateUrl = ReturnUrl;
        }

        protected void InitializeEditControls()
        {
            var emailRegex = MyLocalizedConfiguration.EmailRegex;

            if (ModuleSecurity.IsAnonymous || ControlMode == Mode.EditApprove)
            {
                divInvitedByUser.Visible = true;
                valInvitedByUserFullName.Enabled = true;
                valInvitedByUserEmail.Enabled = true;
                val2InvitedByUserEmail.Enabled = true;
                if (!string.IsNullOrEmpty(emailRegex)) val2InvitedByUserEmail.ValidationExpression = emailRegex;
            }
            else
            {
                divInvitedByUser.Visible = false;
                valInvitedByUserFullName.Enabled = false;
                valInvitedByUserEmail.Enabled = false;
                val2InvitedByUserEmail.Enabled = false;
            }

            if (!string.IsNullOrEmpty(emailRegex)) val2RecipientEmail.ValidationExpression = emailRegex;

            if (MyConfiguration.RequireRecipientEmailConfirm)
            {
                divRecipientEmail2.Visible = true;
                valRecipientEmail2.Enabled = true;
            }
            else
            {
                divRecipientEmail2.Visible = false;
            }

            if (ProfileController.GetPropertyDefinitionByName(PortalId, UserProfile.USERPROFILE_FirstName).Required)
            {
                plRecipientFirstName.CssClass = "dnnFormRequired";
                valRecipientFirstName.Enabled = true;
            }
            else
            {
                plRecipientFirstName.CssClass = "";
                valRecipientFirstName.Enabled = false;
            }

            if (ProfileController.GetPropertyDefinitionByName(PortalId, UserProfile.USERPROFILE_LastName).Required)
            {
                plRecipientLastName.CssClass = "dnnFormRequired";
                valRecipientLastName.Enabled = true;
            }
            else
            {
                plRecipientLastName.CssClass = "";
                valRecipientLastName.Enabled = false;
            }

            divCultureCode.Visible = LocaleController.Instance.GetLocales(PortalId).Count > 1;

            if (ModuleSecurity.CanAssignCredentials)
            {
                divCredentials.Visible = true;

                if (ModuleSecurity.UseEmailAsUsername)
                {
                    divUsername.Visible = false;
                    valAssignedUsername.Enabled = false;
                    val2AssignedUsername.Enabled = false;
                }
                else
                {
                    tbAssignedUsername.Attributes.Add("AUTOCOMPLETE", "off");
                    val2AssignedUsername.ValidationExpression = ModuleSecurity.UsernameValidation;
                    val2AssignedUsername.Enabled = val2AssignedUsername.ValidationExpression != string.Empty;
                }

                if (MyConfiguration.TemporaryPasswordMode == TemporaryPasswordMode.OptionalTemporaryPassword || MyConfiguration.TemporaryPasswordMode == TemporaryPasswordMode.RequiredTemporaryPassword)
                {
                    divTemporaryPassword.Visible = true;
                    valTemporaryPassword.Enabled = MyConfiguration.TemporaryPasswordMode == TemporaryPasswordMode.RequiredTemporaryPassword;
                    plTemporaryPassword.CssClass = valTemporaryPassword.Enabled ? "dnnFormRequired" : "";
                    tbTemporaryPassword.Attributes.Add("AUTOCOMPLETE", "off");

                    var passwordRegex = MembershipProviderConfig.PasswordStrengthRegularExpression;
                    val2TemporaryPassword.ValidationExpression = string.IsNullOrEmpty(passwordRegex) ? ".*" : passwordRegex;
                    if (MyConfiguration.RequireTemporaryPasswordConfirm)
                    {
                        divTemporaryPassword2.Visible = true;
                        valTemporaryPassword2.Enabled = valTemporaryPassword.Enabled;
                        plTemporaryPassword2.CssClass = valTemporaryPassword2.Enabled ? "dnnFormRequired" : "";
                        tbTemporaryPassword2.Attributes.Add("AUTOCOMPLETE", "off");
                    }
                    else
                    {
                        divTemporaryPassword2.Visible = false;
                        valTemporaryPassword2.Enabled = false;
                    }
                }
                else
                {
                    divTemporaryPassword.Visible = false;
                    valTemporaryPassword.Enabled = false;
                    val2TemporaryPassword.Enabled = false;
                    divTemporaryPassword2.Visible = false;
                    valTemporaryPassword2.Enabled = false;
                }
            }
            else
            {
                divCredentials.Visible = false;
            }

            if (ModuleSecurity.DisplayNameFormat == "")
            {
                divDisplayName.Visible = true;
                valAssignedDisplayName.Enabled = true;
                val2AssignedDisplayName.ValidationExpression = ModuleSecurity.RegistrationExcludedTermsRegex;
                val2AssignedDisplayName.Enabled = val2AssignedDisplayName.ValidationExpression != string.Empty;
                tbAssignedDisplayName.Attributes.Add("AUTOCOMPLETE", "off");
                plAssignedDisplayName.CssClass = "dnnFormRequired";
            }
            else
            {
                divDisplayName.Visible = false;
                valAssignedDisplayName.Enabled = false;
                val2AssignedDisplayName.Enabled = false;
                plAssignedDisplayName.CssClass = string.Empty;
            }

            divPersonalNote.Visible = MyConfiguration.EnablePersonalNote;
            divRedirection.Visible = ModuleSecurity.CanAssignRedirection;

            dnnRolesAssignmentForm.Visible = ModuleSecurity.AssignableRoles.Count > 0;

            if (!Page.IsPostBack)
            {
                ctlCultureCode.DataBind();
                BindRedirectOnFirstLoginTabs();
                BindAssignedRoles(false);
                BindAddRolesDDL();

                if (ControlMode == Mode.Add && !ModuleSecurity.IsAnonymous)
                {
                    tbInvitedByUserFullName.Text = UserInfo.FirstName + " " + UserInfo.LastName;
                    tbInvitedByUserEmail.Text = UserInfo.Email;
                }
                else if (ControlMode == Mode.Edit || ControlMode == Mode.EditApprove)
                {
                    tbInvitedByUserFullName.Text = CurrentInvitation.InvitedByUserFullName;
                    tbInvitedByUserEmail.Text = CurrentInvitation.InvitedByUserEmail;
                    tbRecipientEmail.Text = CurrentInvitation.RecipientEmail;
                    tbRecipientEmail2.Text = tbRecipientEmail.Text;
                    tbRecipientFirstName.Text = CurrentInvitation.RecipientFirstName;
                    tbRecipientLastName.Text = CurrentInvitation.RecipientLastName;
                    CultureCode = CurrentInvitation.RecipientCultureCode;
                    RedirectOnFirstLoginTabID = CurrentInvitation.RedirectOnFirstLogin;
                    tbAssignedUsername.Text = CurrentInvitation.AssignedUsername;
                    tbTemporaryPassword.Text = Security.DecryptString(CurrentInvitation.TemporaryPassword, CurrentInvitation.TemporaryPasswordSalt);
                    tbTemporaryPassword2.Text = tbTemporaryPassword.Text;
                    tbAssignedDisplayName.Text = CurrentInvitation.AssignedDisplayName;
                    tbPersonalNote.Text = CurrentInvitation.PersonalNote;
                }
            }
            
            tbTemporaryPassword.Attributes.Add("value", tbTemporaryPassword.Text);
            tbTemporaryPassword2.Attributes.Add("value", tbTemporaryPassword2.Text);

            if (ControlMode == Mode.Add && (MyConfiguration.EnableInvitationCaptcha == CaptchaUsage.AllUsers || MyConfiguration.EnableInvitationCaptcha == CaptchaUsage.AnonymousUsersOnly && ModuleSecurity.IsAnonymous))
            {
                divCaptcha.Visible = true;
                ctlCaptcha.ErrorMessage = Localization.GetString("ctlCaptcha.Error", Configuration.LocalSharedResourceFile);
                ctlCaptcha.CaptchaTextBoxLabel = Localization.GetString("ctlCaptcha.TextBoxLabel", Configuration.LocalSharedResourceFile);
                ctlCaptcha.CaptchaLinkButtonText = Localization.GetString("ctlCaptchaLinkButton.Text", Configuration.LocalSharedResourceFile);
                ctlCaptcha.CaptchaAudioLinkButtonText = Localization.GetString("ctlCaptchaAudioLinkButton.Text", Configuration.LocalSharedResourceFile);
                ctlCaptcha.CaptchaImage.EnableCaptchaAudio = MyConfiguration.EnableCaptchaAudio;
                ctlCaptcha.IgnoreCase = MyConfiguration.CaptchaIsCaseInsensitive;
                ctlCaptcha.CaptchaImage.LineNoise = MyConfiguration.CaptchaLineNoise;
                ctlCaptcha.CaptchaImage.BackgroundNoise = MyConfiguration.CaptchaBackgroundNoise;
                ctlCaptcha.ValidationGroup = Consts.ValidationGroup;
            }
            else
            {
                divCaptcha.Visible = false;
            }

            SetValidationGroup();

        }

        private void BindRedirectOnFirstLoginTabs()
        {
            List<TabInfo> tabs = TabController.GetPortalTabs(PortalId, TabId, true, true);
            ddlRedirectOnFirstLogin.DataSource = tabs;
            ddlRedirectOnFirstLogin.DataBind();
        }

        private void SetValidationGroup()
        {
            valInvitedByUserFullName.ValidationGroup = Consts.ValidationGroup;
            valInvitedByUserEmail.ValidationGroup = Consts.ValidationGroup;
            val2InvitedByUserEmail.ValidationGroup = Consts.ValidationGroup;
            valRecipientEmail.ValidationGroup = Consts.ValidationGroup;
            val2RecipientEmail.ValidationGroup = Consts.ValidationGroup;
            valRecipientEmail2.ValidationGroup = Consts.ValidationGroup;
            val2RecipientEmail2.ValidationGroup = Consts.ValidationGroup;
            valRecipientFirstName.ValidationGroup = Consts.ValidationGroup;
            valRecipientLastName.ValidationGroup = Consts.ValidationGroup;
            valTemporaryPassword.ValidationGroup = Consts.ValidationGroup;
            val2TemporaryPassword.ValidationGroup = Consts.ValidationGroup;
            valTemporaryPassword2.ValidationGroup = Consts.ValidationGroup;
            val2TemporaryPassword2.ValidationGroup = Consts.ValidationGroup;
            valAssignedUsername.ValidationGroup = Consts.ValidationGroup;
            valAssignedDisplayName.ValidationGroup = Consts.ValidationGroup;
            val2AssignedDisplayName.ValidationGroup = Consts.ValidationGroup;
            cmdSend.ValidationGroup = Consts.ValidationGroup;
            cmdUpdate.ValidationGroup = Consts.ValidationGroup;
        }

        protected void BindAssignedRoles(bool refreshData)
        {
            if (CurrentInvitationID != -1 && refreshData) CurrentInvitation.RefreshAssignedRoles();
            dgAssignedRoles.DataSource = CurrentInvitation.AssignedRoles;
            dgAssignedRoles.DataBind();
        }

        protected void BindAddRolesDDL()
        {
            var filteredAssignableRoles = new List<RoleInfo>(ModuleSecurity.AssignableRoles);
            foreach (GridItem itm in dgAssignedRoles.MasterTableView.Items)
            {
                if (itm.ItemType == GridItemType.Item || itm.ItemType == GridItemType.AlternatingItem)
                {
                    var roleID = (int)dgAssignedRoles.MasterTableView.DataKeyValues[itm.ItemIndex]["RoleID"];
                    filteredAssignableRoles.RemoveAll(roleInfo => roleInfo.RoleID == roleID);
                }
            }

            ddlRoles.DataSource = filteredAssignableRoles;
            ddlRoles.DataBind();
            ddlRoles.Items.Insert(0, new ListItem(LocalizeString("SelectRoleToAssign"), "-1"));
            divAddRole.Visible = ddlRoles.Items.Count > 1;
        }

        protected void dgAssignedRoles_ItemCommand(object sender, GridCommandEventArgs e)
        {

            int roleID = -1;
            if (e.Item.ItemIndex != -1)
            {
                roleID = (int)dgAssignedRoles.MasterTableView.DataKeyValues[e.Item.ItemIndex]["RoleID"];
            }

            switch (e.CommandName)
            {
                case "Edit":
                    e.Item.Edit = true;
                    BindAssignedRoles(false);
                    divAddRole.Visible = false;
                    break;
                case "Delete":
                    int assignableRoleID = (int)dgAssignedRoles.MasterTableView.DataKeyValues[e.Item.ItemIndex]["AssignableRoleID"];
                    if (assignableRoleID != -1)
                    {
                        InvitationController.DeleteAssignableRole(assignableRoleID);
                        BindAssignedRoles(true);
                    }
                    else
                    {
                        var roleToRemove = CurrentInvitation.AssignedRoles.Find(role => role.RoleID == roleID);
                        CurrentInvitation.AssignedRoles.Remove(roleToRemove);
                        BindAssignedRoles(false);
                    }
                    BindAddRolesDDL();
                    break;
                case "Save":
                    GridEditManager editManager = null;
                    GridEditableItem editableItem = e.Item as GridEditableItem;
                    if (editableItem != null)
                    {
                        editManager = editableItem.EditManager;
                        var pickerEffectiveDate = editManager.GetColumnEditor("EffectiveDate") as GridDateTimeColumnEditor;
                        var pickerExpiryDate = editManager.GetColumnEditor("ExpiryDate") as GridDateTimeColumnEditor;
                        var roleToUpdate = CurrentInvitation.AssignedRoles.Find(role => role.RoleID == roleID);
                        if (pickerEffectiveDate != null && pickerEffectiveDate.PickerControl.SelectedDate.HasValue) roleToUpdate.EffectiveDate = pickerEffectiveDate.PickerControl.SelectedDate;
                        if (pickerExpiryDate != null && pickerExpiryDate.PickerControl.SelectedDate.HasValue) roleToUpdate.ExpiryDate = pickerExpiryDate.PickerControl.SelectedDate;
                        e.Item.Edit = false;
                        BindAssignedRoles(false);
                        BindAddRolesDDL();
                    }
                    break;
                case "Cancel":
                    e.Item.Edit = false;
                    BindAssignedRoles(false);
                    divAddRole.Visible = ddlRoles.Items.Count > 0;
                    break;
            }
        }

        private InvitationSubmissionStatus LoadFormData()
        {
            var dataBaseDateTime = DateUtils.GetDatabaseTime();
            var invitingUserEmail = tbInvitedByUserEmail.Text.Trim().ToLowerInvariant();

            if (ControlMode == Mode.Add)
            {
                var invitedByUserIPAddr = Security.GetUserIPAddress(true);
                var invitationFiltering = MyConfiguration.InvitationFiltering;
                if (invitationFiltering != InvitationFilter.NoFiltering)
                {
                    var cutoffDate = dataBaseDateTime - MyConfiguration.InvitationFilteringInterval;
                    IEnumerable<InvitationInfo> lastSubmitted;
                    if (invitationFiltering == InvitationFilter.SenderUserID && ModuleSecurity.IsAnonymous)
                    {
                        invitationFiltering = InvitationFilter.SenderEmail;
                    }

                    switch (invitationFiltering)
                    {
                        case InvitationFilter.SenderEmail:
                            lastSubmitted = InvitationController.GetInvitationsByInvitingUserEmail(PortalId, invitingUserEmail).Take(1);
                            if (lastSubmitted.Count() == 1 && lastSubmitted.Single().InvitedOnDate > cutoffDate)
                            {
                                return InvitationSubmissionStatus.SubmittedTooSoon;
                            }
                            break;
                        case InvitationFilter.SenderIPAddr:
                            lastSubmitted = InvitationController.GetInvitationsByInvitingUserIPAddr(PortalId, invitedByUserIPAddr).Take(1);
                            if (lastSubmitted.Count() == 1 && lastSubmitted.Single().InvitedOnDate > cutoffDate)
                            {
                                return InvitationSubmissionStatus.SubmittedTooSoon;
                            }
                            break;
                        case InvitationFilter.SenderUserID:
                            lastSubmitted = InvitationController.GetInvitationsByInvitingUser(PortalId, UserId, DateTime.MinValue, dataBaseDateTime, true).Take(1);
                            if (lastSubmitted.Count() == 1 && lastSubmitted.Single().InvitedOnDate > cutoffDate)
                            {
                                return InvitationSubmissionStatus.SubmittedTooSoon;
                            }
                            break;
                    }
                }

                CurrentInvitation.InvitedByUserID = UserId;
                CurrentInvitation.InvitedByUserFullName = tbInvitedByUserFullName.Text;
                CurrentInvitation.InvitedByUserEmail = invitingUserEmail;
                CurrentInvitation.InvitedByUserIPAddr = invitedByUserIPAddr;
                CurrentInvitation.InvitedOnDate = dataBaseDateTime;
            }

            CurrentInvitation.RecipientCultureCode = CultureCode;
            CurrentInvitation.RecipientEmail = tbRecipientEmail.Text;
            CurrentInvitation.RecipientFirstName = tbRecipientFirstName.Text;
            CurrentInvitation.RecipientLastName = tbRecipientLastName.Text;
            CurrentInvitation.PersonalNote = tbPersonalNote.Text;
            CurrentInvitation.AssignedUsername = tbAssignedUsername.Text;
            CurrentInvitation.AssignedDisplayName = tbAssignedDisplayName.Text;
            CurrentInvitation.TemporaryPassword = tbTemporaryPassword.Text;
            CurrentInvitation.RedirectOnFirstLogin = RedirectOnFirstLoginTabID;

            return Utilities.CleanAndVerifyData(CurrentInvitation, cbGenerateRandomPassword.Checked);

        }

        protected void btnAddRole_Click(object sender, EventArgs e)
        {
            int roleID;

            if (ddlRoles.SelectedIndex > 0)
            {
                roleID = int.Parse(ddlRoles.SelectedValue);
                var roleInfo = new RoleController().GetRoleById(PortalId, roleID);
                var assignableRole = new AssignableRoleInfo(roleID, roleInfo.RoleName, roleInfo.Description);
                CurrentInvitation.AssignedRoles.Add(assignableRole);
                BindAssignedRoles(false);
            }
        }

        protected void cmdUpdate_Click(object sender, EventArgs e)
        {
            CreateOrUpdateInvitation();
            ControlMode = Mode.View;
        }

        protected void cmdSend_Click(object sender, EventArgs e)
        {
            if (ModuleSecurity.IsAnonymous)
            {
                var invitedByUserEmail = tbInvitedByUserEmail.Text.Trim().ToLowerInvariant();
                if (ModuleSecurity.HasExceededAllocatedInvitations(invitedByUserEmail))
                {
                    divInvitationHtml.InnerText = string.Format(LocalizeString("InvitationAllocationExceeded.Error"), ModuleSecurity.AllocatedInvitations, ModuleSecurity.AllocationPeriod);
                    btnSendInvitation.Visible = false;
                    divSendInvitation.Visible = true;
                    divEditInvitation.Visible = false;
                    return;
                }
            }

            if (CreateOrUpdateInvitation())
            {

                var notificationsSent = 0;
                var errMsg = "";

                try
                {
                    if (!CurrentInvitation.IsApproved)
                    {
                        notificationsSent += NotificationsHelper.SendNotifications(CurrentInvitation, Notifications.Created | Notifications.PendingModeration);
                        divSendInvitation.Visible = false;
                        divEditInvitation.Visible = false;
                        Utilities.AddTimedModuleMessage(this, LocalizeString("ModerationRequired"), ModuleMessage.ModuleMessageType.YellowWarning, Globals.NavigateURL(TabId));
                    }
                    else
                    {
                        if (ControlMode == Mode.EditApprove)
                        {
                            NotificationsHelper.DeleteAllNotificationRecipients(Consts.ModerationRequestedNotification.Name, CurrentInvitationID);
                            NotificationsHelper.SendNotifications(CurrentInvitation, Notifications.Approved);
                        }

                        var bulkMailer = new MailManager(CurrentInvitation);
                        errMsg += bulkMailer.SendBulkMail("send");

                        if (string.IsNullOrEmpty(errMsg))
                        {
                            CurrentInvitation = InvitationController.UpdateInvitationStatus(CurrentInvitation.InvitationID, "sent", CurrentInvitation.InvitedByUserID);
                            notificationsSent += NotificationsHelper.SendNotifications(CurrentInvitation, Notifications.Created | Notifications.Sent);
                            divSendInvitation.Visible = false;
                            divEditInvitation.Visible = false;
                            Utilities.AddTimedModuleMessage(this, Utilities.LocalizeInvitationSubmissionStatus(InvitationSubmissionStatus.Success), ModuleMessage.ModuleMessageType.GreenSuccess, Globals.NavigateURL(TabId));
                        }
                        else
                        {
                            notificationsSent += NotificationsHelper.SendNotifications(CurrentInvitation, Notifications.Errored, errMsg);
                            AddLocalizedModuleMessage(errMsg, ModuleMessage.ModuleMessageType.BlueInfo);
                        }
                    }
                }
                catch (Exception exc)
                {
                    DotNetNuke.Services.Exceptions.Exceptions.ProcessModuleLoadException(this, exc);
                }
            }
        }

        private bool CreateOrUpdateInvitation()
        {
            InvitationSubmissionStatus status = InvitationSubmissionStatus.Undefined;

            if (Page.IsValid && (!divCaptcha.Visible || ctlCaptcha.IsValid))
            {
                try
                {
                    status = LoadFormData();
                    if (status != InvitationSubmissionStatus.Success)
                    {
                        AddLocalizedModuleMessage(Utilities.LocalizeInvitationSubmissionStatus(status), ModuleMessage.ModuleMessageType.RedError);
                        return false;
                    }

                    var databaseTime = DateUtils.GetDatabaseTime();

                    if (ControlMode == Mode.EditApprove)
                    {
                        CurrentInvitation.ApprovedByUserID = UserId;
                        CurrentInvitation.ApprovedOnDate = databaseTime;
                        if (MyConfiguration.ValidityPeriod != TimeSpan.MinValue)
                        {
                            CurrentInvitation.ExpiresOnDate = databaseTime.Add(MyConfiguration.ValidityPeriod);
                        }
                        else
                        {
                            CurrentInvitation.ExpiresOnDate = DateTime.MinValue;
                        }
                    }
                    else if (ControlMode == Mode.Add && (!MyConfiguration.RequireModeration || ModuleSecurity.CanManageOwnInvitations))
                    {
                        CurrentInvitation.ApprovedByUserID = UserId;
                        CurrentInvitation.ApprovedOnDate = databaseTime;
                    }

                    CurrentInvitationID = InvitationController.UpdateInvitation(CurrentInvitation, UserId);

                    if (CurrentInvitation.AssignedRoles.Count > 0)
                    {
                        foreach (var ar in CurrentInvitation.AssignedRoles)
                        {
                            ar.InvitationID = CurrentInvitationID;
                            InvitationController.UpdateAssignableRole(ar, UserId);
                        }
                    }
                    return true;
                }
                catch (Exception exc)
                {
                    DotNetNuke.Services.Exceptions.Exceptions.ProcessModuleLoadException(this, exc);
                }
            }
            return false;
        }

        private int GetInvitationCount(int userID)
        {
            if (ModuleSecurity.IsAdministrator)
            {
                userID = -1;
            }
            return InvitationController.GetInvitationCount(PortalId, userID, DateTime.MinValue, DateTime.MinValue);
        }
    }
}