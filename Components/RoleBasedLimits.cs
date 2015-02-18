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
using System.Web.UI.WebControls;

using DotNetNuke;
using DotNetNuke.Entities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Users;
using DotNetNuke.Security.Roles;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Security.Membership;

namespace WESNet.DNN.Modules.ByInvitation
{
    public class RoleBasedLimits : BaseEntityInfo, IHydratable
    {
        private string _AssignableRoles = string.Empty;
        private List<RoleInfo> _AssignableRolesList = null;
        private TimeSpan _AllocationPeriod = TimeSpan.FromDays(1.0);
        
        public RoleBasedLimits()
        {
            RoleBasedLimitID = -1;
            PortalId = -1;
            RoleID = -1;
            RoleName = string.Empty;
            RoleDescription = string.Empty;
        }

        public RoleBasedLimits(int portalId, int roleID) : this()
        {
            PortalId = portalId;
            RoleID = roleID;
        }

        public RoleBasedLimits(int portalId, int roleID, string assignableRoles) : this()
        {
            PortalId = portalId;
            RoleID = roleID;
            AssignableRoles = assignableRoles;
        }

        public int RoleBasedLimitID { get; set; }
        public int PortalId { get; set; }
        public int RoleID { get; set; }
        public int AllocatedInvitations { get; set; }
        public TimeSpan AllocationPeriod
        {
            get
            {
                return _AllocationPeriod;
            }
            set
            {
                _AllocationPeriod = value < Consts.MinimumAllocationPeriod ? Consts.MinimumAllocationPeriod : value;
            }
        }

        public int AllocatedInvitationsPerDay
        {
            get
            {
                return AllocatedInvitations == 0 ? int.MaxValue : (int)(AllocatedInvitations * (1.0 / AllocationPeriod.TotalDays));
            }
        }

        public string AssignableRoles
        {
            get
            {
                return _AssignableRoles;
            }
            set
            {
                _AssignableRoles = value;
                _AssignableRolesList = null; //invalidate read-only AssignableRolesList which will lazy-load when again needed
            }
        }

        // from join on Roles table
        public string RoleName { get; set; }
        public string RoleDescription { get; set; }

        public List<RoleInfo> AssignableRolesList
        {
            get
            {
                if (_AssignableRolesList == null)
                {
                    _AssignableRolesList = new List<RoleInfo>();

                    var rc = new RoleController();
                    foreach (var s in EscapedString.Seperate(AssignableRoles).Where(s => !string.IsNullOrEmpty(s)))
                    {
                        int roleID = -1;
                        if (int.TryParse(s, out roleID))
                        {
                            var roleInfo = rc.GetRoleById(PortalId, roleID);
                            if (roleInfo != null && !roleInfo.AutoAssignment)
                            {
                                _AssignableRolesList.Add(roleInfo);
                            }
                        }
                    }
                }
                return _AssignableRolesList;
            }
        } 

        #region IHydratable Members

        public void Fill(System.Data.IDataReader dr)
        {
            base.FillInternal(dr);
            RoleBasedLimitID = Null.SetNullInteger(dr["RoleBasedLimitID"]);
            PortalId = Null.SetNullInteger(dr["PortalId"]);
            RoleID = Null.SetNullInteger (dr["RoleID"]);
            AllocatedInvitations = Null.SetNullInteger(dr["AllocatedInvitations"]);
            AllocationPeriod = Utilities.SetNullTimeSpan(dr["AllocationPeriod"]);
            AssignableRoles = Null.SetNullString(dr["AssignableRoles"]);

            // from join on Roles table
            RoleName = Null.SetNullString(dr["RoleName"]);
            RoleDescription = Null.SetNullString(dr["RoleDescription"]);
        }

        public int KeyID
        {
            get
            {
                return RoleBasedLimitID;
            }
            set
            {
                RoleBasedLimitID = value;
            }
        }

        #endregion
    }
}