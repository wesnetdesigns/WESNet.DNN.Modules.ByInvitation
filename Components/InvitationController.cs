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
using System.Xml;
using System.Xml.Linq;

using DotNetNuke;
using DotNetNuke.ComponentModel;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Services.Social.Notifications;
using DotNetNuke.Services.Tokens;
using DotNetNuke.Services.Scheduling;
using DotNetNuke.Security.Membership;

namespace WESNet.DNN.Modules.ByInvitation
{
    public class InvitationController : DotNetNuke.Entities.Modules.IUpgradeable
    {

        private const string _configurationCacheKey = "WESNet_ByInvitation_Config-{0}";
        private const string _roleBasedLimitsCacheKey = "WESNet_ByInvitation_RBL-{0}";
        private const int _cacheTimeOut = 10;  //minutes
        private static IDataProvider _dataProvider;

        private static string[] validInvitationActions = new string[] { "sent", "approved", "disapproved", "resent", "retracted", "unretracted", "declined", "lockedout", "unlocked", "extended", "accepted", "archived", "unarchived", "pwdfailed" };
        private static string[] validBulkImportActions = new string[] { "appendlogentry", "readstarted", "readbatchfinished", "readfinished", "sendstarted", "sendbatchfinished", "sendfinished", "aborted", "canceled", "finished" };
        
        static InvitationController()
        {
            _dataProvider = ComponentFactory.GetComponent<IDataProvider>();
            if (_dataProvider != null) return;

            // get the provider configuration based on the type
            var defaultprovider = DotNetNuke.Data.DataProvider.Instance().DefaultProviderName;
            const string dataProviderNamespace = "WESNet.DNN.Modules.ByInvitation";

            if (defaultprovider == "SqlDataProvider")
            {
                _dataProvider = new SqlDataProvider();
            }
            else
            {
                var providerType = dataProviderNamespace + "." + defaultprovider;
                _dataProvider = (IDataProvider)DotNetNuke.Framework.Reflection.CreateObject(providerType, providerType, true);
            }

            ComponentFactory.RegisterComponentInstance<IDataProvider>(_dataProvider);
        }

        public InvitationController()
        {

        }

        public InvitationController(IDataProvider dataProvider)
        {
			_dataProvider = dataProvider;
		}
        
#region Configuration       

        public static void UpdateConfiguration(Configuration objConfiguration, int modifiedByUserID)
        {
            _dataProvider.UpdateConfiguration(objConfiguration, modifiedByUserID);
            ClearConfigurationCache(objConfiguration.PortalId);
        }

        public static Configuration GetConfiguration(int portalId)
        {
            CacheItemArgs args = new CacheItemArgs(GetConfigurationCacheKey(portalId), _cacheTimeOut,System.Web.Caching.CacheItemPriority.Normal, portalId);
            return CBO.GetCachedObject<Configuration>(args, ConfigurationCacheItemExpired);
        }

#endregion

#region LocalizedConfiguration

        public static int UpdateLocalizedConfiguration(LocalizedConfiguration objLocalizedConfiguration, int modifiedByUserID)
        {
            int localizedConfigurationID = _dataProvider.UpdateLocalizedConfiguration(objLocalizedConfiguration, modifiedByUserID);
            ClearConfigurationCache(objLocalizedConfiguration.PortalId);
            return localizedConfigurationID;
        }

        public static void DeleteLocalizedConfiguration(int localizedConfigurationID)
        {
            _dataProvider.DeleteLocalizedConfiguration(localizedConfigurationID);
        }

        public static LocalizedConfiguration GetLocalizedConfiguration(int localizedConfigurationID)
        {
            return CBO.FillObject<LocalizedConfiguration>(_dataProvider.GetLocalizedConfiguration(localizedConfigurationID));
        }

        public static LocalizedConfiguration GetLocalizedConfigurationByPortalAndCulture(int portalId, string cultureCode)
        {
            return CBO.FillObject<LocalizedConfiguration>(_dataProvider.GetLocalizedConfigurationByPortalAndCulture(portalId, cultureCode));
        }

        public static Dictionary<string, LocalizedConfiguration> GetLocalizedConfigurations(int portalId)
        {
            return CBO.FillDictionary<string, LocalizedConfiguration>("CultureCode", _dataProvider.GetLocalizedConfigurations(portalId));
        }

#endregion

#region RoleBasedLimits

        public static int UpdateRoleBasedLimits(RoleBasedLimits objRoleBasedLimits, int modifiedByUserID)
        {
            int roleBasedLimitID = _dataProvider.UpdateRoleBasedLimits(objRoleBasedLimits, modifiedByUserID);
            ClearRoleBasedLimitsCollectionCache(objRoleBasedLimits.PortalId);
            return roleBasedLimitID;
        }

        public static void DeleteRoleBasedLimits(int roleBasedLimitID, int portalId)
        {
            _dataProvider.DeleteRoleBasedLimits(roleBasedLimitID);
            ClearRoleBasedLimitsCollectionCache(portalId);

        }

        public static RoleBasedLimits GetRoleBasedLimits(int portalId, int roleID)
        {
            return CBO.FillObject<RoleBasedLimits>(_dataProvider.GetRoleBasedLimits(portalId, roleID));
        }

        public static Dictionary<int, RoleBasedLimits> GetRoleBasedLimitsCollection(int portalId)
        {
            CacheItemArgs args = new CacheItemArgs(GetRoleBasedLimitsCacheKey(portalId), _cacheTimeOut, System.Web.Caching.CacheItemPriority.Normal, portalId);
            return CBO.GetCachedObject<Dictionary<int,RoleBasedLimits>>(args, RoleBasedLimitsCollectionCacheItemExpired);
        }

#endregion

#region Invitation

        public static int UpdateInvitation(InvitationInfo objInvitation, int userID)
        {
            return (int)_dataProvider.UpdateInvitation(objInvitation, userID);
        }

        public static InvitationInfo UpdateInvitationStatus(int invitationID, string action, int modifiedByUserID, string reasonDeclined, int assignedUserID)
        {
            action = action.ToLowerInvariant();
            if (!validInvitationActions.Contains(action)) throw new ArgumentException("Invitation action must be one of: " + validInvitationActions.ToString(), "action");

            return CBO.FillObject <InvitationInfo>(_dataProvider.UpdateInvitationStatus(invitationID, action, modifiedByUserID, DateTime.MinValue, reasonDeclined, assignedUserID));
        }

        public static InvitationInfo UpdateInvitationStatus(int invitationID, string action, int modifiedByUserID)
        {
            action = action.ToLowerInvariant();
            var validActionsSubset = validInvitationActions.Except(new []{"extended", "lockedout", "accepted", "declined"});
            if (!validActionsSubset.Contains(action)) throw new ArgumentException("Action must be one of: " + validActionsSubset.ToString(), "action");
            
            return CBO.FillObject <InvitationInfo>(_dataProvider.UpdateInvitationStatus(invitationID, action, modifiedByUserID, DateTime.MinValue, Null.NullString, -1));
        }

        public static InvitationInfo UpdateInvitationStatus(int invitationID, string action, int modifiedByUserID, DateTime newDateTimeValue)
        {
            action = action.ToLowerInvariant();
            var validActionsSubset = validInvitationActions.Intersect(new[] { "approved", "extended", "lockedout" });
            if (!validActionsSubset.Contains(action)) throw new ArgumentException("Action must be one of: " + validActionsSubset.ToString(), "action");

            return CBO.FillObject <InvitationInfo>(_dataProvider.UpdateInvitationStatus(invitationID, action, modifiedByUserID, newDateTimeValue, Null.NullString, -1));
        }

        public static void DeleteInvitation(int invitationID)
        {
            _dataProvider.DeleteInvitation(invitationID);
        }

        public static void DeleteInvitations(int portalID, int invitedByUserID)
        {
            _dataProvider.DeleteInvitations(portalID, invitedByUserID);
        }

        public static InvitationInfo GetInvitation(int invitationID)
        {
            return CBO.FillObject<InvitationInfo>(_dataProvider.GetInvitation(invitationID));
        }

        public static InvitationInfo GetInvitationByRecipientEmail(int portalId, string recipientEmail)
        {
            return CBO.FillObject<InvitationInfo>(_dataProvider.GetInvitationByRecipientEmail(portalId, recipientEmail));
        }

        public static InvitationInfo GetInvitationByAssignedUsername(int portalId, string assignedUsername)
        {
            return CBO.FillObject<InvitationInfo>(_dataProvider.GetInvitationByAssignedUsername(portalId, assignedUsername));
        }

        public static InvitationInfo GetInvitationByPortalAndRSVPCode(int portalId, string rsvpCode)
        {
            return CBO.FillObject<InvitationInfo>(_dataProvider.GetInvitationByPortalAndRSVPCode(portalId, rsvpCode));
        }

        public static IEnumerable<InvitationInfo> GetInvitations(int portalId)
        {
            return CBO.FillCollection<InvitationInfo>(_dataProvider.GetInvitations(portalId));
        }

        public static IEnumerable<InvitationInfo> GetInvitations(int portalId, bool hasStatus, InvitationStatus invitationStatus)
        {
            if (hasStatus)
            {
                return CBO.FillCollection<InvitationInfo>(_dataProvider.GetInvitations(portalId)).Where(i => i.HasStatus(invitationStatus));
            }
            else
            {
                return CBO.FillCollection<InvitationInfo>(_dataProvider.GetInvitations(portalId)).Where(i => i.HasNotStatus(invitationStatus));
            }
        }

        public static IEnumerable<InvitationInfo> GetBulkImportedInvitationsNotSent(int bulkImportID, int sendBatchSize, ref int totalInvitationsToSend)
        {
            return CBO.FillCollection<InvitationInfo>(_dataProvider.GetBulkImportedInvitationsNotSent(bulkImportID, sendBatchSize), ref totalInvitationsToSend);
        }

        public static IEnumerable<InvitationInfo> GetInvitationsByInvitingUser(int portalId, int invitingUserID, DateTime startDate, DateTime endDate, bool includeArchivedInvitations)
        {
            return CBO.FillCollection<InvitationInfo>(_dataProvider.GetInvitationsByInvitingUser(portalId, invitingUserID, startDate, endDate, includeArchivedInvitations));
        }

        public static IEnumerable<InvitationInfo> GetInvitationsByInvitingUserIPAddr(int portalId, string invitingUserIPAddr)
        {
            return CBO.FillCollection<InvitationInfo>(_dataProvider.GetInvitationsByInvitingUserIPAddr(portalId, invitingUserIPAddr));
        }

        public static IEnumerable<InvitationInfo> GetInvitationsByInvitingUserEmail(int portalId, string invitingUserEmail)
        {
            return GetInvitationsByInvitingUserEmail(portalId, invitingUserEmail, DateTime.MinValue, DateTime.MinValue, false);
        }

        public static IEnumerable<InvitationInfo> GetInvitationsByInvitingUserEmail(int portalId, string invitingUserEmail, DateTime startDate, DateTime endDate, bool includeArchived)
        {
            return CBO.FillCollection<InvitationInfo>(_dataProvider.GetInvitationsByInvitingUserEmail(portalId, invitingUserEmail, startDate, endDate, includeArchived));
        }

        public static int GetInvitationCount(int portalId, int invitingUserID, DateTime startDate, DateTime endDate)
        {
            return _dataProvider.GetInvitationCount(portalId, invitingUserID, startDate, endDate);
        }

        public static InvitingUserStatsInfo GetInvitingUserStats(int portalId, DateTime startDate, DateTime endDate, bool includeArchivedInvitations, string invitingUser)
        {
            return CBO.FillObject<InvitingUserStatsInfo>(_dataProvider.GetInvitingUserStats(portalId, startDate, endDate, includeArchivedInvitations, invitingUser));
        }

        public static List<InvitingUser>GetInvitingUsers(int portalId, DateTime startDate, DateTime endDate, bool includeArchivedInvitations)
        {
            return CBO.FillCollection<InvitingUser>(_dataProvider.GetInvitingUsers(portalId, startDate, endDate, includeArchivedInvitations));
        }

#endregion

#region AssignableRole

        public static int UpdateAssignableRole(AssignableRoleInfo objAssignableRole, int modifiedByUserID)
        {
            return (int)_dataProvider.UpdateAssignableRole(objAssignableRole, modifiedByUserID);
        }

        public static void DeleteAssignableRole(int assignableRoleID)
        {
            _dataProvider.DeleteAssignableRole(assignableRoleID);
        }

        public static AssignableRoleInfo GetAssignableRole(int assignableRoleID)
        {
            return CBO.FillObject<AssignableRoleInfo>(_dataProvider.GetAssignableRole(assignableRoleID));
        }
        
        public static List<AssignableRoleInfo> GetAssignableRolesByInvitation(int invitationID)
        {
            return CBO.FillCollection<AssignableRoleInfo>(_dataProvider.GetAssignableRolesByInvitation(invitationID));
        }

#endregion

#region BatchImport
        public static int SaveBulkImport(BulkImportInfo obj)
        {
            return (int)_dataProvider.SaveBulkImport(obj);
        }

        public static void UpdateBulkImportStatus(BulkImportInfo bulkImport, string action, int recordsRead, int recordsSkipped, int invitationsCreated, int invitationsSent, int errorCount, XElement logEntry)
        {
            if (!validBulkImportActions.Contains(action)) throw new ArgumentException(string.Format("Invalid action: {0} - UpdateBulkImportStatus workflow action must be one of: {1}", action, validBulkImportActions));
            
            logEntry.SetAttributeValue("action", action);

            _dataProvider.UpdateBulkImportStatus(bulkImport.BulkImportID, action, bulkImport.BatchNumber, recordsRead, recordsSkipped, invitationsCreated, invitationsSent, errorCount, logEntry);
            
            var tmp = GetBulkImport(bulkImport.BulkImportID);
            bulkImport.RecordsRead = tmp.RecordsRead;
            bulkImport.RecordsSkipped = tmp.RecordsSkipped;
            bulkImport.InvitationsCreated = tmp.InvitationsCreated;
            bulkImport.InvitationsSent = tmp.InvitationsSent;
            bulkImport.ErrorCount = tmp.ErrorCount;
            bulkImport.ReadStartedOnDate = tmp.ReadStartedOnDate;
            bulkImport.ReadBatchFinishedOnDate = tmp.ReadBatchFinishedOnDate;
            bulkImport.ReadFinishedOnDate = tmp.ReadFinishedOnDate;
            bulkImport.SendStartedOnDate = tmp.SendStartedOnDate;
            bulkImport.SendBatchFinishedOnDate = tmp.SendBatchFinishedOnDate;
            bulkImport.SendFinishedOnDate = tmp.SendFinishedOnDate;
            bulkImport.FinishedOnDate = tmp.FinishedOnDate;
            bulkImport.AbortedOnDate = tmp.FinishedOnDate;
            bulkImport.CanceledOnDate = tmp.CanceledOnDate;
        }
       
        public static void DeleteBulkImport(int portalId, int importedByUserID, int bulkImportID)
        {
            _dataProvider.DeleteBulkImport(portalId, importedByUserID, bulkImportID);
        }

        public static BulkImportInfo GetBulkImport(int bulkImportID)
        {
            return CBO.FillObject<BulkImportInfo>(_dataProvider.GetBulkImport(bulkImportID));
        }

        public static List<BulkImportInfo> GetBulkImports(int portalId, int importedByUserID, bool excludeAbortedOrFinished)
        {
            return CBO.FillCollection<BulkImportInfo>(_dataProvider.GetBulkImports(portalId, importedByUserID, excludeAbortedOrFinished));
        }

        #endregion

        #region Private Methods

        private static string GetConfigurationCacheKey(int portalId)
        {
            return string.Format(_configurationCacheKey, portalId);
        }

        private static string GetRoleBasedLimitsCacheKey(int portalId)
        {
            return string.Format(_roleBasedLimitsCacheKey, portalId);
        }

        private static Configuration ConfigurationCacheItemExpired (CacheItemArgs args)
        {
            string cacheKey = args.CacheKey;
            int portalId = (int) args.Params[0];
            Configuration config =  CBO.FillObject<Configuration>(_dataProvider.GetConfiguration(portalId));
            if (config == null)
            {
                config = new Configuration(portalId);
            }
            return config;
        }

        private static Dictionary<int,RoleBasedLimits> RoleBasedLimitsCollectionCacheItemExpired (CacheItemArgs args)
        {
            string cacheKey = args.CacheKey;
            int portalId = (int)args.Params[0];
            var roleBasedLimitsCollection = CBO.FillDictionary<int, RoleBasedLimits>("RoleID", _dataProvider.GetRoleBasedLimitsCollection(portalId));
            if (roleBasedLimitsCollection == null) roleBasedLimitsCollection = new Dictionary<int, RoleBasedLimits>();
            if (roleBasedLimitsCollection.Count == 0)
            {
                var adminRoleID = PortalController.Instance.GetCurrentPortalSettings().AdministratorRoleId;
                var adminAssignableRoles = Utilities.GetPortalRoles(portalId);
                var adminRoleBasedLimits = new RoleBasedLimits(portalId, adminRoleID, EscapedString.Combine(adminAssignableRoles));
                UpdateRoleBasedLimits(adminRoleBasedLimits, -1);
                roleBasedLimitsCollection = CBO.FillDictionary<int, RoleBasedLimits>("RoleID", _dataProvider.GetRoleBasedLimitsCollection(portalId));
            }
            return roleBasedLimitsCollection;
        }

        private static void ClearConfigurationCache(int portalId)
        {
            string cacheKey = GetConfigurationCacheKey(portalId);
            DataCache.RemoveCache(cacheKey);
        }

        private static void ClearRoleBasedLimitsCollectionCache(int portalId)
        {
            string cacheKey = GetRoleBasedLimitsCacheKey(portalId);
            DataCache.RemoveCache(cacheKey);
        }

#endregion
        
#region IUpgradeable Members

        public string UpgradeModule(string Version)
        {
            if (Version == "01.00.00")
            {
                NotificationsHelper.AddNotificationTypes();

                // Install and enable ByInvitationScheduler ScheduleClient

                SchedulingController.AddSchedule("WESNet.DNN.Modules.ByInvitation.InvitationScheduler, WESNet.DNN.Modules.ByInvitation",
                                                 10, "m", 5, "m", 10, "", false, true, "", null, "By Invitation Scheduler", DateTime.MinValue);
                                      
            }
            else if (Version == "01.00.01")
            {
                // Fix issue of incorrect API Call in NotificationTypeActions for Notification Type "ModerationRequested"
                var notificationType = NotificationsController.Instance.GetNotificationType(Configuration.ModuleName + "_" + Consts.ModerationRequestedNotification.Name);
                var actions = NotificationsController.Instance.GetNotificationTypeActions(notificationType.NotificationTypeId);
                foreach (var action in actions)
                {
                    NotificationsController.Instance.DeleteNotificationTypeAction(action.NotificationTypeActionId);
                    action.APICall = Configuration.ModulePath + "API/ByInvitationService/" + action.NameResourceKey.Replace(".Name", "");
                }
                NotificationsController.Instance.SetNotificationTypeActions(actions, notificationType.NotificationTypeId);
            }
            
            return Version;
        }

#endregion

    }
}