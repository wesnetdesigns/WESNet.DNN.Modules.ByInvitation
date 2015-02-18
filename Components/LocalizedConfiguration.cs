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

using DotNetNuke;
using DotNetNuke.Entities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Host;
using DotNetNuke.Entities.Users;
using DotNetNuke.Entities.Controllers;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Services.Localization;
using DotNetNuke.Security.Membership;

namespace WESNet.DNN.Modules.ByInvitation 
{
    public class LocalizedConfiguration : BaseEntityInfo, IHydratable
    {
        public LocalizedConfiguration()
        {
        }

        public LocalizedConfiguration(PortalSettings portalSettings) : this(portalSettings.PortalId, portalSettings.CultureCode)
        {
        }

        public LocalizedConfiguration(int portalId, string cultureCode)
        {
            LocalizedConfigurationID = -1;
            PortalId = portalId;
            CultureCode = cultureCode;
            EmailRegex = DefaultEmailRegex;
            SendInvitationButtonImage = string.Empty;
            if (cultureCode == Localization.SystemLocale)
            {
                SendInvitationHtml = Localization.GetString("Invitation.Html", Configuration.LocalSharedResourceFile);
                SendInvitationButtonText = Localization.GetString("Invitation.ButtonText", Configuration.LocalSharedResourceFile);
                InvitationSubject = Localization.GetString("Invitation.Subject", Configuration.LocalSharedResourceFile);
                InvitationBody = Localization.GetString("Invitation.Body", Configuration.LocalSharedResourceFile);
            }
            else
            {
                SendInvitationHtml = string.Empty;
                SendInvitationHtml = string.Empty;
                InvitationSubject = string.Empty;
                InvitationBody = string.Empty;
            }
        }

        public int LocalizedConfigurationID { get; set; }

        public int PortalId { get; set; }

        public string CultureCode { get; set; }

        public string SendInvitationHtml { get; set; }

        public string SendInvitationButtonText { get; set; }

        public string SendInvitationButtonImage { get; set; }

        public System.Drawing.Size ImageButtonSize { get; set; }

        public string InvitationSubject { get; set; }

        public string InvitationBody { get; set; }

        public string EmailRegex { get; set; }

        public string DefaultEmailRegex
        {
            get
            {
                return (string)Utilities.GetUserSetting(PortalId, "Security_EmailValidation", "");
            }
        }

    
#region IHydratable Members

        public void  Fill(System.Data.IDataReader dr)
        {
            base.FillInternal(dr);
            LocalizedConfigurationID = Null.SetNullInteger(dr["LocalizedConfigurationID"]);
            PortalId = Null.SetNullInteger(dr["PortalId"]);
            CultureCode = Null.SetNullString(dr["CultureCode"]);
            SendInvitationHtml = Null.SetNullString(dr["SendInvitationHtml"]);
            SendInvitationButtonText = Null.SetNullString(dr["SendInvitationButtonText"]);
            SendInvitationButtonImage = Null.SetNullString(dr["SendInvitationButtonImage"]);
            int width, height;
            width = Null.SetNullInteger(dr["ImageButtonWidth"]);
            height = Null.SetNullInteger(dr["ImageButtonHeight"]);
            if (width > 0 && height > 0)
            {
                ImageButtonSize = new System.Drawing.Size(width, height);
            }
            InvitationSubject = Null.SetNullString(dr["InvitationSubject"]); 
            InvitationBody = Null.SetNullString(dr["InvitationBody"]);
            if (Null.IsNull(dr["EmailRegex"]))
                EmailRegex = DefaultEmailRegex;
            else
                EmailRegex = Convert.ToString(dr["EmailRegex"]);
        }

        public int  KeyID
        {
	          get 
	        {
                return LocalizedConfigurationID;
	        }
	          set 
	        {
                LocalizedConfigurationID = value;
	        }
        }

#endregion

    }
}