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
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using System.Xml.Schema;
using System.IO;

using DotNetNuke;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Modules.Definitions;
using DotNetNuke.Entities.Tabs;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Users;
using DotNetNuke.Security;
using DotNetNuke.Security.Roles;
using DotNetNuke.Security.Permissions;
using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Services.FileSystem;
using DotNetNuke.UI;
using DotNetNuke.UI.Skins.Controls;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Common.Lists;
using DotNetNuke.Services.Localization;
using DotNetNuke.Services.Social.Notifications;


using System.Text;


namespace WESNet.DNN.Modules.ByInvitation
{
    public class Utilities
    {
        public static DateTime? NullDateTime = null;

        public static int SetNullAsZeroInteger(object objValue)
        {
            return DBNull.Value == objValue ? 0 : Convert.ToInt32(objValue);
        }

        public static long SetNullLong (object objValue)
        {
            return DBNull.Value == objValue ? -1L : Convert.ToInt64(objValue);
        }

        public static long SetNullTicks (object objValue)
        {
            return DBNull.Value == objValue ? 0L : Convert.ToInt64(objValue);
        }

        public static TimeSpan SetNullTimeSpan (object objValue)
        {
            long temp = SetNullTicks(objValue);
            return temp == 0L ? TimeSpan.Zero : new TimeSpan(temp);
        }

        public static DateTime? SetNullableDateTime (object objValue)
        {
            return DBNull.Value == objValue ?  NullDateTime : Convert.ToDateTime(objValue);
        }

        public static bool NullableDateTimeTryParse(string s, out DateTime? value)
        {
            DateTime tmpValue;
            if (DateTime.TryParse(s, out tmpValue))
            {
                value = tmpValue;
                return true;
            }
            value = null;
            return false;
        }
 
        public static List<int> GetUserRoles (UserInfo user)
        {
            var rc = new DotNetNuke.Security.Roles.RoleController();
            return rc.GetUserRoles(user, true).Select<UserRoleInfo, int>(r => r.RoleID).ToList<int>();  
        }

        public static string FormattedDate(DateTime dt, string format, System.Globalization.CultureInfo formatProvider)
        {
            if (dt == DateTime.MinValue || dt == DateTime.MaxValue) return string.Empty;

            return dt.ToString(format, formatProvider);
        }

        public static string FormattedUTCDate(DateTime value, UserInfo accessingUser)
        {
            return FormattedUTCDate(value, accessingUser, "d");
        }

        public static string FormattedUTCDate(DateTime value, UserInfo accessingUser, string format)
        {
            return FormattedUTCDate(value, accessingUser, format, Thread.CurrentThread.CurrentUICulture);
        }
        
        public static string FormattedUTCDate(DateTime value, UserInfo accessingUser, string format, System.Globalization.CultureInfo formatProvider)
        {
            DateTime localTime;
            if (accessingUser == null || accessingUser.UserID == -1)
            {
                localTime = TimeZoneInfo.ConvertTime(value, TimeZoneInfo.Utc, PortalController.Instance.GetCurrentPortalSettings().TimeZone);
            }
            else
            {
                localTime = accessingUser.LocalTime(value);
            }

            return value == DateTime.MinValue ? "---" : localTime.ToString(format, formatProvider);
        }

        public static List<int> GetPortalRoles (int portalId)
        {
            var rc = new DotNetNuke.Security.Roles.RoleController();
            return rc.GetRoles(portalId).Select<RoleInfo, int>(r => r.RoleID).ToList<int>();
        }

        public static T GetSettingValue<T>(object setting, T defaultvalue)
        {
            if (setting == null || (defaultvalue is System.String && ((string)setting).Length == 0))
            {
                return defaultvalue;
            }
            else
            {
                if (defaultvalue is System.Enum)
                {
                    try
                    {
                        return ((T)(Enum.Parse(typeof(T), System.Convert.ToString(setting))));
                    }
                    catch (ArgumentException)
                    {
                        return defaultvalue;
                    }
                }
                else if (defaultvalue is System.DateTime)
                {
                    object objDateTime;
                    try
                    {
                        objDateTime = DateTime.Parse(System.Convert.ToString(setting));
                    }
                    catch (FormatException)
                    {
                        DateTime dt;
                        if (!DateTime.TryParse(System.Convert.ToString(setting), System.Globalization.DateTimeFormatInfo.InvariantInfo, System.Globalization.DateTimeStyles.None, out dt))
                        {
                            dt = DateTime.Now;
                        }
                        objDateTime = dt;
                    }
                    return ((T)objDateTime);
                }
                else
                {
                    try
                    {
                        return (T)Convert.ChangeType(setting, typeof(T));
                    }
                    catch (InvalidCastException)
                    {
                        return defaultvalue;
                    }
                }
            }
        }

        public static T GetUserSetting<T>(int portalId, string key, T defaultValue)
        {
            Hashtable userSettings = UserController.GetUserSettings(portalId);
            object obj = null;

            if (userSettings.ContainsKey(key))
            {
                obj = userSettings[key];
                return GetSettingValue(obj, defaultValue);
            }
            else
            {
                return defaultValue;
            }
        }

        public static string ResolveImageUrl(int portalId, string relativePath )
        {
            string imageUrl = string.Empty;
            if (!string.IsNullOrEmpty(relativePath))
            {
                IFileInfo fileInfo = null;
                var m = Regex.Match(relativePath, @"^(?:fileid=)?(\d+)$");
                if (m.Success)
                {
                    var fileID = int.Parse(m.Groups[1].Value);
                    fileInfo = FileManager.Instance.GetFile(fileID);
                }
                else
                {
                    fileInfo = FileManager.Instance.GetFile(portalId, relativePath);
                }

                if (fileInfo != null)
                {
                    imageUrl = FileManager.Instance.GetUrl(fileInfo);
                }
            }
            return imageUrl;
        }

        public static string FormatHtmlContent(string htmlContent)
        {
            string s = HttpUtility.HtmlDecode(htmlContent);
            s = Globals.ManageUploadDirectory(s, PortalSettings.Current.HomeDirectory);
            return HttpUtility.HtmlEncode(s);
        }

        public static string LocalizeSharedResource(string key)
        {
            return Localization.GetString(key, Configuration.LocalSharedResourceFile);
        }

        public static string LocalizeInvitationSubmissionStatus(InvitationSubmissionStatus status)
        {
            return LocalizeSharedResource("SubmissionStatus." + status.ToString());
        }

        public static void AddTimedModuleMessage(Control control, string message, ModuleMessage.ModuleMessageType moduleMessageType, string redirectUrl)
        {
            AddTimedModuleMessage(control, "", message, moduleMessageType, "", 5000, 600, redirectUrl);
        }

        public static void AddTimedModuleMessage(Control control, string heading, string message, ModuleMessage.ModuleMessageType moduleMessageType, string iconSrc, int delay, int speed, string redirectUrl)
        {
            if (control != null)
            {
                if (!String.IsNullOrEmpty(message))
                {
                    var messagePlaceHolder = ControlUtilities.FindControl<PlaceHolder>(control, "MessagePlaceHolder", true);
                    if (messagePlaceHolder != null)
                    {
                        messagePlaceHolder.Visible = true;
                        ModuleMessage moduleMessage = DotNetNuke.UI.Skins.Skin.GetModuleMessageControl(heading, message, moduleMessageType, iconSrc);
                        messagePlaceHolder.Controls.Add(moduleMessage);
                        Panel messagePanel = moduleMessage.FindControl("dnnSkinMessage") as Panel;
                        if (messagePanel != null)
                        {
                            ClientScriptManager csm = control.Page.ClientScript;
                            var script = new StringBuilder();
                            script.AppendLine("$(document).ready(function () {");
                            if (string.IsNullOrEmpty(redirectUrl))
                            {
                                script.AppendFormat("   $('#{0}').delay({1}).fadeOut({2});", messagePanel.ClientID, delay, speed);
                            }
                            else
                            {
                                script.AppendFormat("   $('#{0}').delay({1}).fadeOut({2}, function(){{window.location.href='{3}'}});", messagePanel.ClientID, delay, speed, redirectUrl);

                            }                
                            script.AppendLine("});");
                            csm.RegisterStartupScript(Type.GetType("WESNet.DNN.Modules.ByInvitation.Utilities"), "WESNet_TimedModuleMessage", script.ToString(), true);
                        }
                    }
                }
            }
        }

        public static ModuleDefinitionInfo GetModuleDefinition(string desktopModuleName, string moduleDefinitionName)
        {
            ModuleDefinitionInfo moduleDefinition = null;

            var desktopModule = DesktopModuleController.GetDesktopModuleByModuleName(desktopModuleName, Null.NullInteger);

            if (desktopModule != null)
            {
                moduleDefinition = ModuleDefinitionController.GetModuleDefinitionByFriendlyName(moduleDefinitionName, desktopModule.DesktopModuleID);
            }

            return moduleDefinition; 
        }

        //public static ModuleDefinitionInfo AddModuleDefinition (string desktopModuleName, string description, string moduleDefinitionName, string controlTitle,
        //                                                        string controlPath, string controlKey, DotNetNuke.Security.SecurityAccessLevel accessLevel)
        //{
        //    var moduleDefID = DotNetNuke.Services.Upgrade.Upgrade.AddModuleDefinition(desktopModuleName, description, moduleDefinitionName);
        //    DotNetNuke.Services.Upgrade.Upgrade.AddModuleControl(moduleDefID, controlKey, controlTitle, controlPath, "", accessLevel, 0);
        //    return ModuleDefinitionController.GetModuleDefinitionByID(moduleDefID);
        //}

        public static void AddPagePermission(TabPermissionCollection permissions, string key, int roleId)
        {
            var permissionController = new PermissionController();
            var permission = (PermissionInfo)permissionController.GetPermissionByCodeAndKey("SYSTEM_TAB", key)[0];
            var tabPermission = new TabPermissionInfo { PermissionID = permission.PermissionID, RoleID = roleId, AllowAccess = true };
            permissions.Add(tabPermission);
        }

        public static TabInfo AddPage(int portalId, int parentId, string tabName, string description, string tabIconFile, string tabIconFileLarge, bool isVisible, TabPermissionCollection permissions, bool isAdmin)
        {
            var tabController = new TabController();

            TabInfo tab = tabController.GetTabByName(tabName, portalId, parentId);

            if (tab == null || tab.ParentId != parentId)
            {
                tab = new TabInfo
                {
                    TabID = Null.NullInteger,
                    PortalID = portalId,
                    TabName = tabName,
                    Title = "",
                    Description = description,
                    KeyWords = "",
                    IsVisible = isVisible,
                    DisableLink = false,
                    ParentId = parentId,
                    IconFile = tabIconFile,
                    IconFileLarge = tabIconFileLarge,
                    IsDeleted = false
                };
                tab.TabID = tabController.AddTab(tab, !isAdmin);
                tab.TabSettings["AllowIndex"] = "false";

                if (((permissions != null)))
                {
                    foreach (TabPermissionInfo tabPermission in permissions)
                    {
                        tab.TabPermissions.Add(tabPermission, true);
                    }
                    TabPermissionController.SaveTabPermissions(tab);
                }
            }
            return tab;
        }

        public static int AddModuleToPage(TabInfo page, int moduleDefId, string moduleTitle, string moduleIconFile, bool inheritPermissions)
        {
            ModuleInfo moduleInfo;
            int moduleId = Null.NullInteger;

            if ((page != null))
            {
                bool isDuplicate = false;
                foreach (var kvp in ModuleController.Instance.GetTabModules(page.TabID))
                {
                    moduleInfo = kvp.Value;
                    if (moduleInfo.ModuleDefID == moduleDefId)
                    {
                        isDuplicate = true;
                        moduleId = moduleInfo.ModuleID;
                    }
                }

                if (!isDuplicate)
                {
                    moduleInfo = new ModuleInfo
                    {
                        ModuleID = Null.NullInteger,
                        PortalID = page.PortalID,
                        TabID = page.TabID,
                        ModuleOrder = -1,
                        ModuleTitle = moduleTitle,
                        PaneName = Globals.glbDefaultPane,
                        ModuleDefID = moduleDefId,
                        CacheTime = 0,
                        IconFile = moduleIconFile,
                        AllTabs = false,
                        Visibility = VisibilityState.None,
                        InheritViewPermissions = inheritPermissions
                    };

                    try
                    {
                        moduleId = ModuleController.Instance.AddModule(moduleInfo);
                    }
                    catch (Exception exc)
                    {
                        var dnnException = new ModuleLoadException("Unable to add By WESNet Invitation Processing Module to page", exc);
                        Exceptions.LogException(dnnException);
                        
                    }
                }
            }

            return moduleId;
        }

        public static DateTime UserToUTCTime(DateTime userTime, UserInfo user)
        {
            if (user == null || user.UserID == Null.NullInteger)
            {
               return TimeZoneInfo.ConvertTime(userTime, PortalController.Instance.GetCurrentPortalSettings().TimeZone, TimeZoneInfo.Utc); 
            }
            return TimeZoneInfo.ConvertTime(userTime, user.Profile.PreferredTimeZone, TimeZoneInfo.Utc);
        }

        public static DateTime UserToUTCTime(DateTime userTime, int userID)
        {
            var ps = PortalController.Instance.GetCurrentPortalSettings();
            if (userID != Null.NullInteger)
            {
                var user = UserController.Instance.GetUserById(ps.PortalId, userID);
                if (user != null)
                {
                    return TimeZoneInfo.ConvertTime(userTime, user.Profile.PreferredTimeZone, TimeZoneInfo.Utc);
                }
            }
            return TimeZoneInfo.ConvertTime(userTime, ps.TimeZone, TimeZoneInfo.Utc);
        }


        public static InvitationSubmissionStatus CleanAndVerifyData(InvitationInfo invitation, bool generateRandomPassword)
        {
            if (invitation == null || invitation.PortalId == -1)
            {
                throw new ArgumentException("Invitation object cannot be null or have invalid PortalId.");
            }

            var myConfiguration = Configuration.GetConfiguration(invitation.PortalId);

            var myLocalizedConfiguration = myConfiguration.GetLocalizedConfiguration(invitation.RecipientCultureCode, Thread.CurrentThread.CurrentUICulture.Name);

            if (invitation.TabId == -1 || invitation.ModuleId == -1)
            {
                throw new ArgumentException("Invitation object cannot have invalid TabId or ModuleId.");
            }

            var moduleSecurity = new Security(invitation.TabId, invitation.ModuleId, invitation.InvitedByUserID);

            var dataBaseDateTime = DateUtils.GetDatabaseTime();

            var ps = new DotNetNuke.Security.PortalSecurity();
            var filterFlags = DotNetNuke.Security.PortalSecurity.FilterFlag.NoMarkup | DotNetNuke.Security.PortalSecurity.FilterFlag.NoScripting;

            invitation.InvitedByUserFullName = ps.InputFilter(invitation.InvitedByUserFullName, filterFlags);
            invitation.InvitedByUserEmail = ps.InputFilter(invitation.InvitedByUserEmail, filterFlags);

            var emailRegex = myLocalizedConfiguration.EmailRegex;
            invitation.RecipientEmail = ps.InputFilter(invitation.RecipientEmail, filterFlags);
            if (emailRegex != string.Empty && !Regex.IsMatch(invitation.RecipientEmail, emailRegex))
            {
                return InvitationSubmissionStatus.InvalidEmail;
            }

            invitation.RecipientFirstName = ps.InputFilter(invitation.RecipientFirstName, filterFlags);
            invitation.RecipientLastName = ps.InputFilter(invitation.RecipientLastName, filterFlags);

            var existingInvitation = InvitationController.GetInvitationByRecipientEmail(invitation.PortalId, invitation.RecipientEmail);
            if (existingInvitation != null && existingInvitation.InvitationID != invitation.InvitationID)
            {
                if (existingInvitation.RecipientFirstName == invitation.RecipientFirstName
                      && existingInvitation.RecipientLastName == invitation.RecipientLastName)
                {
                    if (existingInvitation.IsPending) return InvitationSubmissionStatus.InvitationAlreadyPending;
                    if (existingInvitation.WasAccepted) return InvitationSubmissionStatus.InvitationAlreadyAccepted;
                    if (existingInvitation.WasDeclined) return InvitationSubmissionStatus.InvitationAlreadyDeclined;
                }
                else if (moduleSecurity.UseEmailAsUsername)
                {
                    return InvitationSubmissionStatus.DuplicateEmail;
                }
            }

            if (myConfiguration.EnablePersonalNote)
            {
                invitation.PersonalNote = ps.InputFilter(invitation.PersonalNote, filterFlags);
                invitation.PersonalNote = ReplaceProfanity(invitation.PersonalNote, invitation.PortalId);
            }
            else
            {
                invitation.PersonalNote = string.Empty;
            }

            if (moduleSecurity.CanAssignCredentials || moduleSecurity.UseEmailAsUsername)
            {
                var assignedUsername = string.Empty;
                if (moduleSecurity.UseEmailAsUsername)
                {
                    assignedUsername = invitation.RecipientEmail;
                }
                else
                {
                    assignedUsername = ps.InputFilter(invitation.AssignedUsername, filterFlags);

                    if (moduleSecurity.RegistrationExcludedTermsRegex != string.Empty &&
                          Regex.IsMatch(assignedUsername, moduleSecurity.RegistrationExcludedTermsRegex))
                    {
                        return InvitationSubmissionStatus.InvalidUsername;
                    }

                    if (moduleSecurity.RegistrationUseProfanityFilter &&
                        assignedUsername != Utilities.ReplaceProfanity(assignedUsername, invitation.PortalId))
                    {
                        return InvitationSubmissionStatus.InvalidUsername;
                    }

                    if (moduleSecurity.UsernameValidation != string.Empty && !Regex.IsMatch(assignedUsername, moduleSecurity.UsernameValidation))
                    {
                        return InvitationSubmissionStatus.InvalidUsername;
                    }

                    existingInvitation = InvitationController.GetInvitationByAssignedUsername(invitation.PortalId, assignedUsername);

                    if (existingInvitation != null && existingInvitation.InvitationID != invitation.InvitationID && 
                            existingInvitation.IsPending && existingInvitation.RecipientEmail == invitation.RecipientEmail)
                    {
                        return InvitationSubmissionStatus.InvitationAlreadyPending;
                    }
                }

                var tmpUser = UserController.GetUserByName(invitation.PortalId, assignedUsername);

                if (tmpUser != null)
                {
                    if (tmpUser.Email == invitation.RecipientEmail)
                    {
                        return moduleSecurity.UseEmailAsUsername ? InvitationSubmissionStatus.DuplicateEmail : InvitationSubmissionStatus.UserAlreadyRegistered;
                    }

                    var i = 2;
                    var baseUsername = assignedUsername;
                    while (tmpUser != null && i <= 9)
                    {
                        assignedUsername = baseUsername + i.ToString("0#");
                        existingInvitation = InvitationController.GetInvitationByAssignedUsername(invitation.PortalId, assignedUsername);
                        if (existingInvitation == null)
                        {
                            tmpUser = UserController.GetUserByName(invitation.PortalId, assignedUsername);
                        }
                        i++;
                    }
                    invitation.AssignedUsername = assignedUsername;
                    return InvitationSubmissionStatus.UsernameAlreadyExists;
                }

                invitation.AssignedUsername = assignedUsername;

            }

            var displayName = string.Empty;
            if (moduleSecurity.DisplayNameFormat == "")
            {
                displayName = ps.InputFilter(invitation.AssignedDisplayName, filterFlags);

                if (moduleSecurity.RegistrationExcludedTermsRegex != string.Empty &&
                          Regex.IsMatch(displayName, moduleSecurity.RegistrationExcludedTermsRegex))
                {
                    return InvitationSubmissionStatus.InvalidDisplayName;
                }

                if (moduleSecurity.RegistrationUseProfanityFilter &&
                    displayName != Utilities.ReplaceProfanity(displayName, invitation.PortalId))
                {
                    return InvitationSubmissionStatus.InvalidDisplayName;
                }
            }
            else
            {
                displayName = moduleSecurity.DisplayNameFormat;
                displayName = displayName.Replace("[FIRSTNAME]", invitation.RecipientFirstName);
                displayName = displayName.Replace("[LASTNAME]", invitation.RecipientLastName);
                displayName = displayName.Replace("[USERNAME]", invitation.AssignedUsername);
            }

            if (moduleSecurity.RequireUniqueDisplayName && !moduleSecurity.UseEmailAsUsername)
            {
                var tmpUser = UserController.Instance.GetUserByDisplayname(invitation.PortalId, displayName);
                if (tmpUser != null)
                {
                    var i = 1;
                    var baseDisplayName = displayName;

                    while (tmpUser != null && i <= 9)
                    {
                        displayName = baseDisplayName + i.ToString("0#");
                        tmpUser = UserController.Instance.GetUserByDisplayname(invitation.PortalId, displayName);
                        i++;
                    }
                    invitation.AssignedDisplayName = displayName;
                    return InvitationSubmissionStatus.DuplicateDisplayName;
                }
            }

            invitation.AssignedDisplayName = displayName;

            var tmpPasswordPlainText = string.Empty;

            if (myConfiguration.TemporaryPasswordMode == TemporaryPasswordMode.AutoGenerateTemporaryPassword ||
                    (!moduleSecurity.CanAssignCredentials && myConfiguration.TemporaryPasswordMode == TemporaryPasswordMode.RequiredTemporaryPassword) || generateRandomPassword)
            {
                tmpPasswordPlainText = UserController.GeneratePassword();
            }
            else if (moduleSecurity.CanAssignCredentials && myConfiguration.TemporaryPasswordMode != TemporaryPasswordMode.NoTemporaryPassword)
            {
                tmpPasswordPlainText = invitation.TemporaryPassword;
            }

            if (tmpPasswordPlainText == string.Empty)
            {
                if (myConfiguration.TemporaryPasswordMode == TemporaryPasswordMode.RequiredTemporaryPassword)
                {
                    return InvitationSubmissionStatus.TemporaryPasswordRequired;
                }
                else
                {
                    invitation.TemporaryPassword = string.Empty;
                    invitation.TemporaryPasswordSalt = string.Empty;
                }
            }
            else
            {
                if (!UserController.ValidatePassword(tmpPasswordPlainText))
                {
                    return InvitationSubmissionStatus.InvalidPassword;
                }
                var tmpPasswordSalt = "";
                var tmpPasswordEncrypted = Security.EncryptString(tmpPasswordPlainText, ref tmpPasswordSalt);
                invitation.TemporaryPassword = tmpPasswordEncrypted;
                invitation.TemporaryPasswordSalt = tmpPasswordSalt;
            }

            if (moduleSecurity.CanAssignRedirection && invitation.RedirectOnFirstLogin != -1)
            {
                var redirectPage = TabController.Instance.GetTab(invitation.RedirectOnFirstLogin, invitation.PortalId);
                if (redirectPage == null)      // also test for permissions??
                {
                    return InvitationSubmissionStatus.InvalidRedirectOnLogin;
                }
            }

            invitation.InvitedOnDate = dataBaseDateTime;
            if (myConfiguration.ValidityPeriod != TimeSpan.MinValue) invitation.ExpiresOnDate = dataBaseDateTime.Add(myConfiguration.ValidityPeriod);

            return InvitationSubmissionStatus.Success;
        }

        // The following is a substitute for the PortalSecurity.Replace method which cannot be called from a scheduled job as it relies on the HttpContext
        // to provide a valid PortalSettings object. Since HttpContext.Current is null in a scheduled job, the call fails with a null reference exception.

        public static string ReplaceProfanity(string inputString, int portalId)
        {
            return ReplaceProfanity(inputString, PortalSecurity.ConfigType.ListController, null, PortalSecurity.FilterScope.SystemAndPortalList, portalId);
        }

        public static string ReplaceProfanity(string inputString, PortalSecurity.ConfigType configType, string configSource, PortalSecurity.FilterScope filterScope, int portalId)
        {
            switch (configType)
            {
                case PortalSecurity.ConfigType.ListController:
                    const RegexOptions options = RegexOptions.IgnoreCase | RegexOptions.Singleline;
                    const string listName = "ProfanityFilter";

                    var listController = new ListController();

                    IEnumerable<ListEntryInfo> listEntryHostInfos;
                    IEnumerable<ListEntryInfo> listEntryPortalInfos;

                    switch (filterScope)
                    {
                        case PortalSecurity.FilterScope.SystemList:
                            listEntryHostInfos = listController.GetListEntryInfoItems(listName, "", Null.NullInteger);
                            inputString = listEntryHostInfos.Aggregate(inputString, (current, removeItem) => Regex.Replace(current, @"\b" + removeItem.Text + @"\b", removeItem.Value, options));
                            break;
                        case PortalSecurity.FilterScope.SystemAndPortalList:
                            listEntryHostInfos = listController.GetListEntryInfoItems(listName, "", Null.NullInteger);
                            listEntryPortalInfos = listController.GetListEntryInfoItems(listName + "-" + portalId, "", portalId);
                            inputString = listEntryHostInfos.Aggregate(inputString, (current, removeItem) => Regex.Replace(current, @"\b" + removeItem.Text + @"\b", removeItem.Value, options));
                            inputString = listEntryPortalInfos.Aggregate(inputString, (current, removeItem) => Regex.Replace(current, @"\b" + removeItem.Text + @"\b", removeItem.Value, options));
                            break;
                        case PortalSecurity.FilterScope.PortalList:
                            listEntryPortalInfos = listController.GetListEntryInfoItems(listName + "-" + portalId, "", portalId);
                            inputString = listEntryPortalInfos.Aggregate(inputString, (current, removeItem) => Regex.Replace(current, @"\b" + removeItem.Text + @"\b", removeItem.Value, options));
                            break;
                    }
                    break;
                case PortalSecurity.ConfigType.ExternalFile:
                    throw new NotImplementedException();
                default:
                    throw new ArgumentOutOfRangeException("configType");
            }

            return inputString;
        }

        public static bool IsValidExtension (string ext, ImportSourceTypes type)
        {
            return Consts.ValidFileExtensions[(int)type].Contains(ext);
        }

        public static string GetOrCreateImportFolderPath()
        {
            try
            {
                var physicalImportFolderPath = Path.Combine(Globals.ApplicationMapPath, Consts.ImportFolderPath);
                var directoryInfo = new DirectoryInfo(physicalImportFolderPath);
                if (!directoryInfo.Exists)
                {
                    directoryInfo.Create();
                }
                return physicalImportFolderPath;
            }
            catch
            {
                return string.Empty;
            }
        }

        public static List<string> GetImportFolderFileNames(string[] extensions)
        {
            var importFiles = new List<string>();
            var physicalImportFolderPath = GetOrCreateImportFolderPath();
            if (physicalImportFolderPath != string.Empty)
            {
                var directoryInfo = new DirectoryInfo(physicalImportFolderPath);
                importFiles = directoryInfo.EnumerateFiles().Where(f => extensions.Contains(f.Extension.ToLower())).Select(f1 => f1.Name).OrderBy (f2 => f2).ToList();
            }
            return importFiles;
        }

        public static string GetImportFilePhysicalPath(string filename)
        {
            var physicalImportFilePath = Path.Combine(GetOrCreateImportFolderPath(), filename);
            if (File.Exists(physicalImportFilePath))
            {
                return physicalImportFilePath;
            }
            return string.Empty;
        }

        public static string SaveImportFile(Stream s, string filename)
        {
            var errorMsg = string.Empty;
            byte[] buffer = new byte[2048];
            var bytesRead = 0;
            var originalPosition =-1L;  
         
            try
            {
                if (s != null && !string.IsNullOrEmpty(filename))
                {
                    var physicalImportFilePath = Path.Combine(GetOrCreateImportFolderPath(), filename);
                    if (File.Exists(physicalImportFilePath))
                    {
                        File.SetAttributes(physicalImportFilePath, FileAttributes.Normal);
                        File.Delete(physicalImportFilePath);
                    }

                    using (var fs = new FileStream(physicalImportFilePath, FileMode.Create))
                    {
                        if (s.CanSeek)
                        {
                            originalPosition = s.Position;
                            s.Position = 0L;
                        }
                        bytesRead = s.Read(buffer, 0, buffer.Length);
                        while (bytesRead > 0)
                        {
                            fs.Write(buffer, 0, bytesRead);
                            bytesRead = s.Read(buffer, 0, buffer.Length);
                        }
                    }
                }
                else
                {
                    errorMsg = Utilities.LocalizeSharedResource("Import file upload stream is empty or closed or no filename was provided.");
                }
            }
            catch (IOException exc)
            {
                errorMsg = Utilities.LocalizeSharedResource("FileUploadError") + Environment.NewLine + exc.Message;
            }
            finally
            {
                if (originalPosition > -1L)
                {
                    s.Position = originalPosition;
                }             
            }
            return errorMsg;
        }

        public static void ModerateInvitation(InvitationInfo invitation, bool approve, int moderatorUserID)
        {

            var myConfiguration = WESNet.DNN.Modules.ByInvitation.Configuration.GetConfiguration(invitation.PortalId);
            var newExpiresOnDate = DateUtils.GetDatabaseTime().Add(myConfiguration.ValidityPeriod);

            var moderationAction = approve ? "approved" : "disapproved";

            invitation = InvitationController.UpdateInvitationStatus(invitation.InvitationID, moderationAction, moderatorUserID, newExpiresOnDate);
            
            // send approved/rejected post notification to inviting user and if approved email the invition to recipient and any copy recipients.

            var mailManager = new MailManager(invitation);
            if (approve) mailManager.SendBulkMail("sent");

            if (invitation.InvitedByUserID == -1) //anonymous inviting user?
            {
                mailManager.SendBulkMail(moderationAction);
            }
            else
            {
                NotificationsHelper.SendNotifications(invitation, approve ? Notifications.Approved : Notifications.Disapproved);
            }

            NotificationsHelper.DeleteAllNotificationRecipients(Consts.ModerationRequestedNotification.Name, invitation.InvitationID);
        }
    }
}