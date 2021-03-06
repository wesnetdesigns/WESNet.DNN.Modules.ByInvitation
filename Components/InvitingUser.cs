﻿// Copyright (c) 2015, William Severance, Jr., WESNet Designs
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
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Users;

namespace WESNet.DNN.Modules.ByInvitation
{
    public class InvitingUser : IHydratable
    {
        public int InvitedByUserID { get; private set; }
        public int PortalId { get; private set; }
        public string InvitedByUserFullName { get; private set; }
        public string InvitedByUserEmail { get; private set; }
        public string InvitedByUserDisplayName { get; private set; }

        //public string DisplayName
        //{
        //    get
        //    {
        //        UserInfo user = null;
        //        if (InvitedByUserID != -1)
        //        {
        //            user = UserController.GetUserById(PortalId, InvitedByUserID);
        //            if (user != null)
        //            {
        //                return string.IsNullOrEmpty(user.DisplayName) ? user.Username : user.DisplayName;
        //            }
        //        }
        //        return Utilities.LocalizeSharedResource("Anonymous") + ":" + (string.IsNullOrEmpty(InvitedByUserFullName) ? InvitedByUserEmail : InvitedByUserFullName);  
        //    }
        //}

        public string Value
        {
            get
            {
                return InvitedByUserID == -1 ? InvitedByUserEmail : InvitedByUserID.ToString();
            }
        }
        
        #region IHydratable Members

        public void Fill(System.Data.IDataReader dr)
        {
            InvitedByUserID = Null.SetNullInteger(dr["InvitedByUserID"]);
            InvitedByUserFullName = Null.SetNullString(dr["InvitedByUserFullName"]);
            InvitedByUserEmail = Null.SetNullString(dr["InvitedByUserEmail"]);
            InvitedByUserDisplayName = (InvitedByUserID == -1 ? Utilities.LocalizeSharedResource("Anonymous") + ":" : "") + Null.SetNullString(dr["DisplayName"]);
        }

        public int KeyID
        {
            get
            {
                return InvitedByUserID;
            }
            set
            {
                InvitedByUserID = value;
            }
        }

        #endregion
    }


}