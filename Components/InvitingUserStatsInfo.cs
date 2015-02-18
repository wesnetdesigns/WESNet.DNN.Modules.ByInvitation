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
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;

namespace WESNet.DNN.Modules.ByInvitation
{
    public class InvitingUserStatsInfo : IHydratable
    {
        public string UserDisplayName { get; set; }
        
        public int TotalInvitationCount { get; private set;}
        public int ApprovedInvitationCount { get; private set; }
        public int DisapprovedInvitationCount { get; private set; }
        public int SentInvitationCount { get; private set; }
        public int RetractedInvitationCount { get; private set; }
        public int DeclinedInvitationCount { get; private set; }
        public int AcceptedInvitationCount { get; private set; }
        public int ExpiredInvitationCount { get; private set; }
        public int PendingInvitationCount { get; private set; }
        public DateTime LastInvitedOnDate { get; private set; }
        public int LastInvitationID { get; private set; }

        #region IHydratable Members

        public void Fill(System.Data.IDataReader dr)
        {
            TotalInvitationCount = Utilities.SetNullAsZeroInteger(dr["TotalInvitationCount"]);
            ApprovedInvitationCount = Utilities.SetNullAsZeroInteger(dr["ApprovedInvitationCount"]);
            DisapprovedInvitationCount = Utilities.SetNullAsZeroInteger(dr["DisapprovedInvitationCount"]);
            SentInvitationCount = Utilities.SetNullAsZeroInteger(dr["SentInvitationCount"]);
            RetractedInvitationCount = Utilities.SetNullAsZeroInteger(dr["RetractedInvitationCount"]);
            DeclinedInvitationCount = Utilities.SetNullAsZeroInteger(dr["DeclinedInvitationCount"]);
            AcceptedInvitationCount = Utilities.SetNullAsZeroInteger(dr["AcceptedInvitationCount"]);
            ExpiredInvitationCount = Utilities.SetNullAsZeroInteger(dr["ExpiredInvitationCount"]);
            PendingInvitationCount = Math.Max(0, SentInvitationCount - RetractedInvitationCount
                                                 - DeclinedInvitationCount - AcceptedInvitationCount - ExpiredInvitationCount);
            LastInvitedOnDate = Null.SetNullDateTime(dr["LastInvitedOnDate"]);
            LastInvitationID = Null.SetNullInteger(dr["LastInvitationID"]);
        }

        public int KeyID
        {
            get
            {
                return LastInvitationID;
            }
            set
            {
                LastInvitationID = value;
            }
        }

        #endregion
    }


}