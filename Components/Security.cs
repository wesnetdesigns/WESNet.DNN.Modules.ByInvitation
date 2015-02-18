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
using System.Text;
using System.IO;
using System.Security.Cryptography;
using System.Net;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Users;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Security;
using DotNetNuke.Security.Membership;
using DotNetNuke.Security.Permissions;
using DotNetNuke.Security.Roles;
using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;


namespace WESNet.DNN.Modules.ByInvitation
{
    public class Security
    {
        private const int KeyLengthBits = 256;      //AES Key Length in bits
        private const int SaltLength = 8;           //Salt length in bytes

        public const string PermissionCode = "WESNET_BYINVITATION";
        public const string AssignCredentials_PermissionKey = "CREDENTIAL";
        public const string AssignRedirection_PermissionKey = "REDIRECT";
        public const string ManageOwnInvitations_PermissionKey = "MANAGE";
        public const string RetractInvitation_PermissionKey = "RETRACT";
        public const string ExtendInvitation_PermissionKey = "EXTEND";
        public const string ModerateInvitations_PermissionKey = "MODERATE";
        public const string BulkImportInvitations_PermissionKey = "BULKIMPORT";
        public const string DefaultDisplayNameFormat = "";
        public const string DefaultUserNameValidation = ".*";

        private ModuleInfo _moduleConfiguration = null;
        private UserInfo _CurrentUser = null;
        private Dictionary<int, RoleBasedLimits> _RoleBasedLimitsCollection = null;
        private List<int> _UserRoles = null;
        private Hashtable _UserSettings = null;
        private List<UserInfo> _Moderators = null;

#region Constructors

        public Security (ModuleInfo moduleInfo)
        {
            _moduleConfiguration = moduleInfo;
            GetLimits();
        }

        public Security (int tabId, int moduleId)
        {
            _moduleConfiguration = ModuleController.Instance.GetModule(moduleId, tabId, false);
            GetLimits();
        }

        public Security (int tabId, int moduleId, int userId)
        {
            _moduleConfiguration = ModuleController.Instance.GetModule(moduleId, tabId, false);
            CurrentUser = UserController.GetUserById(PortalId, userId);
            GetLimits();
        }

#endregion


#region Public Read-Only Properties

        public int PortalId
        {
            get
            {               
                return _moduleConfiguration.PortalID;
            }
        }

        public int TabId
        {
            get
            {
                return _moduleConfiguration.TabID;              
            }
        }

        public int ModuleId
        {
            get
            {
                return _moduleConfiguration.ModuleID;
            }
        }

        public ModuleInfo ModuleInfo
        {
            get
            {
                return _moduleConfiguration;
            } 
        }

        public PortalSettings PortalSettings
        {
            get
            {
                return (HttpContext.Current == null ? new PortalSettings(TabId, PortalId) : PortalController.Instance.GetCurrentPortalSettings());
            }
        }

        public Hashtable UserSettings
        {
            get
            {
                if (_UserSettings == null)
                {
                    _UserSettings = UserController.GetUserSettings(PortalId);
                }
                return _UserSettings;
            }
        }

        public bool RegistrationUseProfanityFilter
        {
            get
            {
                return GetUserSetting("Registration_UseProfanityFilter", false);
            }
        }

        public bool RequireUniqueDisplayName
        {
            get
            {
                return GetUserSetting("Registration_RequireUniqueDisplayName", false);
            }
        }

        public string DisplayNameFormat
        {
            get
            {
                return GetUserSetting("Security_DisplayNameFormat", DefaultDisplayNameFormat);
            }
        }

        public bool UseEmailAsUsername
        {
            get
            {
                return GetUserSetting("Registration_UseEmailAsUserName", false);
            }
        }

        public string RegistrationExcludedTermsRegex
        {
            get
            {
                var excludedTerms = GetUserSetting("Registration_ExcludeTerms", "").Replace(" ", "").Replace(",", "|");
                if (excludedTerms != string.Empty)
                {
                    return @"^(?:(?!" + excludedTerms + @").)*$\r?\n?";
                }
                else return string.Empty;
            }
        }

        public string EmailValidation
        {
            get
            {
                var emailValidation = GetUserSetting("Security_EmailValidation", "");
                return emailValidation;
            }
        }

        public string UsernameValidation
        {
            get
            {
                var usernameValidation = GetUserSetting("Security_UserNameValidation", "");
                return usernameValidation == "" ? RegistrationExcludedTermsRegex : usernameValidation;
            }
        }

        public bool RequireValidProfile
        {
            get
            {
                return GetUserSetting("Security_RequireValidProfile", false) || GetUserSetting("Security_RequireValidProfileAtLogin", false);
            }
        }

        public bool ProfileDisplayVisability
        {
            get
            {
                return GetUserSetting("Profile_DisplayVisibility", true);
            }
        }

        public int RedirectAfterRegistration
        {
            get
            {
                return GetUserSetting("Redirect_AfterRegistration", -1);
            }
        }

        public int RedirectAfterFirstLogin
        {
            get
            {
                return GetUserSetting("Redirect_AfterLogin", -1);
            }
        }

        public PasswordFormat PasswordFormat
        {
            get
            {
                return MembershipProviderConfig.PasswordFormat;
            }
        }

        public UserInfo CurrentUser
        {
            get
            {
                if (_CurrentUser == null)
                {
                    _CurrentUser = UserController.Instance.GetCurrentUserInfo();
                    
                }
                return _CurrentUser;
            }
            set
            {
                _CurrentUser = value;
            }
        }

        public int UserID
        {
            get
            {
                return CurrentUser.UserID;
            }
        }

        public bool IsAnonymous
        {
            get
            {
            return CurrentUser.UserID == -1;
            }
        }

        public bool IsAuthenticated
        {
            get
            {
                return ((HttpContext.Current != null) && HttpContext.Current.Request.IsAuthenticated) || (CurrentUser.UserID != -1);
            }
        }

        public bool IsAdministrator
        {
            get
            {
                return CurrentUser.IsInRole(PortalSettings.AdministratorRoleName) || CurrentUser.IsSuperUser;
            }
        }

        public bool HasModulePermission(string permissionKey)
        {
            return IsAdministrator || ModulePermissionController.HasModulePermission(ModuleInfo.ModulePermissions, permissionKey);    
        }

        public bool CanInvite
        {
            get
            {
                bool canInvite;
                if (_moduleConfiguration.InheritViewPermissions)
                {
                    var tabPermissions = TabPermissionController.GetTabPermissions(TabId, PortalId);
                    canInvite = PortalSecurity.IsInRoles(CurrentUser, PortalSettings, tabPermissions.ToString("VIEW"));
                }
                else
                {
                    canInvite = HasModulePermission("VIEW");
                }
                return canInvite;
            }
        }

        public bool CanAssignCredentials
        {
            get
            {
                return HasModulePermission(AssignCredentials_PermissionKey);
            }
        }

        public bool CanAssignRedirection
        {
            get
            {
                return HasModulePermission(AssignRedirection_PermissionKey);
            }
        }

        public bool CanManageOwnInvitations
        {
            get
            {
                return HasModulePermission(ManageOwnInvitations_PermissionKey);
            }
        }

        public bool CanRetractInvitation
        {
            get
            {
                return HasModulePermission(RetractInvitation_PermissionKey);
            }
        }

        public bool CanExtendInvitation
        {
            get
            {
                return HasModulePermission(ExtendInvitation_PermissionKey);
            }
        }

        public bool CanModerate
        {
            get
            {
                return HasModulePermission(ModerateInvitations_PermissionKey);
            }
        }

        public bool CanBulkImportInvitations
        {
            get
            {
                return HasModulePermission(BulkImportInvitations_PermissionKey);
            }
        }

        public bool IsInvitingUser(InvitationInfo invitation)
        {
            return (invitation == null || invitation.InvitationID == -1) ? false : CurrentUser.UserID == invitation.InvitedByUserID;
        }

        public bool HasExceededAllocatedInvitations()
        {
            if (IsAnonymous || AllocatedInvitations == 0) return false;

            var endDate = DateUtils.GetDatabaseTime();
            var startDate = endDate - AllocationPeriod;
            var invitationCount = InvitationController.GetInvitationCount(PortalId, CurrentUser.UserID, startDate, endDate);

            return (AllocatedInvitations - invitationCount) == 0;
        }

        public bool HasExceededAllocatedInvitations(string invitedByUserEmail)
        {
            if (AllocatedInvitations == 0) return false;

            var endDate = DateUtils.GetDatabaseTime();
            var startDate = endDate - AllocationPeriod;
            var invitationCount = InvitationController.GetInvitationsByInvitingUserEmail(PortalId, invitedByUserEmail, startDate, endDate, false).Count();

            return (AllocatedInvitations - invitationCount) == 0;
        }

        public Dictionary<int, RoleBasedLimits> RoleBasedLimitsCollection
        {
            get
            {
                if (_RoleBasedLimitsCollection == null)
                {
                    _RoleBasedLimitsCollection = InvitationController.GetRoleBasedLimitsCollection(PortalId);
                }
                return _RoleBasedLimitsCollection;
            }
        }

        public List<int> UserRoles
        {
            get
            {
                if (_UserRoles == null)
                {
                    _UserRoles = Utilities.GetUserRoles(CurrentUser);     
                }
                return _UserRoles;
            }
        }

        public int AllocatedInvitations { get; private set; }

        public TimeSpan AllocationPeriod { get; private set; }

        public int MaxAllocationResets { get; private set; }

        public List<RoleInfo> AssignableRoles { get; private set; }

        public List<UserInfo> Moderators
        {
            get
            {
                if (_Moderators == null)
                {
                    var cacheItemKey = GetModeratorsCacheKey();
                    var cacheItemArgs = new CacheItemArgs(cacheItemKey, 20, System.Web.Caching.CacheItemPriority.Normal, ModuleId, TabId, PortalId);
                    _Moderators = CBO.GetCachedObject<List<UserInfo>>(cacheItemArgs, GetModerators);
                }
                return _Moderators;
            }
        }

#endregion

        #region Public/Private Methods

        public T GetUserSetting<T>(string key, T defaultValue)
        {
            object obj = null;

            if (UserSettings.ContainsKey(key))
            {
                obj = UserSettings[key];
                return Utilities.GetSettingValue(obj, defaultValue);
            }
            else
            {
                return defaultValue;
            }
        }      

        public bool IsInRole(string role)
        {
            if ((!String.IsNullOrEmpty(role) && role != null && ((IsAnonymous && role == Globals.glbRoleUnauthUserName))))
            {
                return true;
            }
            else
            {
                return CurrentUser.IsInRole(role);
            }
        }

        public bool IsInRoles(string roles)
        {
            //super user always has full access
            bool blnIsInRoles = CurrentUser.IsSuperUser;

            if (!blnIsInRoles)
            {
                if (roles != null)
                {
                    //permissions strings are encoded with Deny permissions at the beginning and Grant permissions at the end for optimal performance
                    foreach (string role in roles.Split(new[] { ';' }))
                    {
                        if (!String.IsNullOrEmpty(role))
                        {
                            //Deny permission
                            if (role.StartsWith("!"))
                            {
                                //Portal Admin cannot be denied from his/her portal (so ignore deny permissions if user is portal admin)
                                PortalSettings settings = PortalController.Instance.GetCurrentPortalSettings();
                                if (!(settings.PortalId == CurrentUser.PortalID && settings.AdministratorId == CurrentUser.UserID))
                                {
                                    string denyRole = role.Replace("!", "");
                                    if (((IsAnonymous && denyRole == Globals.glbRoleUnauthUserName) || denyRole == Globals.glbRoleAllUsersName || CurrentUser.IsInRole(denyRole)))
                                    {
                                        blnIsInRoles = false;
                                        break;
                                    }
                                }
                            }
                            else //Grant permission
                            {
                                if (((!IsAuthenticated && role == Globals.glbRoleUnauthUserName) || role == Globals.glbRoleAllUsersName || CurrentUser.IsInRole(role)))
                                {
                                    blnIsInRoles = true;
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            return blnIsInRoles;
        }

        private string GetModeratorsCacheKey()
        {
            return string.Format("WESNet_ByInvitation_Moderators:{0}:{1}:{2}", ModuleId, TabId, PortalId);
        }

        private static object GetModerators(CacheItemArgs args)
        {
            var cachekey = args.CacheKey;
            var moduleID = (int)args.Params[0];
            var tabID = (int)args.Params[1];
            var portalID = (int)args.Params[2];
            return GetModerators(moduleID, tabID, portalID);

        }

        private static List<UserInfo> GetModerators(int moduleID, int tabID, int portalID)
        {
            var modulePermissions = ModulePermissionController.GetModulePermissions(moduleID, tabID);

            var moderators = new List<UserInfo>();

            var permissions = modulePermissions.Where (mp => mp.PermissionKey == ModerateInvitations_PermissionKey && mp.AllowAccess);
            foreach (var p in permissions)
            {
                if (p.UserID != Null.NullInteger)
                {
                    var userInfo = UserController.GetUserById(portalID, p.UserID);
                    if (userInfo.Membership.Approved && !userInfo.IsDeleted && !userInfo.Membership.LockedOut)
                    {
                        moderators.Add(userInfo);
                    }
                }
                else if (p.RoleID != Null.NullInteger)
                {
                    var roleInfo = RoleController.Instance.GetRoleById(portalID, p.RoleID);
                    {
                        if (roleInfo.Status == RoleStatus.Approved)
                        {
                            var users = RoleController.Instance.GetUsersByRole(portalID, roleInfo.RoleName);
                            foreach (var user in users)
                            {
                                var userRole = RoleController.Instance.GetUserRole(portalID, user.UserID, roleInfo.RoleID);
                                if ((userRole.EffectiveDate <= DateTime.Now) && (userRole.ExpiryDate == DateTime.MinValue || userRole.ExpiryDate > DateTime.Now))
                                {
                                    moderators.Add(user);
                                }
                            }
                        }
                    }
                }
            }
            return moderators;
        }

        private void GetLimits()
        {
            int allocatedInvitationsPerDay = 0;

            AllocatedInvitations = 1;
            AllocationPeriod = Consts.DefaultAllocationPeriod;
            var assignableRoles = new Dictionary<int, RoleInfo>();

            var roleBasedLimitsCollection = RoleBasedLimitsCollection;
                  
            foreach (int RoleID in UserRoles)
            {
                RoleBasedLimits roleBasedLimits;
                if (roleBasedLimitsCollection.TryGetValue(RoleID, out roleBasedLimits))
                {
                    if (roleBasedLimits.AllocatedInvitationsPerDay > allocatedInvitationsPerDay)
                    {   
                        AllocatedInvitations = roleBasedLimits.AllocatedInvitations;
                        AllocationPeriod = roleBasedLimits.AllocationPeriod;
                    }

                    foreach (var roleInfo in roleBasedLimits.AssignableRolesList)
                    {
                        if (!assignableRoles.ContainsKey(roleInfo.RoleID))
                        {
                            assignableRoles.Add(roleInfo.RoleID, roleInfo);
                        }
                    }
                }
            }

            AssignableRoles = assignableRoles.Values.ToList<RoleInfo>();
        }

#endregion

#region Public Static Methods

         public static string FormatRemoteAddr(string remoteAddr, bool includeBrackets)
         {
            string result = "";
            if (!string.IsNullOrEmpty(remoteAddr))
            {
                IPAddress address = IPAddress.None;
                if (IPAddress.TryParse(remoteAddr, out address))
                {
                    result = address.ToString();
                } 
            }
            return includeBrackets ? "[" + result + "]" : result;
         }       
 
        public static string GetUserIPAddress(bool includeBrackets)
        {
            HttpContext currentContext = HttpContext.Current;

            if (currentContext == null)
            {
                return string.Empty;
            }
            else
            {
                string ipAddress = currentContext.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                string remoteAddr = null;

                if (!string.IsNullOrEmpty(ipAddress))
                {
                    string[] addresses = ipAddress.Split(',');
                    if (addresses.Length != 0)
                    {
                        remoteAddr = addresses[0];
                    }
                }

                if (remoteAddr == null) remoteAddr = currentContext.Request.ServerVariables["REMOTE_ADDR"];
                
                return FormatRemoteAddr(remoteAddr, includeBrackets);
            }
        }
            

        public static string EncryptString(string plainText, ref string salt)
        {
            return EncryptString(plainText, Config.GetDecryptionkey(), ref salt);
        }

        public static string DecryptString(string encryptedText, string salt)
        {
            return DecryptString(encryptedText, Config.GetDecryptionkey(), salt);
        }

        public static string EncryptString(string plainText, string passPhrase, ref string salt)
        {          
            if (string.IsNullOrEmpty(plainText)) throw new ArgumentException("Cannot encrypt a null or empty string", "plainText");
            if (string.IsNullOrEmpty(passPhrase)) throw new ArgumentException("Cannot encrypt using a null or empty passPhrase string", "passPhrase");

            byte[] saltBytes;

            if (string.IsNullOrEmpty(salt))
            {
                saltBytes = GenerateRandomBytes(SaltLength);
            }
            else
            {
                saltBytes = Convert.FromBase64String(salt); // Extract the salt
            }
             
            byte[] ivBytes = GenerateRandomBytes(16);                           // 128-bit block size for AES
            byte[] keyBytes = DeriveKeyFromPassPhrase(passPhrase, saltBytes);   // Derive the key from the passphrase
            byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);

            // Encrypt the plainText and append it to the ivBytes  
            byte[] encryptedBytes = ivBytes.Concat(DoCryptoOperation(plainTextBytes, keyBytes, ivBytes, true)).ToArray();
                        
            // Return the formatted string and the salt
            salt = Convert.ToBase64String(saltBytes);
            return Convert.ToBase64String(encryptedBytes);
        }

        public static string DecryptString(string encryptedText, string passPhrase, string salt)
        {
            if (string.IsNullOrEmpty(encryptedText)) return string.Empty;
            if (string.IsNullOrEmpty(passPhrase) || string.IsNullOrEmpty(salt)) return encryptedText;

            byte[] encryptedBytes = Convert.FromBase64String(encryptedText); // Extract the ivBytes and the encryptedTextBytes
            byte[] ivBytes = encryptedBytes.Take(16).ToArray();

            int len = encryptedBytes.Length - 16;
            byte[] plainTextBytes = new byte[len];
            Array.Copy(encryptedBytes, 16, plainTextBytes, 0, len);
            byte[] saltBytes = Convert.FromBase64String(salt); // Extract the salt

            // Derive the key from the supplied passphrase and extracted salt 
            byte[] keyBytes = DeriveKeyFromPassPhrase(passPhrase, saltBytes);
            
            // Decrypt and return string

            return Encoding.UTF8.GetString(DoCryptoOperation(plainTextBytes, keyBytes, ivBytes, false));
        }

#endregion

#region Private Static Methods

        private static byte[] DeriveKeyFromPassPhrase(string passPhrase, byte[] saltBytes, int iterations = 2000)
        {
            var pbkdf2 = new Rfc2898DeriveBytes(passPhrase, saltBytes, iterations);
            return pbkdf2.GetBytes(KeyLengthBits / 8);
        }

        private static byte[] GenerateRandomBytes(int length)
        {
            byte[] bytes = new byte[length];
            new RNGCryptoServiceProvider().GetBytes(bytes);
            return bytes;
        }

        // This function does both encryption and decryption, depending on the value of the "encrypt" parameter 

        private static byte[] DoCryptoOperation(byte[] inputBytes, byte[] keyBytes, byte[] ivBytes, bool encrypt)
        {
            byte[] outputBytes;
            using (var aes = new AesCryptoServiceProvider())
                using (var ms = new MemoryStream())
                {
                    var cryptoTransform = encrypt ? aes.CreateEncryptor(keyBytes, ivBytes) : aes.CreateDecryptor(keyBytes, ivBytes);
                    using (var cs = new CryptoStream(ms, cryptoTransform, CryptoStreamMode.Write))
                        cs.Write(inputBytes, 0, inputBytes.Length);
                    outputBytes = ms.ToArray();
                }
            return outputBytes;
        }

#endregion

    }
}