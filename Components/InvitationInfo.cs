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
using System.Text;

using DotNetNuke;
using DotNetNuke.Entities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Users;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Security.Membership;
using DotNetNuke.Services.Localization;
using DotNetNuke.Services.Tokens;
using DotNetNuke.Services.SystemDateTime;

namespace WESNet.DNN.Modules.ByInvitation
{
    public class InvitationInfo : BaseEntityInfo, IHydratable, IPropertyAccess
    {

        private List<AssignableRoleInfo> _AssignedRoles;
        Guid _RSVPCode;
        string _OriginatingContext;
        int _ModuleId = -1;
        int _TabId = -1;
        string _AssignedUserName = null;

        private Configuration _MyConfiguration = null;

        public InvitationInfo()
        {
            InvitationID = -1;
            PortalId = -1;
            BulkImportID = -1;
            RetractedByUserID = -1;
            ApprovedByUserID = -1;
            DisapprovedByUserID = -1;
            AssignedUserID = -1;
            RedirectOnFirstLogin = -1;
        }

        public InvitationInfo(int portalId, int moduleId, int tabId)
            : this()
        {
            PortalId = portalId;
            OriginatingContext = string.Format("{0}:{1}", moduleId, tabId);
        }

        public InvitationInfo(int portalId, string originatingContext)
            : this()
        {
            PortalId = portalId;
            OriginatingContext = originatingContext;
        }


        public int InvitationID { get; set; }

        public int PortalId { get; set; }

        public string OriginatingContext
        {
            get
            {
                return _OriginatingContext;
            }
            set
            {
                if (string.IsNullOrEmpty(value)) throw new ArgumentException("OriginatingContext of invitation cannot be null or empty string", "OriginatingContext");
                _OriginatingContext = value;                    
                var parts = value.Split(':');
                if (parts.Length == 2)
                {
                    int.TryParse(parts[0], out _ModuleId);
                    int.TryParse(parts[1], out _TabId);
                }
            }
        }

        public int BulkImportID { get; set; }

        public string RecipientCultureCode { get; set; }

        public string RecipientEmail { get; set; }

        public string RecipientFirstName { get; set; }

        public string RecipientLastName { get; set; }

        public string AssignedUsername
        {
            get
            {
                return AssignedUserID == -1 ? _AssignedUserName : UserController.GetUserById(PortalId, AssignedUserID).Username;
            }
            set
            {
                _AssignedUserName = value;
            }
        }

        public string AssignedDisplayName { get; set; }

        public string TemporaryPassword { get; set; }

        public string TemporaryPasswordSalt { get; set; }

        public string PersonalNote { get; set; }

        public Guid RSVPCode
        {
            get
            {
                if (_RSVPCode == Guid.Empty)
                {
                    _RSVPCode = Guid.NewGuid();
                }
                return _RSVPCode;
            }
        }

        public int RedirectOnFirstLogin { get; set; }

        public DateTime InvitedOnDate { get; set; }

        public int InvitedByUserID { get; set; }

        public string InvitedByUserIPAddr { get; set; }

        public string InvitedByUserFullName { get; set; }

        public string InvitedByUserEmail { get; set; }

        public DateTime ApprovedOnDate { get; set; }

        public int ApprovedByUserID { get; set; }

        public DateTime DisapprovedOnDate { get; set; }

        public int DisapprovedByUserID { get; set; }

        public DateTime SentOnDate { get; set; }

        public DateTime LastResentOnDate { get; set; }

        public int ResentCount { get; set; }

        public int RetractedByUserID { get; set; }

        public DateTime RetractedOnDate { get; set; }

        public int FailedAttemptCount { get; set; }

        public DateTime LockedOutUntilDate { get; set; }

        public DateTime AcceptedOnDate { get; set; }

        public DateTime ExpiresOnDate { get; set; }

        public DateTime DeclinedOnDate { get; set; }

        public string ReasonDeclined { get; set; }

        public int AssignedUserID { get; set; }

        public List<AssignableRoleInfo> AssignedRoles
        {
            get
            {
                if (_AssignedRoles == null)
                {
                    if (InvitationID == -1)
                    {
                        _AssignedRoles = new List<AssignableRoleInfo>();
                    }
                    else
                    {
                        _AssignedRoles = InvitationController.GetAssignableRolesByInvitation(InvitationID);
                    }
                }
                return _AssignedRoles;
            }
        }

        public bool IsArchived { get; set; }


        #region Public ReadOnly Properties

        public int ModuleId
        {
            get
            {
                return _ModuleId;
            }
        }

        public int TabId
        {
            get
            {
                return _TabId;
            }
        }

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

        //Filled from join on DNN Tabs table
        public string RedirectOnFirstLoginTabName { get; private set; }

        public UserInfo InvitedByUser
        {
            get
            {
                return InvitedByUserID == -1 ? new UserInfo() : UserController.GetUserById(PortalId, InvitedByUserID);
            }
        }

        public string InvitedByUsername
        {
            get
            {
                return InvitedByUser.Username;
            }
        }

        public string InvitedByUserDisplayName
        {
            get
            {
                return InvitedByUserID == -1 ? InvitedByUserFullName : InvitedByUser.DisplayName;
            }
        }

        public UserInfo RetractedByUser
        {
            get
            {
                return RetractedByUserID == -1 ? new UserInfo() : UserController.GetUserById(PortalId, RetractedByUserID);
            }
        }

        public string RetractedByUsername
        {
            get
            {
                return RetractedByUser.Username;
            }
        }

        public string RetractedByUserDisplayName
        {
            get
            {
                return RetractedByUser.DisplayName;
            }
        }

        public string RecipientFullName
        {
            get
            {
                return (RecipientFirstName + (" " + RecipientLastName));
            }
        }

        public string JoinUrl
        {
            get
            {
                return BuildActionLinkUrl("join");
            }
        }

        public string DeclineUrl
        {
            get
            {
                return BuildActionLinkUrl("decline");
            }
        }

        public string EditApproveUrl
        {
            get
            {
                var @params = new List<string>();
                @params.Add("mid=" + ModuleId.ToString());
                @params.Add("id=" + InvitationID.ToString());
                @params.Add("mode=3");
                return Globals.NavigateURL(TabId, "", @params.ToArray());
            }
        }

        public UserInfo ApprovedByUser
        {
            get
            {
                return ApprovedByUserID == -1 ? new UserInfo() : UserController.GetUserById(PortalId, ApprovedByUserID);
            }
        }

        public string ApprovedByUsername
        {
            get
            {
                return ApprovedByUser.Username;
            }
        }

        public string ApprovedByUserDisplayName
        {
            get
            {
                return ApprovedByUser.DisplayName;
            }
        }

        public UserInfo DisapprovedByUser
        {
            get
            {
                return DisapprovedByUserID == -1 ? new UserInfo() : UserController.GetUserById(PortalId, DisapprovedByUserID);
            }
        }

        public string DisapprovedByUsername
        {
            get
            {
                return DisapprovedByUser.Username;
            }
        }

        public string DisapprovedByUserDisplayName
        {
            get
            {
                return DisapprovedByUser.DisplayName;
            }
        }

        public bool IsCreated
        {
            get
            {
                return CreatedOnDate != DateTime.MinValue;
            }
        }

        public bool IsApproved
        {
            get
            {
                return ApprovedOnDate != DateTime.MinValue;
            }
        }

        public bool IsDisapproved
        {
            get
            {
                return DisapprovedOnDate != DateTime.MinValue;
            }
        }

        public bool HasBeenSent
        {
            get
            {
                return SentOnDate != DateTime.MinValue;
            }
        }

        public bool HasBeenResent
        {
            get
            {
                return LastResentOnDate != DateTime.MinValue && ResentCount > 0;
            }
        }

        public bool WasRetracted
        {
            get
            {
                return RetractedOnDate != DateTime.MinValue;
            }
        }

        public bool IsLockedOut
        {
            get
            {
                return LockedOutUntilDate > DateUtils.GetDatabaseTime();
            }

        }

        public bool LockOutHasExpired
        {
            get
            {
                return LockedOutUntilDate != DateTime.MinValue && !IsLockedOut;
            }
        }

        private bool HasExpiredInternal
        {
            get
            {
                return ExpiresOnDate != DateTime.MinValue && ExpiresOnDate < DateUtils.GetDatabaseTime();
            } 
        }

        public bool HasExpired
        {
            get
            {
                return !WasActedUpon && HasExpiredInternal;
            }
        }

        public bool WasAccepted
        {
            get
            {
                return AcceptedOnDate != DateTime.MinValue;
            }
        }

        public bool WasDeclined
        {
            get
            {
                return DeclinedOnDate != DateTime.MinValue;
            }
        }

        public bool WasActedUpon
        {
            get
            {
                return HasBeenSent && (WasRetracted || WasDeclined || WasAccepted);
            }
        }

        public bool IsPending
        {
            get
            {
                return !WasActedUpon && !HasExpiredInternal;
            }
        }

        public InvitationStatus Status
        {
            get
            {
                InvitationStatus flags = InvitationStatus.None;

                if (IsCreated) flags |= InvitationStatus.Created;

                if (IsDisapproved)
                {
                    flags |= InvitationStatus.Disapproved;
                }
                else if (IsApproved)
                {
                    flags |= InvitationStatus.Approved;

                    if (SentOnDate != DateTime.MinValue) flags |= InvitationStatus.Sent;
                    if (HasBeenResent) flags |= InvitationStatus.Resent;
                    if (IsLockedOut) flags |= InvitationStatus.LockedOut;

                    if (WasRetracted)
                    {
                        flags |= InvitationStatus.Retracted;
                    }
                    else if (WasDeclined)
                    {
                        flags |= InvitationStatus.Declined;
                    }
                    else if (WasAccepted)
                    {
                        flags |= InvitationStatus.Accepted;
                    }
                    else if (HasExpired)
                    {
                        flags |= InvitationStatus.Expired;
                    }
                    else
                    {
                        flags |= InvitationStatus.Pending;
                    }
                }
                return flags;
            }
        }

        public bool HasStatus(InvitationStatus invitationStatus)
        {
            return (Status & invitationStatus) == invitationStatus;
        }

        public bool HasNotStatus(InvitationStatus invitationStatus)
        {
            return (Status & invitationStatus) == 0;
        }


        #endregion

        #region Public Methods

        public void RefreshAssignedRoles()
        {
            _AssignedRoles = null;
        }

        #endregion

        #region Private Methods

        private string BuildActionLinkUrl(string action)
        {
            if (action == "join" || action == "decline")
            {
                var processorPageTabID = new Configuration(PortalId).GetProcessorPageTabID();
                if (processorPageTabID == Null.NullInteger)
                {
                    throw new DotNetNuke.Entities.Tabs.TabExistsException(processorPageTabID, "WESNet_ByInvitation Processor Page is missing. Invitations cannot be sent or processed.");
                }
                else
                {
                    var portalSettings = new PortalSettings(processorPageTabID, PortalId);
                    if (portalSettings.PortalAlias == null)
                    {
                        if (portalSettings.PrimaryAlias != null)
                        {
                            portalSettings.PortalAlias = portalSettings.PrimaryAlias;
                        }
                        else
                        {
                            var primaryAlias = PortalAliasController.Instance.GetPortalAliasesByPortalId(PortalId).Where(p => p.IsPrimary).First();
                            portalSettings.PortalAlias = primaryAlias;
                        }
                    }
                    var paramList = new List<string>();
                    paramList.Add("pid=" + PortalId.ToString());
                    paramList.Add("email=" + UrlUtils.EncodeParameter(RecipientEmail));
                    paramList.Add("rsvpcode=" + RSVPCode);
                    paramList.Add("action=" + action);
                    return DotNetNuke.Common.Globals.NavigateURL(processorPageTabID, portalSettings, "", paramList.ToArray());
                }
            }
            else
            {
                throw new ArgumentException("BuildActionLinkUrl action must be 'join' or 'decline'", "action");
            }
        }

        #endregion

        #region IHydratable Members

        public void Fill(System.Data.IDataReader dr)
        {
            base.FillInternal(dr);
            InvitationID = Null.SetNullInteger(dr["InvitationID"]);
            PortalId = Null.SetNullInteger(dr["PortalId"]);
            OriginatingContext = Null.SetNullString(dr["OriginatingContext"]);
            BulkImportID = Null.SetNullInteger(dr["BulkImportID"]);
            RecipientCultureCode = Null.SetNullString(dr["RecipientCultureCode"]);
            RecipientEmail = Null.SetNullString(dr["RecipientEmail"]);
            RecipientFirstName = Null.SetNullString(dr["RecipientFirstName"]);
            RecipientLastName = Null.SetNullString(dr["RecipientLastName"]);
            AssignedUsername = Null.SetNullString(dr["AssignedUsername"]);
            AssignedDisplayName = Null.SetNullString(dr["AssignedDisplayName"]);
            TemporaryPassword = Null.SetNullString(dr["TemporaryPassword"]);
            TemporaryPasswordSalt = Null.SetNullString(dr["TemporaryPasswordSalt"]);
            _RSVPCode = Null.SetNullGuid(dr["RSVPCode"]);
            PersonalNote = Null.SetNullString(dr["PersonalNote"]);
            RedirectOnFirstLogin = Null.SetNullInteger(dr["RedirectOnFirstLogin"]);
            InvitedOnDate = Null.SetNullDateTime(dr["InvitedOnDate"]);
            ApprovedOnDate = Null.SetNullDateTime(dr["ApprovedOnDate"]);
            ApprovedByUserID = Null.SetNullInteger(dr["ApprovedByUserID"]);
            DisapprovedOnDate = Null.SetNullDateTime(dr["DisapprovedOnDate"]);
            DisapprovedByUserID = Null.SetNullInteger(dr["DisapprovedByUserID"]);
            SentOnDate = Null.SetNullDateTime(dr["SentOnDate"]);
            LastResentOnDate = Null.SetNullDateTime(dr["LastResentOnDate"]);
            ResentCount = dr["ResentCount"] == DBNull.Value ? 0 : Convert.ToInt32(dr["ResentCount"]);
            RetractedByUserID = Null.SetNullInteger(dr["RetractedByUserID"]);
            RetractedOnDate = Null.SetNullDateTime(dr["RetractedOnDate"]);
            FailedAttemptCount = Null.SetNullInteger(dr["FailedAttemptCount"]);
            LockedOutUntilDate = Null.SetNullDateTime(dr["LockedOutUntilDate"]);
            AcceptedOnDate = Null.SetNullDateTime(dr["AcceptedOnDate"]);
            ExpiresOnDate = Null.SetNullDateTime(dr["ExpiresOnDate"]);
            DeclinedOnDate = Null.SetNullDateTime(dr["DeclinedOnDate"]);
            ReasonDeclined = Null.SetNullString(dr["ReasonDeclined"]);
            AssignedUserID = Null.SetNullInteger(dr["AssignedUserID"]);
            InvitedByUserID = Null.SetNullInteger(dr["InvitedByUserID"]);
            InvitedByUserIPAddr = Null.SetNullString(dr["InvitedByUserIPAddr"]);
            InvitedByUserEmail = Null.SetNullString(dr["InvitedByUserEmail"]);
            InvitedByUserFullName = Null.SetNullString(dr["InvitedByUserFullName"]);
            IsArchived = Null.SetNullBoolean(dr["IsArchived"]);

            //Filled from join on DNN Tabs table
            RedirectOnFirstLoginTabName = Null.SetNullString(dr["RedirectOnFirstLoginTabName"]);
        }

        public int KeyID
        {
            get
            {
                return InvitationID;
            }
            set
            {
                InvitationID = value;
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
                case "originatingcontext":
                    result = OriginatingContext;
                    break;
                case "portalname":
                    result = PortalController.Instance.GetPortal(PortalId, formatProvider.Name).PortalName;
                    break;
                case "portallogosrc":
                    result = PortalController.Instance.GetPortal(PortalId, formatProvider.Name).LogoFile;
                    break;
                case "recipientemail":
                    result = string.IsNullOrEmpty(RecipientEmail) ? string.Empty : RecipientEmail;
                    break;
                case "recipientfirstname":
                    result = string.IsNullOrEmpty(RecipientFirstName) ? string.Empty : RecipientFirstName;
                    break;
                case "recipientlastname":
                    result = string.IsNullOrEmpty(RecipientLastName) ? string.Empty : RecipientLastName;
                    break;
                case "recipientfullname":
                    result = string.IsNullOrEmpty(RecipientFullName) ? string.Empty : RecipientFullName;
                    break;
                case "recipientculturecode":
                    result = RecipientCultureCode;
                    break;
                case "rsvpcode":
                    result = RSVPCode.ToString(format);
                    break;
                case "personalnote":
                    result = string.IsNullOrEmpty(PersonalNote) ? string.Empty : PersonalNote;
                    break;
                case "redirecttabid":
                    result = RedirectOnFirstLogin.ToString();
                    break;
                case "redirecttabname":
                    result = RedirectOnFirstLoginTabName;
                    break;
                case "assignedusername":
                    result = AssignedUsername;
                    break;
                case "assigneddisplayname":
                    result = AssignedDisplayName;
                    break;
                case "temporarypassword":
                    result = Security.DecryptString(TemporaryPassword, TemporaryPasswordSalt);
                    break;
                case "assignedroles":
                    var sb = new StringBuilder();
                    if (AssignedRoles.Count > 0)
                    { 
                        sb.Append("<ul class='RolesAddedList'>");
                        foreach (var assignedRole in AssignedRoles)
                        {
                            sb.Append("  <li>" + assignedRole.ToString() + "</li>");
                        }
                        sb.Append("</ul>");
                    }
                    result = sb.ToString();
                    break;
                case "joinurl":
                    result = JoinUrl;
                    break;
                case "declineurl":
                    result = DeclineUrl;
                    break;
                case "editapproveurl":
                    result = EditApproveUrl;
                    break;
                case "invitedbyuserid":
                    result = InvitedByUserID.ToString(format, formatProvider);
                    break;
                case "invitedbyuserfullname":
                    result = InvitedByUserFullName;
                    break;
                case "invitedbyusername":
                    result = InvitedByUsername;
                    break;
                case "invitedbyuserdisplayname":
                    result = InvitedByUserDisplayName;
                    break;
                case "invitedondate":
                    result = Utilities.FormattedUTCDate(InvitedOnDate, accessingUser, format, formatProvider);
                    break;
                case "approvedondate":
                    result = Utilities.FormattedUTCDate(ApprovedOnDate, accessingUser, format, formatProvider);
                    break;
                case "approvedbyuserid":
                    result = ApprovedByUserID.ToString(format, formatProvider);
                    break;
                case "approvedbyusername":
                    result = ApprovedByUserID == -1 ? "" : ApprovedByUsername;
                    break;
                case "approvedbyuserdisplayname":
                    result = ApprovedByUserID == -1 ? "" : ApprovedByUserDisplayName;
                    break;
                case "disapprovedondate":
                    result = Utilities.FormattedUTCDate(DisapprovedOnDate, accessingUser, format, formatProvider);
                    break;
                case "disapprovedbyuserid":
                    result = DisapprovedByUserID.ToString(format, formatProvider);
                    break;
                case "disapprovedbyusername":
                    result = DisapprovedByUserID == -1 ? "" : DisapprovedByUsername;
                    break;
                case "disapprovedbyuserdisplayname":
                    result = DisapprovedByUserID == -1 ? "" : DisapprovedByUserDisplayName;
                    break;
                case "sentondate":
                    result = Utilities.FormattedUTCDate(SentOnDate, accessingUser, format, formatProvider);
                    break;
                case "expiresondate":
                    result = Utilities.FormattedUTCDate(ExpiresOnDate, accessingUser, format, formatProvider);
                    break;
                case "acceptedondate":
                    result = Utilities.FormattedUTCDate(AcceptedOnDate, accessingUser, format, formatProvider);
                    break;
                case "declinedondate":
                    result = Utilities.FormattedUTCDate(DeclinedOnDate, accessingUser, format, formatProvider);
                    break;
                case "reasondeclined":
                    result = string.IsNullOrEmpty(ReasonDeclined) ? string.Empty : ReasonDeclined;
                    break;
                case "retractedondate":
                    result = Utilities.FormattedUTCDate(RetractedOnDate, accessingUser, format, formatProvider);
                    break;
                case "retractedbyuserid":
                    result = RetractedByUserID.ToString(format, formatProvider);
                    break;
                case "retractedbyusername":
                    result = RetractedByUserID == -1 ? "" : RetractedByUsername;
                    break;
                case "retractedbyuserdisplayname":
                    result = RetractedByUserID == -1 ? "" : RetractedByUserDisplayName;
                    break;
                case "resentcount":
                    result = ResentCount.ToString(format, formatProvider);
                    break;
                case "lastresentondate":
                    result = Utilities.FormattedUTCDate(LastResentOnDate, accessingUser, format, formatProvider);
                    break;
                case "lockedoutuntildate":
                    result = Utilities.FormattedUTCDate(LockedOutUntilDate, accessingUser, format, formatProvider);
                    break;
                case "failedattemptcount":
                    result = FailedAttemptCount.ToString(format, formatProvider);
                    break;
                case "lastmodifiedondate":
                    result = Utilities.FormattedDate(LastModifiedOnDate, format, formatProvider);
                    break;
                case "status":
                    result = Status.ToString(format);
                    break;
            }
            return result;
        }

        #endregion
    }
}