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
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Threading;
using System.Linq;
using System.Text.RegularExpressions;
using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Modules.Actions;
using DotNetNuke.Entities.Tabs;
using DotNetNuke.Entities.Users;
using DotNetNuke.Security.Membership;
using DotNetNuke.Security.Roles;
using DotNetNuke.Security.Permissions;
using DotNetNuke.Services.Localization;
using DotNetNuke.Services.FileSystem;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.SystemDateTime;
using DotNetNuke.UI.Skins;
using DotNetNuke.UI.Skins.Controls;
using Telerik.Web.UI;

namespace WESNet.DNN.Modules.ByInvitation
{
    public class ByInvitationModuleBase : PortalModuleBase
    {
        private Configuration _MyConfiguration = null;
        private LocalizedConfiguration _MyLocalizedConfiguration = null;
        private Security _ModuleSecurity = null;
        private InvitationInfo _CurrentInvitation = null;
        private UserInfo _User = null;

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

        public LocalizedConfiguration MyLocalizedConfiguration
        {
            get
            {
                if (_MyLocalizedConfiguration == null)
                {
                    _MyLocalizedConfiguration = MyConfiguration.GetLocalizedConfiguration(CultureCode, PortalSettings.CultureCode);
                }
                return _MyLocalizedConfiguration;
            }
        }

        public Security ModuleSecurity
        {
            get
            {
                if (_ModuleSecurity == null)
                {
                    _ModuleSecurity = new Security(ModuleConfiguration);
                }
                return _ModuleSecurity;
            }
        }

        public virtual string CultureCode
        {
            get
            {
                return (ControlMode == Mode.Edit ? CurrentInvitation.RecipientCultureCode : Thread.CurrentThread.CurrentUICulture.Name);
            }
            set
            {
            }
        }

        public Mode ControlMode
        {
            get
            {
                return ViewState["mode"] == null ? Mode.View : (Mode)ViewState["mode"];
            }
            set
            {
                ViewState["mode"] = value;
            }
        }

        public int CurrentInvitationID
        {
            get
            {
                return ViewState["id"] == null ? -1 : (int)ViewState["id"];
            }
            set
            {
                ViewState["id"] = value;
            }
        }

        public InvitationInfo CurrentInvitation
        {
            get
            {
                if (_CurrentInvitation == null)
                {
                    if (CurrentInvitationID != -1)
                    {
                        _CurrentInvitation = InvitationController.GetInvitation(CurrentInvitationID);
                    }
                    else
                    {
                        _CurrentInvitation = DataCache.GetCache<InvitationInfo>(InvitationCacheKey);
                    }
                    if (_CurrentInvitation == null) _CurrentInvitation = new InvitationInfo(PortalId, ModuleId, TabId);
                    DataCache.SetCache(InvitationCacheKey, _CurrentInvitation, TimeSpan.FromMinutes(Consts.InvitationCacheDuration));
                }
                return _CurrentInvitation;
            }
            set
            {
                DataCache.SetCache(InvitationCacheKey, value, TimeSpan.FromMinutes(Consts.InvitationCacheDuration));
                _CurrentInvitation = value;
            }
        }
        
        private string InvitationCacheKey
        {
            get
            {
                if (ViewState["CacheKey"] == null)
                {
                    ViewState["CacheKey"] = Guid.NewGuid().ToString() + "_" + UserId;
                }
                return (string)ViewState["CacheKey"];
            }
        }

        public string ReturnUrl
        {
            get
            {
                return ViewState["returnurl"] == null ? Globals.NavigateURL() : (string)ViewState["returnurl"];
            }
            set
            {
                ViewState["returnurl"] = value;
            }
        }

        protected void AddModuleMessage(string messageKey, ModuleMessage.ModuleMessageType moduleMessageType)
        {
            AddModuleMessage("", messageKey, moduleMessageType);
        }

        protected void AddModuleMessage(string headingKey, string messageKey, ModuleMessage.ModuleMessageType moduleMessageType)
        {
            var localizedHeading = headingKey == "" ? "" : LocalizeString(headingKey);
            Skin.AddModuleMessage(this, localizedHeading, LocalizeString(messageKey), moduleMessageType);
        }

        protected void AddLocalizedModuleMessage(string message, ModuleMessage.ModuleMessageType moduleMessageType)
        {
            AddLocalizedModuleMessage("", message, moduleMessageType);
        }

        protected void AddLocalizedModuleMessage(string heading, string message, ModuleMessage.ModuleMessageType moduleMessageType)
        {
            Skin.AddModuleMessage(this, heading, message, moduleMessageType);
        }

    }
}