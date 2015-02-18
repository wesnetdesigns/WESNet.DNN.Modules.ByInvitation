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
using System.Web;
using System.Linq;

using DotNetNuke;
using DotNetNuke.Entities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Tabs;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Users;
using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Security.Membership;
using DotNetNuke.Security.Permissions;
using DotNetNuke.Services.Localization;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Tokens;

namespace WESNet.DNN.Modules.ByInvitation 
{
    public class Configuration : BaseEntityInfo, IHydratable, IPropertyAccess
    {

        private Dictionary<string, LocalizedConfiguration> _LocalizedConfigurations;
                
        public Configuration()
        {
            PortalId = -1;
            SendFrom = string.Empty;
            SendCopiesTo = string.Empty;
            SendCopiesToRoles = string.Empty;
            EnabledInvitingUserNotifications = Notifications.Created | Notifications.Accepted | Notifications.Declined;
            EnabledAdminUserNotifications = EnabledInvitingUserNotifications | Notifications.Declined | Notifications.Retracted;

            EnableInvitationCaptcha = CaptchaUsage.AllUsers;
            EnableCaptchaAudio = true;
            CaptchaIsCaseInsensitive = false;
            CaptchaLineNoise = Telerik.Web.UI.CaptchaLineNoiseLevel.Low;
            CaptchaBackgroundNoise = Telerik.Web.UI.CaptchaBackgroundNoiseLevel.Low;

            InvitationFiltering = InvitationFilter.NoFiltering;
            InvitationFilteringInterval = TimeSpan.Zero;
            MaxFailedAttempts = 5;
            LockoutDuration = new TimeSpan(0, 10, 0); // 10 minutes
            EnableAutoResend = false;
            MaxResends = 0;
            ResendInterval = TimeSpan.Zero;
            AutoDeleteArchiveExpiredInvitations = ExpiredInvitationActions.None;
            DaysPastExpiration = 0;
            ValidityPeriod = new TimeSpan(7, 0, 0, 0);        // 7 days
            RequireRecipientEmailConfirm = true;
            EnablePersonalNote = true;
            TemporaryPasswordMode = ByInvitation.TemporaryPasswordMode.AutoGenerateTemporaryPassword;
            RequireTemporaryPasswordConfirm = false;
            RequirePasswordChangeOnAcceptance = true;
            ProcessorPageName = "By Invitation Processor";
            RequireModeration = true;
        }
                
        public Configuration(int portalId) : this()
        {
            PortalId = portalId;    
        }

        public static Configuration GetConfiguration(int portalId)
        {
            return InvitationController.GetConfiguration(portalId);
        }

        public int PortalId { get; private set; }

        // General Invitation Settings

        public TimeSpan ValidityPeriod { get; set; }

        public bool EnableAutoResend { get; set; }

        public int MaxResends { get; set; }

        public TimeSpan ResendInterval { get; set;}

        public ExpiredInvitationActions AutoDeleteArchiveExpiredInvitations { get; set; }

        public int DaysPastExpiration { get; set; }

        // Invitation Form Settings

        public bool RequireRecipientEmailConfirm { get; set; }

        public bool EnablePersonalNote { get; set; }

        public TemporaryPasswordMode TemporaryPasswordMode { get; set; }

        public bool RequireTemporaryPasswordConfirm { get; set; }

        // Security Settings

        public CaptchaUsage EnableInvitationCaptcha { get; set; }

        public bool EnableAcceptanceCaptcha { get; set; }

        public bool EnableCaptchaAudio { get; set; }

        public bool CaptchaIsCaseInsensitive { get; set; }

        public Telerik.Web.UI.CaptchaLineNoiseLevel CaptchaLineNoise { get; set; }

        public Telerik.Web.UI.CaptchaBackgroundNoiseLevel CaptchaBackgroundNoise { get; set; }

        public InvitationFilter InvitationFiltering { get; set; }

        public TimeSpan InvitationFilteringInterval { get; set; }

        public int MaxFailedAttempts { get; set; }

        public TimeSpan LockoutDuration { get; set; }

        public bool RequireModeration { get; set; }

        // Invitation Processing Settings

        public string ProcessorPageName { get; set; }

        public bool AllowUsernameModification { get; set; }

        public bool AllowFirstLastNameModification { get; set; }

        public bool AllowDisplayNameModification { get; set; }

        public bool RequireTemporaryPasswordEntry { get; set; }

        public bool RequirePasswordChangeOnAcceptance { get; set; }

        public bool RequirePasswordConfirmOnChange { get; set; }

        public bool EnableReasonDeclinedField { get; set; }

        public bool SuspendInvitationProcessing { get; set; }


        // Notification Settings

        public string SendFrom { get; set; }

        public string SendCopiesTo { get; set; }

        public string SendCopiesToRoles { get; set; }

        public Notifications EnabledInvitingUserNotifications { get; set; }

        public Notifications EnabledAdminUserNotifications { get; set; }

        public static string ModuleName
        {
            get
            {
                return "WESNet_ByInvitation";
            }
        }

        public static string FriendlyName
        {
            get
            {
                return "By Invitation";
            }
        }

        public static string ModulePath
        {
            get
            {
                return "DesktopModules/" + ModuleName + "/";
            }
        }

        public static string LocalSharedResourceFile
        {
            get
            {
                return ModulePath + Localization.LocalResourceDirectory + "/" + Localization.LocalSharedResourceFile;
            }
        }

        public int GetProcessorPageTabID()
        {
            var tabController = new TabController();
            var processorPage = tabController.GetTabByName(ProcessorPageName, PortalId);
            if (processorPage != null)
            {
                return processorPage.TabID;
            }
            else
            {
                return Null.NullInteger;
            }
        }

        public Dictionary<string, LocalizedConfiguration> LocalizedConfigurations
        {
            get
            {
                if (_LocalizedConfigurations == null)
                {
                    _LocalizedConfigurations = InvitationController.GetLocalizedConfigurations(PortalId);
                }
                return _LocalizedConfigurations;
            }
        }

        public LocalizedConfiguration GetLocalizedConfiguration(string cultureCode, string fallbackCultureCode)
        {
            LocalizedConfiguration localizedConfiguration = null;
            LocalizedConfigurations.TryGetValue(cultureCode, out localizedConfiguration);
            if (localizedConfiguration == null)
            {
                if (fallbackCultureCode != string.Empty && cultureCode != fallbackCultureCode)
                {
                    return GetLocalizedConfiguration(fallbackCultureCode, "");
                }
                else
                {
                    return new LocalizedConfiguration(PortalId, cultureCode);
                }
            }
            else
            {
                return localizedConfiguration;
            }
        }

        public int VerifyOrAddInvitationProcessor()
        {
            TabInfo processorTab = null;

            // Get the Invitation Processor ModuleDefID
            
            var moduleDefinition = Utilities.GetModuleDefinition("WESNet_ByInvitation_Processor", "By Invitation Processor");

            if (moduleDefinition == null)
            {
                var dnnException = new ModuleLoadException("Unable to find WESNet_ByInvitation_Processor module definition.");
                Exceptions.LogException(dnnException);
                return -1;
            }
            else
            {
                ArrayList processorModules = ModuleController.Instance.GetModulesByDefinition(PortalId, moduleDefinition.FriendlyName);
                if (processorModules.Count > 0)
                {
                    ModuleInfo moduleInfo = (ModuleInfo)processorModules[0];
                    IDictionary<int, TabInfo> tabDictionary = TabController.Instance.GetTabsByModuleID(moduleInfo.ModuleID);
                    if (tabDictionary.Count > 0)
                    {
                        processorTab = tabDictionary.Values.First();
                        if (processorTab.TabName != ProcessorPageName)
                        {
                            processorTab.TabName = ProcessorPageName;
                            TabController.Instance.UpdateTab(processorTab);
                        }
                        return processorTab.TabID;
                    }
                }
                
                // Processor Tab does not exist so add the By Invitation Processing Page
                var administratorRoleId = new PortalSettings(-1, PortalId).AdministratorRoleId;
                var tabPermissions = new TabPermissionCollection();
                Utilities.AddPagePermission(tabPermissions, "View", Convert.ToInt32(Globals.glbRoleUnauthUser));
                Utilities.AddPagePermission(tabPermissions, "View", administratorRoleId);
                Utilities.AddPagePermission(tabPermissions, "Edit", administratorRoleId);
                processorTab = Utilities.AddPage(PortalId, Null.NullInteger, ProcessorPageName, "", "", "", false, tabPermissions, false);

                // Add the Invitation Processing Module to the Page
                var moduleId = Utilities.AddModuleToPage(processorTab, moduleDefinition.ModuleDefID, "Accept or Decline Invitation", "", true);
                return processorTab.TabID;

            }
        }

        #region IHydratable Members

        public void Fill(System.Data.IDataReader dr)
        {
            base.FillInternal(dr);
            PortalId = Null.SetNullInteger(dr["PortalId"]);
            
            //Invitation General Settings

            ValidityPeriod = Utilities.SetNullTimeSpan(dr["ValidityPeriod"]);
            EnableAutoResend = Null.SetNullBoolean(dr["EnableAutoResend"]);
            MaxResends = Null.SetNullInteger(dr["MaxResends"]);
            ResendInterval = Utilities.SetNullTimeSpan(dr["ResendInterval"]);
            AutoDeleteArchiveExpiredInvitations = (ExpiredInvitationActions)(Null.SetNullInteger(dr["AutoDeleteArchiveExpiredInvitations"]));
            DaysPastExpiration = Null.SetNullInteger(dr["DaysPastExpiration"]);

            // Invitation Form Settings

            RequireRecipientEmailConfirm = Null.SetNullBoolean(dr["RequireRecipientEmailConfirm"]);
            EnablePersonalNote = Null.SetNullBoolean(dr["EnablePersonalNote"]);
            TemporaryPasswordMode = (TemporaryPasswordMode)Null.SetNullInteger(dr["TemporaryPasswordMode"]);
            RequireTemporaryPasswordConfirm = Null.SetNullBoolean(dr["RequireTemporaryPasswordConfirm"]);

            // Security Settings

            EnableInvitationCaptcha = (CaptchaUsage)Null.SetNullInteger(dr["EnableInvitationCaptcha"]);
            EnableAcceptanceCaptcha = Null.SetNullBoolean(dr["EnableAcceptanceCaptcha"]);
            EnableCaptchaAudio = Null.SetNullBoolean(dr["EnableCaptchaAudio"]);
            CaptchaIsCaseInsensitive = Null.SetNullBoolean(dr["CaptchaIsCaseInsensitive"]);
            CaptchaLineNoise = (Telerik.Web.UI.CaptchaLineNoiseLevel)Null.SetNullInteger(dr["CaptchaLineNoise"]);
            CaptchaBackgroundNoise = (Telerik.Web.UI.CaptchaBackgroundNoiseLevel)Null.SetNullInteger(dr["CaptchaBackgroundNoise"]);
            InvitationFiltering = (InvitationFilter)Null.SetNullInteger(dr["InvitationFiltering"]);
            InvitationFilteringInterval = Utilities.SetNullTimeSpan(dr["InvitationFilteringInterval"]);
            MaxFailedAttempts = Null.SetNullInteger(dr["MaxFailedAttempts"]);
            LockoutDuration = Utilities.SetNullTimeSpan(dr["LockoutDuration"]);
            RequireModeration = Null.SetNullBoolean(dr["RequireModeration"]);

            //Invitation Processing Settings

            ProcessorPageName = Null.SetNullString(dr["ProcessorPageName"]);
            AllowUsernameModification = Null.SetNullBoolean(dr["AllowUsernameModification"]);
            AllowFirstLastNameModification = Null.SetNullBoolean(dr["AllowFirstLastNameModification"]);
            AllowDisplayNameModification = Null.SetNullBoolean(dr["AllowDisplayNameModification"]);
            RequireTemporaryPasswordEntry = Null.SetNullBoolean(dr["RequireTemporaryPasswordEntry"]);
            RequirePasswordChangeOnAcceptance = Null.SetNullBoolean(dr["RequirePasswordChangeOnAcceptance"]);
            RequirePasswordConfirmOnChange = Null.SetNullBoolean(dr["RequirePasswordConfirmOnChange"]);
            EnableReasonDeclinedField = Null.SetNullBoolean(dr["EnableReasonDeclinedField"]);
            SuspendInvitationProcessing = Null.SetNullBoolean(dr["SuspendInvitationProcessing"]);

            // Notification Settings

            SendFrom = Null.SetNullString(dr["SendFrom"]);
            SendCopiesTo = Null.SetNullString(dr["SendCopiesTo"]);
            SendCopiesToRoles = Null.SetNullString(dr["SendCopiesToRoles"]);
            EnabledInvitingUserNotifications = (Notifications)Null.SetNullInteger(dr["EnabledInvitingUserNotifications"]);
            EnabledAdminUserNotifications = (Notifications)Null.SetNullInteger(dr["EnabledAdminUserNotifications"]);
        }

        public int KeyID
        {
            get
            {
                return PortalId;
            }
            set
            {
                PortalId = value;
            }
        }

        #endregion

        #region IPropertyAccess Members

        public CacheLevel Cacheability
        {
            get { return CacheLevel.notCacheable; }
        }

        public string GetProperty(string propertyName, string format, System.Globalization.CultureInfo formatProvider, UserInfo accessingUser, Scope accessLevel, ref bool propertyNotFound)
        {
            if (string.IsNullOrEmpty(format)) format = "g";
            if (formatProvider == null) formatProvider = System.Threading.Thread.CurrentThread.CurrentCulture;

            propertyNotFound = false;
            string result = string.Empty;

            switch (propertyName.ToLowerInvariant())
            {
                case "portalid":
                    result = PortalId.ToString(format, formatProvider);
                    break;
                case "portalname":
                    var portalInfo = new PortalController().GetPortal(PortalId);
                    result = portalInfo.PortalName;
                    break;
                case "autodeletearchiveexpiredinvitations":
                    result = AutoDeleteArchiveExpiredInvitations.ToString("g");
                    break;
                case "processorpagename":
                    result = ProcessorPageName;
                    break;
                case "requiremoderation":
                    result = RequireModeration.ToString();
                    break;
            }
            return result;
        }

        #endregion
    }
}