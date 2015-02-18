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
using System.Text.RegularExpressions;
using System.Web.UI;
using System.Web.UI.WebControls;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Security;
using DotNetNuke.Security.Roles;
using DotNetNuke.Services.FileSystem;
using DotNetNuke.Services.Localization;
using DotNetNuke.UI.Skins;
using Telerik.Web.UI;


namespace WESNet.DNN.Modules.ByInvitation
{
    public partial class Settings : ModuleSettingsBase
    {

        private Configuration _MyConfiguration = null;
        private LocalizedConfiguration _MyLocalizedConfiguration = null;
        private Security _ModuleSecurity = null;

        

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
                return ctlCultureCode.SelectedValue;
            }
            set
            {
                ctlCultureCode.SetLanguage(value);
            }
        }

        protected override void OnInit(EventArgs e)
        {
 	         base.OnInit(e);
             ctlCultureCode.ItemChanged +=new EventHandler(ctlCultureCode_ItemChanged);
             btnDefaultEmailRegex.Click += new EventHandler(btnDefaultEmailRegex_Click);
        }

        public override void LoadSettings()
        {
            ctlCultureCode.PortalId = PortalId;
            
            if (!Page.IsPostBack)
            {
                ctlCultureCode.DataBind();
                ctlCultureCode.SetLanguage(PortalSettings.CultureCode);
                BindLocalizedConfiguration();
                BindConfiguration();
                NotificationsHelper.AddNotificationTypes();
            }
        }

        private void BindLocalizedConfiguration()
        {
            teSendInvitationHtml.Text = Utilities.FormatHtmlContent(MyLocalizedConfiguration.SendInvitationHtml);
            tbSendInvitationButtonText.Text = MyLocalizedConfiguration.SendInvitationButtonText;
            ctlSendInvitationButtonImage.FilePath = Utilities.ResolveImageUrl(PortalId, MyLocalizedConfiguration.SendInvitationButtonImage);
            ctlSendInvitationButtonImage.FileFilter = DotNetNuke.Common.Globals.glbImageFileTypes;
            if (MyLocalizedConfiguration.ImageButtonSize.IsEmpty)
            {
                tbImageButtonWidth.Text = "";
                tbImageButtonHeight.Text = "";
            }
            else
            {
                tbImageButtonWidth.Value = MyLocalizedConfiguration.ImageButtonSize.Width;
                tbImageButtonHeight.Value = MyLocalizedConfiguration.ImageButtonSize.Height;
            }
            tbInvitationSubject.Text = MyLocalizedConfiguration.InvitationSubject;
            teInvitationBody.Text = Utilities.FormatHtmlContent(MyLocalizedConfiguration.InvitationBody);
            tbEmailRegex.Text = MyLocalizedConfiguration.EmailRegex;
        }

        private void BindConfiguration()
        {
            // General Invitation Settings

            tpValidityPeriod.Value = MyConfiguration.ValidityPeriod;
            cbEnableAutoResend.Checked = MyConfiguration.EnableAutoResend;
            tbMaxResends.Value = MyConfiguration.MaxResends;
            tpResendInterval.Value = MyConfiguration.ResendInterval;
            rblAutoDeleteArchiveExpiredInvitations.SelectedValue = MyConfiguration.AutoDeleteArchiveExpiredInvitations.ToString("d");
            tbDaysPastExpiration.Text = MyConfiguration.DaysPastExpiration.ToString();

            // Invitation Form Settings

            cbRequireRecipientEmailConfirm.Checked = MyConfiguration.RequireRecipientEmailConfirm;
            cbEnablePersonalNote.Checked = MyConfiguration.EnablePersonalNote;
            rblTemporaryPasswordMode.SelectedValue = MyConfiguration.TemporaryPasswordMode.ToString("d");
            if (MyConfiguration.TemporaryPasswordMode <= TemporaryPasswordMode.AutoGenerateTemporaryPassword)
            {
                MyConfiguration.RequireTemporaryPasswordConfirm = false;
            }
            cbRequireTemporaryPasswordConfirm.Checked = MyConfiguration.RequireTemporaryPasswordConfirm;
            if (MyConfiguration.TemporaryPasswordMode == TemporaryPasswordMode.NoTemporaryPassword)
            {
                MyConfiguration.RequirePasswordChangeOnAcceptance = false;
            }

            //Security Settings

            rblEnableInvitationCaptcha.SelectedValue = MyConfiguration.EnableInvitationCaptcha.ToString("d");
            cbEnableAcceptanceCaptcha.Checked = MyConfiguration.EnableAcceptanceCaptcha;
            cbEnableCaptchaAudio.Checked = MyConfiguration.EnableCaptchaAudio;
            cbCaptchaIsCaseInsensitive.Checked = MyConfiguration.CaptchaIsCaseInsensitive;
            ddlCaptchaLineNoise.SelectedValue = MyConfiguration.CaptchaLineNoise.ToString("d");
            ddlCaptchaBackgroundNoise.SelectedValue = MyConfiguration.CaptchaBackgroundNoise.ToString("d");
            ddlInvitationFiltering.SelectedValue = MyConfiguration.InvitationFiltering.ToString("d");
            tpInvitationFilteringInterval.Value = MyConfiguration.InvitationFilteringInterval;
            tbMaxFailedAttempts.Value = MyConfiguration.MaxFailedAttempts;
            tpLockoutDuration.Value = MyConfiguration.LockoutDuration;
            cbRequireModeration.Checked = MyConfiguration.RequireModeration;

            //Invitation Processing Settings

            tbProcessorPageName.Text = MyConfiguration.ProcessorPageName;
            cbAllowUsernameModification.Checked = MyConfiguration.AllowUsernameModification;
            cbAllowFirstLastNameModification.Checked = MyConfiguration.AllowFirstLastNameModification;
            cbAllowDisplayNameModification.Checked = MyConfiguration.AllowDisplayNameModification;
            cbRequireTemporaryPasswordEntry.Checked = MyConfiguration.RequireTemporaryPasswordEntry;
            cbRequirePasswordChangeOnAcceptance.Checked = MyConfiguration.RequirePasswordChangeOnAcceptance;
            cbRequirePasswordConfirmOnChange.Checked = MyConfiguration.RequirePasswordConfirmOnChange;
            cbEnableReasonDeclinedField.Checked = MyConfiguration.EnableReasonDeclinedField;
            cbSuspendInvitationProcessing.Checked = MyConfiguration.SuspendInvitationProcessing;

            //Notification Settings

            tbSendFrom.Text = MyConfiguration.SendFrom;
            tbSendCopiesTo.Text = MyConfiguration.SendCopiesTo;
            BindSendCopyToRoles();
            BindEnabledNotificationsCBL(cblEnabledInvitingUserNotifications, MyConfiguration.EnabledInvitingUserNotifications);
            BindEnabledNotificationsCBL(cblEnabledAdminUserNotifications, MyConfiguration.EnabledAdminUserNotifications);

        }

        private void BindEnabledNotificationsCBL(CheckBoxList cbl,  Notifications enabledNotifications)
        {
            ListItem li;
            string key;
            Notifications[] values = (Notifications[])Enum.GetValues(typeof (Notifications));
            cbl.Items.Clear();
            foreach (var value in values)
            {
                key = value.ToString();
                if (!(key == "None" || key == "Create" || key == "All"))
                {
                    li = new ListItem(Localization.GetString(key, LocalResourceFile), value.ToString("d"));
                    li.Selected = (enabledNotifications & value) != 0;
                    cbl.Items.Add(li);
                }
            }
        }

        private Notifications LoadEnabledNotificationsCBL (CheckBoxList cbl)
        {
            int enabledNotifications = 0;
            foreach (ListItem li in cbl.Items)
            {
                if (li.Selected)
                {
                    enabledNotifications |= Convert.ToInt32(li.Value);
                }
            }
            return (Notifications)enabledNotifications;
        }

        private void BindSendCopyToRoles()
        {
            var rc = new RoleController();
            foreach(string roleId in DotNetNuke.Common.Utilities.EscapedString.Seperate(MyConfiguration.SendCopiesToRoles, ';'))
            {
                var roleInfo = rc.GetRoleById(PortalId, Convert.ToInt32(roleId));
                if (roleInfo != null)
                {
                    dgSelectedRoles.SelectedRoleNames.Add(roleInfo.RoleName);
                }
            }
        }

        private bool IsValidTabName(string tabName)
        {
            if (string.IsNullOrEmpty(tabName.Trim()))
            {
                Skin.AddModuleMessage(this, Localization.GetString("EmptyProcessorPageName", LocalResourceFile), DotNetNuke.UI.Skins.Controls.ModuleMessage.ModuleMessageType.RedError);
                return false;
            }
            
            if (Regex.IsMatch(tabName, "^AUX$|^CON$|^LPT[1-9]$|^CON$|^COM[1-9]$|^NUL$|^SITEMAP$|^LINKCLICK$|^KEEPALIVE$|^DEFAULT$|^ERRORPAGE$", RegexOptions.IgnoreCase))
            {
                Skin.AddModuleMessage(this, string.Format(Localization.GetString("InvalidProcessorPageName", LocalResourceFile), tabName), DotNetNuke.UI.Skins.Controls.ModuleMessage.ModuleMessageType.RedError);
                return false;
            }

            //var tabController = new TabController();
            //var processingPage = tabController.GetTabByName(tabName, PortalId);
            //if (processingPage != null)
            //{
            //     var moduleController = new ModuleController();
            //     var tabModules = moduleController.GetTabModules(processingPage.TabID);
            //     var processingModuleDefinition = Utilities.GetModuleDefinition("WESNet_ByInvitation_Processor", "By Invitation Processor");
            //     if (processingModuleDefinition != null)
            //     {
            //         return tabModules.Values.Where(m => m.ModuleDefID == processingModuleDefinition.ModuleDefID).Count() > 0
            //     }
            //} 
 
            return true;
        }

        protected void ctlCultureCode_ItemChanged (object sender, EventArgs e)
        {
            _MyLocalizedConfiguration = null;
            BindLocalizedConfiguration();
        }

        protected void btnDefaultEmailRegex_Click (object sender, EventArgs e)
        {
            tbEmailRegex.Text = MyLocalizedConfiguration.DefaultEmailRegex;
        }

        public override void UpdateSettings()
        {
            var ps = new PortalSecurity();
            var filterFlags = PortalSecurity.FilterFlag.NoMarkup | PortalSecurity.FilterFlag.NoScripting;
            var processorPageName = ps.InputFilter(tbProcessorPageName.Text, filterFlags);

            if (Page.IsValid && IsValidTabName(processorPageName))
            {
                var portalAliasController = new PortalAliasController();
                var aliases = from PortalAliasInfo pai in PortalAliasController.Instance.GetPortalAliasesByPortalId(PortalSettings.PortalId) select pai.HTTPAlias;
                                
                MyLocalizedConfiguration.CultureCode = CultureCode;
                MyLocalizedConfiguration.SendInvitationHtml = DotNetNuke.Common.Utilities.HtmlUtils.AbsoluteToRelativeUrls(teSendInvitationHtml.Text, aliases);
                MyLocalizedConfiguration.SendInvitationButtonText = ps.InputFilter(tbSendInvitationButtonText.Text, filterFlags);

                MyLocalizedConfiguration.SendInvitationButtonImage = string.Format("fileid={0}", ctlSendInvitationButtonImage.FileID);
                if (ctlSendInvitationButtonImage.FileID != -1)
                {
                    int width = 0;
                    int height = 0;

                    if (tbImageButtonWidth.Value > 0 && tbImageButtonHeight.Value > 0)
                    {
                        width = (int)tbImageButtonWidth.Value;
                        height = (int)tbImageButtonHeight.Value;
                    }
                    else
                    {
                        var fileInfo = FileManager.Instance.GetFile(ctlSendInvitationButtonImage.FileID);
                        if (fileInfo != null)
                        {
                            width = fileInfo.Width;
                            height = fileInfo.Height;
                        }
                    }
                    MyLocalizedConfiguration.ImageButtonSize = new System.Drawing.Size(width, height);
                }
                else
                {
                    MyLocalizedConfiguration.ImageButtonSize = System.Drawing.Size.Empty;
                }

                MyLocalizedConfiguration.InvitationSubject = ps.InputFilter(tbInvitationSubject.Text, filterFlags);
                MyLocalizedConfiguration.InvitationBody = teInvitationBody.Text;
                MyLocalizedConfiguration.EmailRegex = tbEmailRegex.Text;
                                
                // General Invitation Settings

                MyConfiguration.ValidityPeriod = tpValidityPeriod.Value;
                MyConfiguration.EnableAutoResend = cbEnableAutoResend.Checked;
                MyConfiguration.MaxResends = (int)tbMaxResends.Value;
                MyConfiguration.ResendInterval = tpResendInterval.Value;
                MyConfiguration.AutoDeleteArchiveExpiredInvitations = (ExpiredInvitationActions)Convert.ToInt32(rblAutoDeleteArchiveExpiredInvitations.SelectedValue);
                MyConfiguration.DaysPastExpiration = (int)tbDaysPastExpiration.Value;

                // Invitation Form Settings
                MyConfiguration.RequireRecipientEmailConfirm = cbRequireRecipientEmailConfirm.Checked;
                MyConfiguration.EnablePersonalNote = cbEnablePersonalNote.Checked;

                MyConfiguration.TemporaryPasswordMode = (TemporaryPasswordMode)Convert.ToInt32(rblTemporaryPasswordMode.SelectedValue);
                if (MyConfiguration.TemporaryPasswordMode <= TemporaryPasswordMode.AutoGenerateTemporaryPassword)
                {
                    MyConfiguration.RequireTemporaryPasswordConfirm = false;
                }
                else
                {
                    MyConfiguration.RequireTemporaryPasswordConfirm = cbRequireTemporaryPasswordConfirm.Checked;
                }
                
                // Security Settings

                MyConfiguration.EnableInvitationCaptcha = (CaptchaUsage)Convert.ToInt32(rblEnableInvitationCaptcha.SelectedValue);
                MyConfiguration.EnableAcceptanceCaptcha = cbEnableAcceptanceCaptcha.Checked;
                MyConfiguration.EnableCaptchaAudio = cbEnableCaptchaAudio.Checked;
                MyConfiguration.CaptchaIsCaseInsensitive = cbCaptchaIsCaseInsensitive.Checked;
                MyConfiguration.CaptchaLineNoise = (CaptchaLineNoiseLevel)Convert.ToInt32(ddlCaptchaLineNoise.SelectedValue);
                MyConfiguration.CaptchaBackgroundNoise = (CaptchaBackgroundNoiseLevel)Convert.ToInt32(ddlCaptchaBackgroundNoise.SelectedValue);
                MyConfiguration.InvitationFiltering = (InvitationFilter)Convert.ToInt32(ddlInvitationFiltering.SelectedValue);
                MyConfiguration.InvitationFilteringInterval = tpInvitationFilteringInterval.Value;
                MyConfiguration.MaxFailedAttempts = (int)tbMaxFailedAttempts.Value;
                MyConfiguration.LockoutDuration = tpLockoutDuration.Value;
                MyConfiguration.RequireModeration = cbRequireModeration.Checked;

                // Processing Form Settings

                MyConfiguration.ProcessorPageName = processorPageName;
                MyConfiguration.AllowUsernameModification = cbAllowUsernameModification.Checked;
                MyConfiguration.AllowFirstLastNameModification = cbAllowFirstLastNameModification.Checked;
                MyConfiguration.AllowDisplayNameModification = cbAllowDisplayNameModification.Checked;
                MyConfiguration.RequireTemporaryPasswordEntry = cbRequireTemporaryPasswordEntry.Checked;
                MyConfiguration.RequirePasswordChangeOnAcceptance = cbRequirePasswordChangeOnAcceptance.Checked;
                MyConfiguration.RequirePasswordConfirmOnChange = cbRequirePasswordConfirmOnChange.Checked;
                MyConfiguration.EnableReasonDeclinedField = cbEnableReasonDeclinedField.Checked;
                MyConfiguration.SuspendInvitationProcessing = cbSuspendInvitationProcessing.Checked;

                //Notification Settings

                MyConfiguration.SendFrom = tbSendFrom.Text;
                MyConfiguration.SendCopiesTo = tbSendCopiesTo.Text;

                var rc = new RoleController();
                var sb = new StringBuilder();
                foreach (string roleName in dgSelectedRoles.SelectedRoleNames)
                {
                    var roleInfo = rc.GetRoleByName(PortalId, roleName);
                    if (roleInfo != null)
                    {
                        sb.Append(roleInfo.RoleID.ToString());
                        sb.Append(";");
                    }
                }
                if (sb.Length > 0) sb.Length--; // remove trailing semi-colon
                MyConfiguration.SendCopiesToRoles = sb.ToString();

                MyConfiguration.EnabledInvitingUserNotifications = LoadEnabledNotificationsCBL(cblEnabledInvitingUserNotifications);

                MyConfiguration.EnabledAdminUserNotifications = LoadEnabledNotificationsCBL(cblEnabledAdminUserNotifications);

                InvitationController.UpdateLocalizedConfiguration(MyLocalizedConfiguration, UserId);
                InvitationController.UpdateConfiguration(MyConfiguration, UserId);

                _MyConfiguration = null;
                _MyLocalizedConfiguration = null;           
            }
        }
    }
}