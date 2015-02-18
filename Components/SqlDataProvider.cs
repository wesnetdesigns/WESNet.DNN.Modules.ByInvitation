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
using System.Data;
using System.Xml;
using System.Xml.Linq;
using Microsoft.ApplicationBlocks.Data;

using DotNetNuke;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Security.Membership;

namespace WESNet.DNN.Modules.ByInvitation
{
    public class SqlDataProvider : IDataProvider
    {
        #region Private Members

        private const string CompanyQualifier = "WESNet_";
        private const string ModuleQualifier = "ByInvitation_";
        private string _connectionString = String.Empty;
        private string _databaseOwner = String.Empty;
        private string _objectQualifier = String.Empty;

        #endregion

        #region Properties

        public string ConnectionString
        {
            get
            {
                return string.IsNullOrEmpty(_connectionString) ? DotNetNuke.Data.DataProvider.Instance().ConnectionString : _connectionString;
            }
            set { _connectionString = value; }
        }

        public string DatabaseOwner
        {
            get
            {
                return string.IsNullOrEmpty(_databaseOwner) ? DotNetNuke.Data.DataProvider.Instance().DatabaseOwner : _databaseOwner;
            }
            set { _databaseOwner = value; }
        }

        public string ObjectQualifier
        {
            get
            {
                return string.IsNullOrEmpty(_objectQualifier) ? DotNetNuke.Data.DataProvider.Instance().ObjectQualifier : _objectQualifier;
            }
            set { _objectQualifier = value; }
        }

        #endregion

        #region Private Methods

        private static object GetNull(object field)
        {
            return Null.GetNull(field, DBNull.Value);
        }

        private static object GetNullTicks(TimeSpan value)
        {
            if (value.Ticks == 0L) return DBNull.Value;

            return value.Ticks;
        }

        private static object GetNullableDateTime(DateTime? field)
        {
            if (!field.HasValue) return DBNull.Value;
            return field.Value;
        }

        private string GetFullyQualifiedName(string name)
        {
            return DatabaseOwner + ObjectQualifier + CompanyQualifier + ModuleQualifier + name;
        }

        #endregion

#region IDataProvider Implementation

   #region Configuration

        public void UpdateConfiguration(Configuration objConfiguration, int ModifiedByUserID)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString, GetFullyQualifiedName("UpdateConfiguration"), objConfiguration.PortalId, GetNullTicks(objConfiguration.ValidityPeriod),
                                        objConfiguration.EnableAutoResend, GetNull(objConfiguration.MaxResends), GetNullTicks(objConfiguration.ResendInterval),
                                        objConfiguration.AutoDeleteArchiveExpiredInvitations, GetNull(objConfiguration.DaysPastExpiration), objConfiguration.RequireRecipientEmailConfirm, 
                                        objConfiguration.EnablePersonalNote, objConfiguration.TemporaryPasswordMode, objConfiguration.RequireTemporaryPasswordConfirm,
                                        objConfiguration.EnableInvitationCaptcha, objConfiguration.EnableAcceptanceCaptcha, objConfiguration.EnableCaptchaAudio,
                                        objConfiguration.CaptchaIsCaseInsensitive, objConfiguration.CaptchaLineNoise, objConfiguration.CaptchaBackgroundNoise,
                                        objConfiguration.InvitationFiltering, GetNullTicks(objConfiguration.InvitationFilteringInterval), GetNull(objConfiguration.MaxFailedAttempts),
                                        GetNullTicks(objConfiguration.LockoutDuration), objConfiguration.RequireModeration, GetNull(objConfiguration.ProcessorPageName), objConfiguration.AllowUsernameModification,
                                        objConfiguration.AllowFirstLastNameModification, objConfiguration.AllowDisplayNameModification, objConfiguration.RequireTemporaryPasswordEntry,
                                        objConfiguration.RequirePasswordChangeOnAcceptance,  objConfiguration.RequirePasswordConfirmOnChange, objConfiguration.EnableReasonDeclinedField,
                                        objConfiguration.SuspendInvitationProcessing, GetNull(objConfiguration.SendFrom), GetNull(objConfiguration.SendCopiesTo),
                                        GetNull(objConfiguration.SendCopiesToRoles), objConfiguration.EnabledInvitingUserNotifications, objConfiguration.EnabledAdminUserNotifications, ModifiedByUserID);
        }

        public IDataReader GetConfiguration(int portalId)
        {
            return SqlHelper.ExecuteReader(ConnectionString, GetFullyQualifiedName("GetConfiguration"), portalId);
        }

   #endregion

   #region LocalizedConfiguration

        public int UpdateLocalizedConfiguration(LocalizedConfiguration objLocalizedConfiguration, int modifiedByUserID)
        {
            objLocalizedConfiguration.LocalizedConfigurationID = Convert.ToInt32(SqlHelper.ExecuteScalar(ConnectionString, GetFullyQualifiedName("UpdateLocalizedConfiguration"),
                    objLocalizedConfiguration.LocalizedConfigurationID, objLocalizedConfiguration.PortalId, objLocalizedConfiguration.CultureCode, objLocalizedConfiguration.SendInvitationHtml,
                    objLocalizedConfiguration.SendInvitationButtonText, GetNull(objLocalizedConfiguration.SendInvitationButtonImage), GetNull(objLocalizedConfiguration.ImageButtonSize.Width),
                    GetNull(objLocalizedConfiguration.ImageButtonSize.Height),objLocalizedConfiguration.InvitationSubject, objLocalizedConfiguration.InvitationBody,
                    GetNull(objLocalizedConfiguration.EmailRegex), modifiedByUserID));
            return objLocalizedConfiguration.LocalizedConfigurationID;
        }

        public void DeleteLocalizedConfiguration(int localizedConfigurationID)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString, GetFullyQualifiedName("DeleteLocalizedConfiguration"), localizedConfigurationID);
        }

        public IDataReader GetLocalizedConfiguration(int localizedConfigurationID)
        {
            return SqlHelper.ExecuteReader(ConnectionString, GetFullyQualifiedName("GetLocalizedConfiguration"), localizedConfigurationID);
        }

        public IDataReader GetLocalizedConfigurationByPortalAndCulture(int portalId, string cultureCode)
        {
            return SqlHelper.ExecuteReader(ConnectionString, GetFullyQualifiedName("GetLocalizedConfigurationByPortalAndCulture"), portalId, cultureCode);
        }

        public IDataReader GetLocalizedConfigurations(int portalId)
        {
            return SqlHelper.ExecuteReader(ConnectionString, GetFullyQualifiedName("GetLocalizedConfigurations"), portalId);
        }

   #endregion

   #region RoleBasedLimits

        public int UpdateRoleBasedLimits(RoleBasedLimits objRoleBasedLimits, int modifiedByUserID)
        {
            objRoleBasedLimits.RoleBasedLimitID = Convert.ToInt32(SqlHelper.ExecuteScalar(ConnectionString, GetFullyQualifiedName("UpdateRoleBasedLimits"), objRoleBasedLimits.RoleBasedLimitID,
                    objRoleBasedLimits.PortalId, objRoleBasedLimits.RoleID, GetNull(objRoleBasedLimits.AllocatedInvitations), GetNullTicks(objRoleBasedLimits.AllocationPeriod),
                    GetNull(objRoleBasedLimits.AssignableRoles), modifiedByUserID));
            return objRoleBasedLimits.RoleBasedLimitID;
        }

        public void DeleteRoleBasedLimits(int roleBasedLimitID)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString, GetFullyQualifiedName("DeleteRoleBasedLimits"), roleBasedLimitID);
        }

        public IDataReader GetRoleBasedLimits(int portalId, int roleID)
        {
            return SqlHelper.ExecuteReader(ConnectionString, GetFullyQualifiedName("GetRoleBasedLimits"), portalId, roleID);
        }

        public System.Data.IDataReader GetRoleBasedLimitsCollection(int portalId)
        {
            return SqlHelper.ExecuteReader(ConnectionString, GetFullyQualifiedName("GetRoleBasedLimitsCollection"), portalId);
        }

   #endregion

   #region Invitation

    public int UpdateInvitation(InvitationInfo objInvitation, int userID)
        {
            objInvitation.InvitationID = Convert.ToInt32(SqlHelper.ExecuteScalar(ConnectionString, GetFullyQualifiedName("UpdateInvitation"), objInvitation.InvitationID, objInvitation.PortalId,
                    objInvitation.OriginatingContext, GetNull(objInvitation.BulkImportID), GetNull(objInvitation.RecipientCultureCode), objInvitation.RecipientEmail, objInvitation.RecipientFirstName,
                    objInvitation.RecipientLastName, GetNull(objInvitation.AssignedUsername), GetNull(objInvitation.AssignedDisplayName), GetNull(objInvitation.TemporaryPassword),
                    GetNull(objInvitation.TemporaryPasswordSalt), GetNull(objInvitation.PersonalNote), GetNull(objInvitation.RedirectOnFirstLogin), objInvitation.RSVPCode, objInvitation.InvitedOnDate, 
                    GetNull(objInvitation.ApprovedOnDate), GetNull(objInvitation.ApprovedByUserID), GetNull(objInvitation.ExpiresOnDate), GetNull(objInvitation.InvitedByUserID), 
                    GetNull(objInvitation.InvitedByUserIPAddr), GetNull(objInvitation.InvitedByUserFullName), GetNull(objInvitation.InvitedByUserEmail), userID));
            return objInvitation.InvitationID;
        }

        public IDataReader UpdateInvitationStatus(int invitationID, string action, int modifiedByUserID, DateTime newDateTimeValue, string reasonDeclined, int assignedUserID)
        {
            return SqlHelper.ExecuteReader(ConnectionString, GetFullyQualifiedName("UpdateInvitationStatus"), invitationID, action, modifiedByUserID, GetNull(newDateTimeValue), GetNull(reasonDeclined), GetNull(assignedUserID));
        }

        public void DeleteInvitation(int invitationID)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString, GetFullyQualifiedName("DeleteInvitation"), invitationID);
        }

        public void DeleteInvitations(int portalId, int invitingUserID)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString, GetFullyQualifiedName("DeleteInvitations"), portalId, GetNull(invitingUserID));
        }

        public void DeleteAcceptedOrExpiredInvitations(int portalId, int invitingUserID)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString, GetFullyQualifiedName("DeleteAcceptedOrExpiredInvitations"), portalId, GetNull(invitingUserID));
        }

        public IDataReader GetInvitation(int invitationId)
        {
            return SqlHelper.ExecuteReader(ConnectionString, GetFullyQualifiedName("GetInvitation"), invitationId);
        }

        public IDataReader GetInvitationByRecipientEmail(int portalId, string recipientEmail)
        {
            return SqlHelper.ExecuteReader(ConnectionString, GetFullyQualifiedName("GetInvitationByRecipientEmail"), portalId, recipientEmail);
        }

        public IDataReader GetInvitationByAssignedUsername(int portalId, string assignedUsername)
        {
            return SqlHelper.ExecuteReader(ConnectionString, GetFullyQualifiedName("GetInvitationByAssignedUsername"), portalId, assignedUsername);
        }

        public IDataReader GetInvitationByPortalAndRSVPCode(int portalId, string rsvpCode)
        {
            return SqlHelper.ExecuteReader(ConnectionString, GetFullyQualifiedName("GetInvitationByPortalAndRSVPCode"), portalId, new Guid(rsvpCode));
        }

        public IDataReader GetInvitations(int portalId)
        {
            return SqlHelper.ExecuteReader(ConnectionString, GetFullyQualifiedName("GetInvitations"), GetNull(portalId));
        }

        public IDataReader GetBulkImportedInvitationsNotSent(int bulkImportID, int batchSize)
        {
            return SqlHelper.ExecuteReader(ConnectionString, GetFullyQualifiedName("GetBulkImportedInvitationsNotSent"), bulkImportID, batchSize);
        }

        public IDataReader GetInvitationsByInvitingUser(int portalId, int invitingUserID, DateTime startDate, DateTime endDate, bool includeArchived)
        {
            return SqlHelper.ExecuteReader(ConnectionString, GetFullyQualifiedName("GetInvitationsByInvitingUser"), portalId, GetNull(invitingUserID), GetNull(startDate), GetNull(endDate), includeArchived);
        }

        public IDataReader GetInvitationsByInvitingUserIPAddr(int portalId, string invitingUserIPAddr)
        {
            return SqlHelper.ExecuteReader(ConnectionString, GetFullyQualifiedName("GetInvitationsByInvitingUserIPAddr"), portalId, GetNull(invitingUserIPAddr));
        }

        public IDataReader GetInvitationsByInvitingUserEmail(int portalId, string invitingUserEmail, DateTime startDate, DateTime endDate, bool includeArchived)
        {
            return SqlHelper.ExecuteReader(ConnectionString, GetFullyQualifiedName("GetInvitationsByInvitingUserEmail"), portalId, GetNull(invitingUserEmail), GetNull(startDate), GetNull(endDate), includeArchived);
        }

        public int GetInvitationCount(int portalId, int invitingUserID, DateTime startDate, DateTime endDate)
        {
            return Convert.ToInt32(SqlHelper.ExecuteScalar(ConnectionString, GetFullyQualifiedName("GetInvitationCount"), portalId, GetNull(invitingUserID), GetNull(startDate), GetNull(endDate)));
        }

        public IDataReader GetInvitingUserStats(int portalId, DateTime startDate, DateTime endDate, bool includeArchivedInvitations, string invitingUser)
        {
            return SqlHelper.ExecuteReader(ConnectionString, GetFullyQualifiedName("GetInvitingUserStats"), portalId, GetNull(startDate), GetNull(endDate), includeArchivedInvitations, invitingUser);
        }

        public IDataReader GetInvitingUsers(int portalId, DateTime startDate, DateTime endDate, bool includeArchived)
        {
            return SqlHelper.ExecuteReader(ConnectionString, GetFullyQualifiedName("GetInvitingUsers"), portalId, GetNull(startDate), GetNull(endDate), includeArchived);
        }

   #endregion

   #region AssignableRoles

        public int UpdateAssignableRole(AssignableRoleInfo objAssignableRole, int modifiedByUserID)
        {
            objAssignableRole.AssignableRoleID = Convert.ToInt32(SqlHelper.ExecuteScalar(ConnectionString, GetFullyQualifiedName("UpdateAssignableRole"), objAssignableRole.AssignableRoleID,
                    objAssignableRole.InvitationID, objAssignableRole.RoleID, GetNullableDateTime(objAssignableRole.EffectiveDate), GetNullableDateTime(objAssignableRole.ExpiryDate), modifiedByUserID));
            return objAssignableRole.AssignableRoleID;
        }

        public void DeleteAssignableRole(int assignableRoleID)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString, GetFullyQualifiedName("DeleteAssignableRole"), assignableRoleID);
        }

        public IDataReader GetAssignableRole(int assignableRoleID)
        {
            return SqlHelper.ExecuteReader(ConnectionString, GetFullyQualifiedName("GetAssignableRole"), assignableRoleID);
        }

        public IDataReader GetAssignableRolesByInvitation(int invitationID)
        {
            return SqlHelper.ExecuteReader(ConnectionString, GetFullyQualifiedName("GetAssignableRolesByInvitation"), invitationID);
        }

   #endregion

   #region Bulk Import

        public int SaveBulkImport(BulkImportInfo obj)
        {
            obj.BulkImportID = Convert.ToInt32(SqlHelper.ExecuteScalar(ConnectionString, GetFullyQualifiedName("SaveBulkImport"),
                obj.BulkImportID, obj.PortalID, obj.TabID, obj.ModuleID, obj.ImportedByUserID, GetNull(obj.ImportedByUserIPAddr), GetNull(obj.WizardStepLastCompleted), obj.ImportSourceType,
                GetNull(obj.ImportFilename), GetNull(obj.ConnectionStringKey), GetNull(obj.DatabaseConnectionString), GetNull(obj.DatabaseTableName), GetNull(obj.OrderBy), obj.CSVDelimiter,
                obj.FirstRowIsHeader, GetNull(obj.ItemNodeName), Json.Serialize(obj.InvitationFields), Json.Serialize(obj.ImportFields), obj.SenderEmail, GetNull(obj.FallbackCultureCode), GetNull(obj.PersonalNote), 
                GetNull(obj.RedirectOnFirstLogin), obj.SendMethod, GetNull(obj.ScheduledJobStartsAt), obj.SendBatchSize, obj.SendBatchInteval, obj.ErrorCount, obj.BulkImportLog.ToString()));
            return obj.BulkImportID;
        }

        public void UpdateBulkImportStatus(int bulkImportID, string action, int batchNumber, int recordsRead, int recordsSkipped, int invitationsCreated, int invitationsSent, int errorCount, XElement logEntry)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString, GetFullyQualifiedName("UpdateBulkImportStatus"), bulkImportID, action, batchNumber, GetNull(recordsRead),
                GetNull(recordsSkipped), GetNull(invitationsCreated), GetNull(invitationsSent), GetNull(errorCount), logEntry.ToString());
        }

        public void DeleteBulkImport(int portalId, int importedByUserID, int bulkImportID)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString, GetFullyQualifiedName("DeleteBulkImports"), portalId, GetNull(importedByUserID), GetNull(bulkImportID));
        }

        public IDataReader GetBulkImport(int bulkImportID)
        {
            return SqlHelper.ExecuteReader(ConnectionString, GetFullyQualifiedName("GetBulkImport"), bulkImportID);
        }

        public IDataReader GetBulkImports(int portalId, int importedByUserID, bool excludeAbortedOrFinished)
        {
            return SqlHelper.ExecuteReader(ConnectionString, GetFullyQualifiedName("GetBulkImports"), GetNull(portalId), GetNull(importedByUserID), excludeAbortedOrFinished);
        }

    #endregion

#endregion

    }
}