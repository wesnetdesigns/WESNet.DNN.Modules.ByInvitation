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
using DotNetNuke.Common.Utilities;
using DotNetNuke.Security.Membership;

namespace WESNet.DNN.Modules.ByInvitation
{
    public class AssignableRoleInfo : BaseEntityInfo, IHydratable
    {

        public AssignableRoleInfo()
        {
            AssignableRoleID = -1;
            InvitationID = -1;
            RoleID = -1;
            RoleName = string.Empty;
            RoleDescription = string.Empty;
        }

        public AssignableRoleInfo(int roleId, string roleName, string description ) : this()
        {
            RoleID = roleId;
            RoleName = roleName;
            RoleDescription = description;
        }

        public int AssignableRoleID { get; set; }

        public int InvitationID { get; set; }

        public int RoleID { get; set; }

        public DateTime? EffectiveDate { get; set; }

        public DateTime? ExpiryDate { get; set; }        

        //Filled from join on DNN Roles table

        public string RoleName { get; set; }

        public string RoleDescription { get; set; }

        public override string ToString()
        {
            var sb = new StringBuilder(RoleName);
            var hasEffectiveDate = EffectiveDate.HasValue;
            var hasExpiryDate = ExpiryDate.HasValue;
            if (hasEffectiveDate || hasExpiryDate)
            {
                sb.Append(" (");
                if (hasEffectiveDate) sb.Append(string.Format(Utilities.LocalizeSharedResource("EffectiveDate"), EffectiveDate));
                if (hasExpiryDate)
                {
                    if (hasEffectiveDate) sb.Append(" - ");
                    sb.Append(string.Format(Utilities.LocalizeSharedResource("ExpiryDate"), ExpiryDate));
                }
                sb.Append(")");
            }
            return sb.ToString();
        }

#region IHydratable Members

        public void Fill(System.Data.IDataReader dr)
        {
            base.FillInternal(dr);
            AssignableRoleID = Null.SetNullInteger(dr["AssignableRoleID"]);
            InvitationID = Null.SetNullInteger(dr["InvitationID"]);
            RoleID = Null.SetNullInteger(dr["RoleID"]);

            EffectiveDate = Utilities.SetNullableDateTime(dr["EffectiveDate"]);
            ExpiryDate = Utilities.SetNullableDateTime(dr["ExpiryDate"]);
           
            //Filled from join on DNN Roles table
            RoleName = Null.SetNullString(dr["RoleName"]);
            RoleDescription = Null.SetNullString(dr["RoleDescription"]);
        }

        public int KeyID
        {
            get
            {
                return AssignableRoleID;
            }
            set
            {
                AssignableRoleID = value;
            }
        }

#endregion
    }
}