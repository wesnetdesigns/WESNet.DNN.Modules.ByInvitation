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
using System.Linq;
using System.Text;
using System.Threading;
using System.Web.UI;
using System.Collections.Specialized;
using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Profile;
using DotNetNuke.Entities.Users;
using DotNetNuke.Framework;
using DotNetNuke.Security;
using DotNetNuke.Security.Membership;
using DotNetNuke.Security.Roles;
using DotNetNuke.Services.Authentication;
using DotNetNuke.Services.Localization;
using DotNetNuke.Services.SystemDateTime;
using DotNetNuke.UI.Skins.Controls;
using DotNetNuke.UI.WebControls;


namespace WESNet.DNN.Modules.ByInvitation
{
    public partial class ProcessInvitation : UserModuleBase
    {
        private int _PortalId = -1;
        private string _RecipientEmail = "";
        private string _RSVPCode = "";

        private Configuration _MyConfiguration = null;
        private LocalizedConfiguration _MyLocalizedConfiguration = null;
        private Security _ModuleSecurity = null;
        private InvitationInfo _Invitation = null;
        private string[] validActions = { "join", "decline" };

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

        public LocalizedConfiguration MyLocalizedConfiguration
        {
            get
            {
                if (_MyLocalizedConfiguration == null)
                {
                    _MyLocalizedConfiguration = MyConfiguration.GetLocalizedConfiguration(CultureCode, PortalSettings.CultureCode);
                }
                return _MyLocalizedConfiguration;
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

        public UserProfile UserProfile
        {
            get
            {
                UserProfile _Profile = null;
                if (User != null)
                {
                    _Profile = User.Profile;
                }
                return _Profile;
            }
        }

        public string Action
        {
            get
            {
                return ViewState["Action"] == null ? "" : (string)ViewState["Action"];
            }
            set
            {
                ViewState["Action"] = value;
            }
        }

        public int InvitationID
        {
            get
            {
                return ViewState["InvitationID"] == null ? -1 : (int)ViewState["InvitationID"];
            }
            set
            {
                ViewState["InvitationID"] = value;
            }
        }

        public InvitationInfo Invitation
        {
            get
            {
                if (_Invitation == null)
                {
                    if (InvitationID > -1)
                    {
                        _Invitation = InvitationController.GetInvitation(InvitationID);
                    }
                }
                return _Invitation;
            }
            set
            {
                _Invitation = value;
                InvitationID = value == null ? -1 : value.InvitationID;
            }
        }
        
        //protected NameValueCollection ProfileProperties
        //{
        //    get
        //    {
        //        return ViewState["ProfileProperties"] == null ? new NameValueCollection() : (NameValueCollection)ViewState["ProfileProperties"];
        //    }
        //    set
        //    {
        //        ViewState["ProfileProperties"] = value;
        //    }
        //}

        private void InitializeParameters()
        {
            if (!String.IsNullOrEmpty(Request.QueryString["pid"]))
            {
                _PortalId = Convert.ToInt32(Request.QueryString["pid"]);
            }

            if (!String.IsNullOrEmpty(Request.QueryString["email"]))
            {
                _RecipientEmail = UrlUtils.DecodeParameter(Request.QueryString["email"]);
            }

            if (!String.IsNullOrEmpty(Request.QueryString["rsvpcode"]))
            {
                _RSVPCode = Request.QueryString["rsvpcode"];
            }

            if (!String.IsNullOrEmpty(Request.QueryString["action"]))
            {
                Action = Request.QueryString["action"].ToLowerInvariant();
            }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            var basePage = Page as PageBase;
            if (basePage != null)
            {
                ctlProfileEditor.LabelMode = basePage.PageCulture.TextInfo.IsRightToLeft ? LabelMode.Right : LabelMode.Left;
            }
            ctlProfileEditor.LocalResourceFile = Localization.SharedResourceFile;
            cmdAcceptInvitation.Click += new EventHandler(cmdAcceptInvitation_Click);
            cmdUpdateProfile.Click += new EventHandler(cmdUpdateProfile_Click);
            cmdDeclineInvitation.Click += new EventHandler(cmdDeclineInvitation_Click);
            cmdCancel.Click += new EventHandler(cmdCancel_Click);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                InitializeParameters();
                if ((_PortalId == -1 || _PortalId != PortalId) || _RecipientEmail == "" || _RSVPCode == "" || !validActions.Contains(Action))
                {
                    if (ModuleSecurity.IsAdministrator)
                    {
                        AddModuleMessage("AdminVisibleOnly", ModuleMessage.ModuleMessageType.RedError, true);
                    }
                    else
                    {
                        ContainerControl.Visible = false; // Hide module from non-administrators if query string parameters not present or PortalId is invalid
                    }
                }
                else if (MyConfiguration.SuspendInvitationProcessing)
                {
                    AddModuleMessage("ProcessingSuspended", ModuleMessage.ModuleMessageType.BlueInfo, true);
                }
                else
                {
                    Invitation = InvitationController.GetInvitationByPortalAndRSVPCode(_PortalId, _RSVPCode);

                    if (InvitationID == -1 || Invitation.RecipientEmail != _RecipientEmail)
                    {
                        AddModuleMessage("InvalidInvitationLink", ModuleMessage.ModuleMessageType.RedError, true);
                        return;
                    }

                    if (Invitation.LockOutHasExpired)
                    {
                        Invitation = InvitationController.UpdateInvitationStatus(InvitationID, "unlocked", -1);
                    }

                    if (Action != "decline" && (Invitation.HasExpired || Invitation.WasRetracted))
                    {
                        Utilities.AddTimedModuleMessage(this, LocalizeString("InvitationExpiredorRetracted"), ModuleMessage.ModuleMessageType.YellowWarning, Globals.NavigateURL(PortalSettings.HomeTabId));
                        return;
                    }

                    if (Invitation.WasAccepted)
                    {
                        Utilities.AddTimedModuleMessage(this, String.Format(LocalizeString("InvitationAlreadyAccepted"), Invitation.AcceptedOnDate.ToShortDateString()), ModuleMessage.ModuleMessageType.BlueInfo, Globals.NavigateURL(PortalSettings.HomeTabId));
                        return;
                    }

                    if (Invitation.WasDeclined)
                    {
                        Utilities.AddTimedModuleMessage(this, String.Format(LocalizeString("InvitationAlreadyDeclined"), Invitation.DeclinedOnDate.ToShortDateString()), ModuleMessage.ModuleMessageType.YellowWarning, Globals.NavigateURL(PortalSettings.HomeTabId));
                        return;
                    }

                    if (Invitation.IsLockedOut)
                    {
                        AddModuleMessage("InvitationLockedOut", ModuleMessage.ModuleMessageType.RedError, true);
                        return;
                    }

                    var tokenizer = new Tokenizer(Invitation);

                    if (Action == "decline")
                    {
                        divMessage.InnerHtml = tokenizer.ReplaceInvitationTokens(LocalizeString("DeclineConfirmation"));
                        divReasonDeclined.Visible = MyConfiguration.EnableReasonDeclinedField;
                        divActions.Visible = true;
                        cmdAcceptInvitation.Visible = false;
                    }
                    else if (Action == "join")
                    {
                        divMessage.InnerHtml = tokenizer.ReplaceInvitationTokens(LocalizeString("AcceptConfirmation"));
                        divCredentials.Visible = true;
                        divActions.Visible = true;
                        cmdDeclineInvitation.Visible = false;

                        if (ModuleSecurity.DisplayNameFormat == "")
                        {
                            divDisplayName.Visible = true;
                            tbDisplayName.Attributes.Add("AUTOCOMPLETE", "off");
                            val2DisplayName.ValidationExpression = ModuleSecurity.RegistrationExcludedTermsRegex;
                            val2DisplayName.Enabled = val2DisplayName.ValidationExpression != string.Empty;
                        }
                        else
                        {
                            divDisplayName.Visible = false;
                        }

                        var passwordRegex = MembershipProviderConfig.PasswordStrengthRegularExpression;
                        val2Password.ValidationExpression = string.IsNullOrEmpty(passwordRegex) ? ".*" : passwordRegex;

                        if (MyConfiguration.RequireTemporaryPasswordEntry && Invitation.TemporaryPassword != string.Empty)
                        {
                            divTemporaryPassword.Visible = true;
                            tbTemporaryPassword.Attributes.Add("AUTOCOMPLETE", "off");

                            plPassword.Text = Localization.GetString("NewPassword", LocalResourceFile);
                            plPassword.HelpText = Localization.GetString("NewPassword.Help", LocalResourceFile);
                            valPassword.Enabled = MyConfiguration.RequirePasswordChangeOnAcceptance;
                            plPassword.CssClass = valPassword.Enabled ? "dnnFormRequired" : "";

                        }
                        else
                        {
                            divTemporaryPassword.Visible = false;
                            plPassword.Text = Localization.GetString("Password", LocalResourceFile);
                            plPassword.HelpText = Localization.GetString("Password.Help", LocalResourceFile);
                            valPassword.Enabled = true;
                            plPassword.CssClass = "dnnFormRequired";
                        }

                        tbPassword.Attributes.Add("AUTOCOMPLETE", "off");

                        if (MyConfiguration.RequirePasswordConfirmOnChange)
                        {
                            divPassword2.Visible = true;
                            tbPassword2.Attributes.Add("AUTOCOMPLETE", "off");
                        }
                        else
                        {
                            divPassword2.Visible = false;
                        }

                        if (ModuleSecurity.UseEmailAsUsername)
                        {
                            if (string.IsNullOrEmpty(Invitation.AssignedUsername))
                            {
                                Invitation.AssignedUsername = Invitation.RecipientEmail;
                            }
                            val2Username.ValidationExpression = ModuleSecurity.EmailValidation;                         
                        }
                        else
                        {
                            val2Username.ValidationExpression = ModuleSecurity.UsernameValidation;
                        }

                        val2Username.Enabled = !string.IsNullOrEmpty(val2Username.ValidationExpression);
                        tbUsername.Text = Invitation.AssignedUsername;
                        tbUsername.Enabled = MyConfiguration.AllowUsernameModification || tbUsername.Text == "";

                        tbFirstName.Text = Invitation.RecipientFirstName;
                        tbFirstName.Enabled = MyConfiguration.AllowFirstLastNameModification || tbFirstName.Text == "";

                        tbLastName.Text = Invitation.RecipientLastName;
                        tbLastName.Enabled = MyConfiguration.AllowFirstLastNameModification || tbLastName.Text == "";

                        if (ProfileController.GetPropertyDefinitionByName(PortalId, UserProfile.USERPROFILE_FirstName).Required)
                        {
                            plFirstName.CssClass = "dnnFormRequired";
                            valFirstName.Enabled = true;
                        }
                        else
                        {
                            plFirstName.CssClass = "";
                            valFirstName.Enabled = false;
                        }

                        if (ProfileController.GetPropertyDefinitionByName(PortalId, UserProfile.USERPROFILE_LastName).Required)
                        {
                            plLastName.CssClass = "dnnFormRequired";
                            valLastName.Enabled = true;
                        }
                        else
                        {
                            plLastName.CssClass = "";
                            valLastName.Enabled = false;
                        }

                        tbDisplayName.Text = Invitation.AssignedDisplayName;
                        tbDisplayName.Enabled = MyConfiguration.AllowDisplayNameModification || tbDisplayName.Text == "";
                        valDisplayName.Enabled = tbDisplayName.Enabled;

                        var decryptedPassword = Security.DecryptString(Invitation.TemporaryPassword, Invitation.TemporaryPasswordSalt);
                        if (decryptedPassword == "")
                        {
                            valPassword.Enabled = true;
                            plPassword.CssClass = valPassword.Enabled ? "dnnFormRequired" : "";
                        }
                        else
                        {
                            if (!divTemporaryPassword.Visible && !MyConfiguration.RequirePasswordChangeOnAcceptance)
                            {
                                tbPassword.Text = decryptedPassword;
                                tbPassword2.Text = decryptedPassword;
                            }
                        }

                        divQuestionAndAnswer.Visible = MembershipProviderConfig.RequiresQuestionAndAnswer;

                        SetValidationGroup();

                        divPersistLogin.Visible = DotNetNuke.Entities.Host.Host.RememberCheckbox;

                        if (MyConfiguration.EnableAcceptanceCaptcha)
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
                    }
                }
            }
            else if (Action == "join")
            {
                tbTemporaryPassword.Attributes.Add("value", tbTemporaryPassword.Text);
                tbPassword.Attributes.Add("value", tbPassword.Text);
                tbPassword2.Attributes.Add("value", tbPassword2.Text);
            }

            // Below must be run on whether initial Page Load or Postback

            if (Action == "join")
            {
                if (ModuleSecurity.UseEmailAsUsername) plUsername.ResourceKey = "plEmail";
            }
            else if (Action == "profile")
            {
                BindProfileEditor();  //Viewstate disabled so needs to be rebound on each postback
            }
        }

        protected void cmdAcceptInvitation_Click(object sender, EventArgs e)
        {
            if (Page.IsValid && (!divCaptcha.Visible || ctlCaptcha.IsValid))
            {
                var portalSecurity = new PortalSecurity();
                var inputFilter = PortalSecurity.FilterFlag.NoMarkup | PortalSecurity.FilterFlag.NoScripting;

                User.Username = portalSecurity.InputFilter(tbUsername.Text, inputFilter);
                User.FirstName = portalSecurity.InputFilter(tbFirstName.Text, inputFilter);
                User.LastName = portalSecurity.InputFilter(tbLastName.Text, inputFilter);

                string displayName;
                if (ModuleSecurity.DisplayNameFormat == "")
                {
                    displayName = portalSecurity.InputFilter(tbDisplayName.Text, inputFilter);
                }
                else
                {
                    displayName = ModuleSecurity.DisplayNameFormat;
                    displayName = displayName.Replace("[FIRSTNAME]", User.FirstName);
                    displayName = displayName.Replace("[LASTNAME]", User.LastName);
                    displayName = displayName.Replace("[USERNAME]", User.Username);
                }
                User.DisplayName = displayName;

                User.Email = Invitation.RecipientEmail;

                if (MembershipProviderConfig.RequiresQuestionAndAnswer)
                {
                    User.Membership.PasswordQuestion = portalSecurity.InputFilter(tbQuestion.Text, inputFilter);
                    User.Membership.PasswordAnswer = portalSecurity.InputFilter(tbAnswer.Text, inputFilter);
                }

                var userCreateStatus = UserCreateStatus.AddUser;
                var rolesAdded = new StringBuilder();

                if (divTemporaryPassword.Visible)
                {
                    var temporaryPassword = Security.DecryptString(Invitation.TemporaryPassword, Invitation.TemporaryPasswordSalt);
                    if (temporaryPassword == tbTemporaryPassword.Text)
                    {
                        if (tbPassword.Text == "")
                        {
                            tbPassword.Text = tbTemporaryPassword.Text;
                            tbPassword2.Text = tbTemporaryPassword.Text;
                        }
                    }
                    else
                    {
                        Invitation = InvitationController.UpdateInvitationStatus(InvitationID, "pwdfailed", -1);

                        if (MyConfiguration.MaxFailedAttempts > 0 && (Invitation.FailedAttemptCount >= MyConfiguration.MaxFailedAttempts))
                        {
                            Invitation = InvitationController.UpdateInvitationStatus(InvitationID, "lockedout", -1, DateUtils.GetDatabaseTime() + MyConfiguration.LockoutDuration);
                            NotificationsHelper.SendNotifications(Invitation, Notifications.LockedOut);
                            divCredentials.Visible = false;
                            cmdAcceptInvitation.Visible = false;
                            cmdDeclineInvitation.Visible = false;
                            AddModuleMessage("InvalidTemporaryPassword-Lockout", ModuleMessage.ModuleMessageType.RedError, true);
                        }
                        else
                        {
                            AddModuleMessage("InvalidTemporaryPassword", ModuleMessage.ModuleMessageType.RedError, true);
                        }
                        return;
                    }
                }

                UserInfo user = null;

                if (!UserController.ValidatePassword(tbPassword.Text))
                {
                    userCreateStatus = UserCreateStatus.InvalidPassword;
                }
                else
                {
                    User.Membership.Password = tbPassword.Text;
                    User.Membership.Approved = true;        //TO DO: Add AutoApprove property to Configuration 
                    user = User;
                    userCreateStatus = UserController.CreateUser(ref user);
                    DataCache.ClearPortalCache(PortalId, true);

                    if (userCreateStatus == UserCreateStatus.Success)
                    {
                        var effectiveOnText = LocalizeString("EffectiveOn");

                        if (Invitation.AssignedRoles.Count > 0)
                        {
                            var currentSystemTime = SystemDateTime.GetCurrentTime();

                            rolesAdded.AppendLine("<ul class='RolesAddedList'>");
                            var roleController = new RoleController();
                            foreach (var assignableRole in Invitation.AssignedRoles)
                            {
                                var expiresOnText = LocalizeString("ExpiresOn");
                                DateTime effectiveDate = assignableRole.EffectiveDate.HasValue ? (DateTime)assignableRole.EffectiveDate : DateTime.MinValue;
                                DateTime expiryDate = assignableRole.ExpiryDate.HasValue ? (DateTime)assignableRole.ExpiryDate : DateTime.MinValue;

                                if (expiryDate == DateTime.MinValue || expiryDate >= currentSystemTime)
                                {
                                    roleController.AddUserRole(PortalId, User.UserID, assignableRole.RoleID, effectiveDate, expiryDate);
                                    rolesAdded.Append("  <li>");
                                    rolesAdded.Append(assignableRole.ToString());
                                    rolesAdded.AppendLine("</li>");
                                }
                            }
                            rolesAdded.AppendLine("</ul>");
                        }

                        Invitation = InvitationController.UpdateInvitationStatus(InvitationID, "accepted", user.UserID, "", user.UserID);
                        NotificationsHelper.SendNotifications(Invitation, Notifications.Accepted);
                    }
                }

                if (userCreateStatus == UserCreateStatus.Success)
                {
                    ShowOrHideForm(false);

                    var successMsg = LocalizeString("InvitationAcceptedSuccess");
                    if (Invitation.AssignedRoles.Count > 0) successMsg += String.Format(LocalizeString("RolesAdded"), rolesAdded);


                    UserLoginStatus loginStatus = UserLoginStatus.LOGIN_FAILURE;
                    UserController.UserLogin(PortalId, user.Username, user.Membership.Password, "", PortalSettings.PortalName,
                                                AuthenticationLoginBase.GetIPAddress(), ref loginStatus, cbPersistLogin.Checked);
                    Localization.SetLanguage(user.Profile.PreferredLocale);

                    if (ModuleSecurity.RequireValidProfile && !ProfileController.ValidateProfile(PortalId, user.Profile))
                    {
                        //var addParams = new string[] { string.Format("userid={0}", user.UserID), string.Format("returnurl={0}", UrlUtils.EncodeParameter(redirectUrl))};
                        //var profileUrl = Globals.NavigateURL(PortalSettings.UserTabId, "", addParams);
                        Action = "profile";
                        UserId = user.UserID;
                        portalSecurity.SignOut();
                        divCredentials.Visible = false;
                        divCaptcha.Visible = false;
                        divActions.Visible = false;
                        
                        BindProfileEditor();


                        if (UrlUtils.InPopUp())
                        {
                            ScriptManager.RegisterClientScriptBlock(this, GetType(), "ResizePopup", "if(parent.$('#iPopUp').length > 0 && parent.$('#iPopUp').dialog('isOpen')){parent.$('#iPopUp').dialog({width: 950, height: 550}).dialog({position: 'center'});};", true);
                        }

                        Utilities.AddTimedModuleMessage(this, successMsg + "<br />" + LocalizeString("ValidProfileRequired"), ModuleMessage.ModuleMessageType.YellowWarning, null);
                    }
                    else
                    {
                        Utilities.AddTimedModuleMessage(this, successMsg, ModuleMessage.ModuleMessageType.GreenSuccess, GetRedirectUrl());
                    }
                }
                else
                {
                    AddLocalizedModuleMessage(UserController.GetUserCreateStatus(userCreateStatus), ModuleMessage.ModuleMessageType.RedError, true);
                    ShowOrHideForm(true);
                    if (userCreateStatus == UserCreateStatus.UserAlreadyRegistered
                            || userCreateStatus == UserCreateStatus.UsernameAlreadyExists
                            || userCreateStatus == UserCreateStatus.InvalidUserName)
                    {
                        tbUsername.Enabled = true;
                        tbUsername.Visible = true;
                        valUsername.Enabled = true;
                        val2Username.Enabled = true;
                        tbUsername.Text = string.Empty;
                    }
                    else if ((ModuleSecurity.RequireUniqueDisplayName && userCreateStatus == UserCreateStatus.DuplicateDisplayName) || userCreateStatus == UserCreateStatus.InvalidDisplayName)
                    {
                        tbDisplayName.Enabled = true;
                        tbDisplayName.Visible = true;
                        valDisplayName.Enabled = true;
                        val2DisplayName.Enabled = true;
                        tbDisplayName.Text = string.Empty;
                    }
                }
            }
        }

        protected void cmdDeclineInvitation_Click(object sender, EventArgs e)
        {
            var portalSecurity = new PortalSecurity();
            var inputFilter = PortalSecurity.FilterFlag.NoMarkup | PortalSecurity.FilterFlag.NoScripting;
            var reasonDeclined = portalSecurity.InputFilter(txtReasonDeclined.Text, inputFilter);
            AddModuleMessage("InvitationDeclined", ModuleMessage.ModuleMessageType.GreenSuccess, true);
            Invitation = InvitationController.UpdateInvitationStatus(InvitationID, "declined", -1, reasonDeclined, -1);
            NotificationsHelper.SendNotifications(Invitation, Notifications.Declined);
            ShowOrHideForm(false);
        }

        protected void cmdCancel_Click(object sender, EventArgs e)
        {
            Utilities.AddTimedModuleMessage(this, string.Format(LocalizeString("InvitationProcessingPostponed"),
                                                     Utilities.FormattedUTCDate(Invitation.ExpiresOnDate, null)),
                                                     ModuleMessage.ModuleMessageType.BlueInfo, Globals.NavigateURL(PortalSettings.HomeTabId));
            ShowOrHideForm(false);
        }

        protected void cmdUpdateProfile_Click(object sender, EventArgs e)
        {
            if (ctlProfileEditor.IsValid)
            {
                var properties = (ProfilePropertyDefinitionCollection)ctlProfileEditor.DataSource;

                //Update User's profile
                User = ProfileController.UpdateUserProfile(User, properties);
                if (!ProfileController.ValidateProfile(PortalId, UserProfile))
                {
                    Utilities.AddTimedModuleMessage(this, LocalizeString("ValidProfileRequired"), ModuleMessage.ModuleMessageType.YellowWarning, null);
                }
                else
                {
                    UserController.UserLogin(PortalId, User, PortalSettings.PortalName, AuthenticationLoginBase.GetIPAddress(), cbPersistLogin.Checked);
                    Utilities.AddTimedModuleMessage(this, LocalizeString("ProfileUpdated"), ModuleMessage.ModuleMessageType.GreenSuccess, GetRedirectUrl());
                }
            }
        }

        private void ShowOrHideForm(bool show)
        {
            divMessage.Visible = show;
            divReasonDeclined.Visible = show;
            divCredentials.Visible = show;
            divCaptcha.Visible = show;
            divActions.Visible = show;
        }

        private void SetValidationGroup()
        {
            valUsername.ValidationGroup = Consts.ValidationGroup;
            val2Username.ValidationGroup = Consts.ValidationGroup;
            valFirstName.ValidationGroup = Consts.ValidationGroup;
            valLastName.ValidationGroup = Consts.ValidationGroup;
            valDisplayName.ValidationGroup = Consts.ValidationGroup;
            val2DisplayName.ValidationGroup = Consts.ValidationGroup;
            valTemporaryPassword.ValidationGroup = Consts.ValidationGroup;
            valPassword.ValidationGroup = Consts.ValidationGroup;
            val2Password.ValidationGroup = Consts.ValidationGroup;
            valPassword2.ValidationGroup = Consts.ValidationGroup;
            cmdAcceptInvitation.ValidationGroup = Consts.ValidationGroup;
        }

        private void BindProfileEditor()
        {

            //Before we bind the Profile to the editor we need to "update" the visible data
            var properties = new ProfilePropertyDefinitionCollection();
            var imageType = new DotNetNuke.Common.Lists.ListController().GetListEntryInfo("DataType", "Image");
            foreach (ProfilePropertyDefinition profProperty in UserProfile.ProfileProperties)
            {
                if (!profProperty.Deleted && (Request.IsAuthenticated || profProperty.DataType != imageType.EntryID))
                {
                    if (profProperty.Required) properties.Add(profProperty);
                }
            }

            divProfile.Visible = true;
            ctlProfileEditor.User = User;
            ctlProfileEditor.ShowVisibility = ModuleSecurity.ProfileDisplayVisability;
            ctlProfileEditor.DataSource = properties;
            ctlProfileEditor.DataBind();
        }

        private string GetRedirectUrl()
        {
            var redirectTabId = -1;
            if (Invitation.RedirectOnFirstLogin > 0)
            {
                redirectTabId = Invitation.RedirectOnFirstLogin;
            }
            else if (ModuleSecurity.RedirectAfterRegistration > 0)
            {
                redirectTabId = ModuleSecurity.RedirectAfterRegistration;
            }
            else if (ModuleSecurity.RedirectAfterFirstLogin > 0)
            {
                redirectTabId = ModuleSecurity.RedirectAfterFirstLogin;
            }

            return redirectTabId > 0 ? Globals.NavigateURL(redirectTabId) : "";
        }
    }
}