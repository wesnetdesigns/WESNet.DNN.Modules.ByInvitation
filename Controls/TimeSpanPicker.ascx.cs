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
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DotNetNuke.Entities.Icons;
using DotNetNuke.Services.Localization;
using DotNetNuke.Web.UI.WebControls;
using Telerik.Web.UI;

namespace WESNet.DNN.Modules.ByInvitation.Controls
{
    public partial class TimeSpanPicker : DotNetNuke.Framework.UserControlBase
    {
        string _LocalResourceFile;

        public bool ShowSign { get; set; }
        public bool ShowDays { get; set; }
        public bool ShowHours { get; set; }
        public bool ShowMinutes { get; set; }
        public bool ShowSeconds { get; set; }
        public bool ShowMilliseconds { get; set; }

        public bool ShowTime
        {
            get
            {
                return ShowHours && ShowMinutes && ShowSeconds && ShowMilliseconds;
            }
            set
            {
                ShowHours = value;
                ShowMinutes = value;
                ShowSeconds = value;
                ShowMilliseconds = value;
            }
        }

        public bool ShowSpinners { get; set; }

        public TimeSpan Value
        {
            get
            {
                return new TimeSpan((int)tbDays.Value, (int)tbHours.Value, (int)tbMinutes.Value, (int)tbSeconds.Value, (int)tbMilliseconds.Value);
            }
            set
            {
                btnSign.SelectedToggleStateIndex = value.CompareTo(TimeSpan.Zero) + 1;
                tbDays.Value = value.Days;
                tbHours.Value = value.Hours;
                tbMinutes.Value = value.Minutes;
                tbSeconds.Value = value.Seconds;
                tbMilliseconds.Value = value.Milliseconds;
            }
        }

        public TimeSpanPicker()
        {
            ShowSign = false;
            ShowDays = true;
            ShowTime = true;
            ShowMilliseconds = false;
            ShowSpinners = true;
        }
        
        public string LocalizeString(string resourceKey)
        {
            return Localization.GetString(resourceKey, LocalResourceFile);
        }

        protected override void  OnInit(EventArgs e)
        {
 	        base.OnInit(e);
            
            btnSign.ToggleStates.Add(new RadButtonToggleState("-"));
            btnSign.ToggleStates.Add(new RadButtonToggleState(" "));
            btnSign.ToggleStates.Add(new RadButtonToggleState("+"));
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            btnSign.Visible = ShowSign;
            spanDays.Visible = ShowDays;
            spanHours.Visible = ShowHours;
            spanMinutes.Visible = ShowMinutes;
            spanSeconds.Visible = ShowSeconds;
            spanMilliseconds.Visible = ShowMilliseconds;
            tbDays.ShowSpinButtons = ShowSpinners;
            tbHours.ShowSpinButtons = ShowSpinners;
            tbMinutes.ShowSpinButtons = ShowSpinners;
            tbSeconds.ShowSpinButtons = ShowSpinners;
            tbMilliseconds.ShowSpinButtons = ShowSpinners;
        }

        public string LocalResourceFile
        {
            get
            {
                string tmp = string.Empty;

                if (_LocalResourceFile == null)
                {
                    tmp = this.TemplateSourceDirectory + "/App_LocalResources/TimeSpanPicker.ascx.resx";
                }
                return tmp;
            }
            set
            {
                _LocalResourceFile = value;
            }
        }
    }
}