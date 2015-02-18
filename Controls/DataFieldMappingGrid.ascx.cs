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
using System.ComponentModel;
using System.Text;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DotNetNuke.Web.UI.WebControls;
using DotNetNuke.Entities.Icons;
using DotNetNuke.Framework;
using DotNetNuke.Services.Localization;
using DotNetNuke.Web.Client.ClientResourceManagement;
using DotNetNuke.Common.Utilities;

namespace WESNet.DNN.Modules.ByInvitation.Controls
{

    [ToolboxData("<{0}:DataFieldMappingGrid runat=server></{0}:DataFieldMappingGrid>")]
    public partial class DataFieldMappingGrid : System.Web.UI.UserControl
    {

        public enum MapStates
        {
            Unmapped = 0,
            Mapped,
        }

        public string UnmappedAltText = Localization.GetString("Unmapped", Configuration.LocalSharedResourceFile);
        public string MappedAltText = Localization.GetString("Mapped", Configuration.LocalSharedResourceFile);
       
        public List<ImportField> ImportFields { get; set; }
        
        public Dictionary<String, InvitationField> InvitationFields { get; set; }
        
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            ClientResourceManager.RegisterScript(Page, Configuration.ModulePath + "Scripts/DataFieldMappingGrid.js", 105);
        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        public void UpdateFieldMapping()
        {
            if (!string.IsNullOrEmpty(hfInvitationFields.Value))
            {
                InvitationFields = Json.Deserialize<Dictionary<string, InvitationField>>(hfInvitationFields.Value);
            }

            if (!string.IsNullOrEmpty(hfImportFields.Value))
            {
                ImportFields = Json.Deserialize<List<ImportField>>(hfImportFields.Value);
            }
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            hfImportFields.Value = Json.Serialize(ImportFields);
            hfInvitationFields.Value = Json.Serialize(InvitationFields);
        }
    }
}