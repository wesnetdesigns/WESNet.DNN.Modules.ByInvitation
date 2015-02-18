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
using System.Data;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

using DotNetNuke;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Security.Membership;

namespace WESNet.DNN.Modules.ByInvitation
{
    public interface IDataProvider
    {

#region Configuration

        void UpdateConfiguration(Configuration objConfiguration, int modifiedByUserID);

        IDataReader GetConfiguration(int portalId);

#endregion

#region LocalizedConfiguration

        int UpdateLocalizedConfiguration(LocalizedConfiguration objLocalizedConfiguration, int modifiedByUserID);

        void DeleteLocalizedConfiguration(int localizedConfigurationID);

        IDataReader GetLocalizedConfiguration(int localizedConfigurationID);
        
        IDataReader GetLocalizedConfigurationByPortalAndCulture(int portalId, string cultureCode);

        IDataReader GetLocalizedConfigurations(int portalId);

#endregion

#region RoleBasedLimits

        int UpdateRoleBasedLimits(RoleBasedLimits objRoleBasedLimits, int modifiedByUserID);

        void DeleteRoleBasedLimits(int roleBasedLimitID);

        IDataReader GetRoleBasedLimits(int portalId, int roleID);

        IDataReader GetRoleBasedLimitsCollection(int portalId);

#endregion

#region Invitation

        int UpdateInvitation(InvitationInfo objInvitation, int userID);

        IDataReader UpdateInvitationStatus(int invitationID, string action, int modifiedByUserID, DateTime newDateTimeValue, string reasonDeclined, int assignedUserID);

        void DeleteInvitation(int invitationID);

        void DeleteInvitations(int portalId, int invitingUserID);

        void DeleteAcceptedOrExpiredInvitations(int portalId, int invitingUserID);

        IDataReader GetInvitation(int invitationID);

        IDataReader GetInvitationByRecipientEmail(int portalId, string recipientEmail);

        IDataReader GetInvitations(int portalId);

        IDataReader GetInvitationByAssignedUsername(int portalId, string assignedUsername);

        IDataReader GetBulkImportedInvitationsNotSent(int bulkImportID, int sendBatchSize);

        IDataReader GetInvitationByPortalAndRSVPCode(int portalId, string rsvpCode);

        IDataReader GetInvitationsByInvitingUser(int portalId, int invitingUserID, DateTime startDate, DateTime endDate, bool includeArchived);

        IDataReader GetInvitationsByInvitingUserIPAddr(int portalId, string invitingUserIPAddr);

        IDataReader GetInvitationsByInvitingUserEmail(int portalId, string invitingUserEmail, DateTime startDate, DateTime endDate, bool includeArchived);

        int GetInvitationCount(int portalId, int invitingUserID, DateTime startDate, DateTime endDate);

        IDataReader GetInvitingUserStats(int portalId, DateTime startDate, DateTime endDate, bool includeArchivedInvitations, string invitingUser);

        IDataReader GetInvitingUsers(int portalId, DateTime startDate, DateTime endDate, bool includeArchived);

#endregion

#region AssignableRoles

        int UpdateAssignableRole(AssignableRoleInfo objAssignableRole, int modifiedByUserID);

        void DeleteAssignableRole(int assignableRoleID);

        IDataReader GetAssignableRole(int assignableRoleID);

        IDataReader GetAssignableRolesByInvitation(int invitationID);

#endregion

#region BulkImport

        int SaveBulkImport(BulkImportInfo obj);

        void UpdateBulkImportStatus(int bulkImportID, string action, int batchNumber, int recordsRead, int recordsSkipped, int invitationsCreated, int invitationsSent, int errorCount, XElement logEntry);
        
        void DeleteBulkImport(int portalId, int importedByUserID, int bulkImportID);
        
        IDataReader GetBulkImport(int bulkImportID);

        IDataReader GetBulkImports(int portalId, int importedByUserID, bool excludeAbortedOrFinished);
    
#endregion








        







    }
}
