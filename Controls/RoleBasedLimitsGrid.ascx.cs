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
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Security.Roles;

using DotNetNuke.Services.Localization;
using DotNetNuke.Web.UI.WebControls;
using Telerik.Web.UI;

namespace WESNet.DNN.Modules.ByInvitation.Controls
{
    public partial class RoleBasedLimitsGrid : DotNetNuke.Framework.UserControlBase
    {
        string _LocalResourceFile;
        List<RoleBasedLimits> _RoleBasedLimitsCollection = null;

        public int PortalId
        {
            get
            {
                return PortalSettings.PortalId;
            }
        }

        public int TabId
        {
            get
            {
                return PortalSettings.ActiveTab.TabID;
            }
        }

        public int ModuleId { get; set; }

        public int UserID
        {
            get
            {
                return PortalSettings.UserId;
            }
        }

        public string LocalResourceFile
        {
            get
            {
                string tmp = string.Empty;

                if (_LocalResourceFile == null)
                {
                    tmp = this.TemplateSourceDirectory + "/App_LocalResources/RoleBasedLimitsGrid.ascx.resx";
                }
                return tmp;
            }
            set
            {
                _LocalResourceFile = value;
            }
        }

        public List<RoleBasedLimits> RoleBasedLimitsCollection
        {
            get
            {
                if (_RoleBasedLimitsCollection == null)
                {
                    _RoleBasedLimitsCollection = InvitationController.GetRoleBasedLimitsCollection(PortalId).Values.ToList<RoleBasedLimits>();
                }
                return _RoleBasedLimitsCollection;
            }
        }

        public int CurrentRoleID
        {
            get
            {
                return ViewState["CurrentRoleID"] == null ? -1 : (int)ViewState["CurrentRoleID"];
            }
            set
            {
                ViewState["CurrentRoleID"] = value;
            }
        }

        public int CurrentRoleBasedLimitID
        {
            get
            {
                return ViewState["CurrentRoleBasedLimitID"] == null ? -1 : (int)ViewState["CurrentRoleBasedLimitID"];
            }
            set
            {
                ViewState["CurrentRoleBasedLimitID"] = value;
            }
        }

        public RoleBasedLimitsGrid()
        {

        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            dgInvitingUserRoles.SelectedIndexChanged +=new EventHandler(dgInvitingUserRoles_SelectedIndexChanged);
            dgInvitingUserRoles.DeleteCommand +=new GridCommandEventHandler(dgInvitingUserRoles_DeleteCommand);
            //dgInvitingUserRoles.ItemCommand +=new GridCommandEventHandler(dgInvitingUserRoles_ItemCommand);
            btnAddInvitingUserRole.Click +=new ImageClickEventHandler(btnAddInvitingUserRole_Click);
            cmdUpdate.Click +=new EventHandler(cmdUpdate_Click);
            cmdCancel.Click +=new EventHandler(cmdCancel_Click);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                DataBind();
                BindAssignableRolesCBL();
            }
        }

        protected override void DataBind(bool refreshData)
        {
            if (refreshData) _RoleBasedLimitsCollection = null;

            dgInvitingUserRoles.DataSource = RoleBasedLimitsCollection;
            dgInvitingUserRoles.DataBind();
            BindInvitingUserRolesDDL();
        }

        private void dgInvitingUserRoles_ItemCommand(object sender, GridCommandEventArgs e)
        {
            GridItem itm = e.Item;
            if (itm.ItemType == GridItemType.SelectedItem)
            {
                CurrentRoleBasedLimitID = (int)dgInvitingUserRoles.MasterTableView.DataKeyValues[0]["RoleBasedLimitID"];
                CurrentRoleID = (int)dgInvitingUserRoles.MasterTableView.DataKeyValues[0]["RoleID"];
                var selectedRBL = RoleBasedLimitsCollection.Where(rbl => rbl.RoleBasedLimitID == CurrentRoleBasedLimitID).SingleOrDefault();
                divEditRoleBasedLimits.Visible = true;
                BindEditForm(selectedRBL);
            }
            else
            {
                divEditRoleBasedLimits.Visible = false;
            }
        }

        private void dgInvitingUserRoles_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (dgInvitingUserRoles.SelectedItems.Count > 0)
            {
                CurrentRoleBasedLimitID = (int)dgInvitingUserRoles.SelectedValues["RoleBasedLimitID"];
                CurrentRoleID = (int)dgInvitingUserRoles.SelectedValues["RoleID"];
                var selectedRBL = RoleBasedLimitsCollection.Where(rbl => rbl.RoleBasedLimitID == CurrentRoleBasedLimitID).SingleOrDefault();
                divEditRoleBasedLimits.Visible = true;
                BindEditForm(selectedRBL);
            }
            else
            {
                divEditRoleBasedLimits.Visible = false;
            }
        }

        private void dgInvitingUserRoles_DeleteCommand(object sender, GridCommandEventArgs e)
        {
            int roleBasedLimitID = -1;
            if (e.Item.ItemIndex != -1)
            {
                roleBasedLimitID = (int)dgInvitingUserRoles.MasterTableView.DataKeyValues[e.Item.ItemIndex]["RoleBasedLimitID"];
                if (roleBasedLimitID != -1)
                {
                    InvitationController.DeleteRoleBasedLimits(roleBasedLimitID, PortalId);
                    DataBind(true);
                }
            }
        }

        private void btnAddInvitingUserRole_Click (object sender, EventArgs e)
        {
            if (ddlInvitingUserRoles.SelectedIndex > -1)
            {
                CurrentRoleID = int.Parse(ddlInvitingUserRoles.SelectedValue);
                CurrentRoleBasedLimitID = -1;
                var roleInfo = new RoleController().GetRoleById(PortalId, CurrentRoleID);
                var roleBasedLimits = new RoleBasedLimits(PortalId, CurrentRoleID, PortalSettings.RegisteredRoleId.ToString()) { RoleName = roleInfo.RoleName, RoleDescription = roleInfo.Description };
                BindEditForm(roleBasedLimits);
                divEditRoleBasedLimits.Visible = true;
            }
        }

        private void cmdUpdate_Click(object sender, EventArgs e)
        {
            RoleBasedLimits roleBasedLimits;

            if (CurrentRoleBasedLimitID == -1)
            {
                roleBasedLimits = new RoleBasedLimits(PortalId, CurrentRoleID);
                var roleInfo = new RoleController().GetRoleById(PortalId, CurrentRoleID);
                roleBasedLimits.RoleName = roleInfo.RoleName;
                roleBasedLimits.RoleDescription = roleInfo.Description;
            }
            else
            {
                roleBasedLimits = RoleBasedLimitsCollection.Where(rbl => rbl.RoleBasedLimitID == CurrentRoleBasedLimitID).SingleOrDefault<RoleBasedLimits>();
            }

            FillFromEditForm(ref roleBasedLimits);
            InvitationController.UpdateRoleBasedLimits(roleBasedLimits, UserID);
            DataBind();
            divEditRoleBasedLimits.Visible = false;
        }

        private void cmdCancel_Click(object sender, EventArgs e)
        {
            divEditRoleBasedLimits.Visible = false;
            CurrentRoleID = -1;
            CurrentRoleBasedLimitID = -1;
        }

        private void BindInvitingUserRolesDDL()
        {
           var filteredInvitingUserRoles = new RoleController().GetRoles(PortalId).ToList<RoleInfo>();
           foreach (GridItem itm in dgInvitingUserRoles.MasterTableView.Items)
            {
                if (itm.ItemType == GridItemType.Item || itm.ItemType == GridItemType.AlternatingItem)
                {
                    var roleID = (int)dgInvitingUserRoles.MasterTableView.DataKeyValues[itm.ItemIndex]["RoleID"];
                   filteredInvitingUserRoles.RemoveAll(roleInfo => roleInfo.RoleID == roleID);
                }
            }

            ddlInvitingUserRoles.DataSource = filteredInvitingUserRoles;
            ddlInvitingUserRoles.DataBind();
            divAddInvitingUserRole.Visible = ddlInvitingUserRoles.Items.Count > 0;
        }

        private void BindAssignableRolesCBL()
        {
            
            cblAssignableRoles.DataSource = new RoleController().GetRoles(PortalId, r => !r.AutoAssignment).Select(r => new { r.RoleName, r.RoleID });
            cblAssignableRoles.DataBind();
        }

        private void AssignableRolesCBLSetAssigned(string assignableRoles)
        {
            var separatedAssignableRoles = EscapedString.Seperate(assignableRoles).Where(s => !string.IsNullOrEmpty(s)).ToList<string>();

            foreach (ListItem li in cblAssignableRoles.Items)
            {
                if (separatedAssignableRoles.Count > 0)
                {
                    foreach (var s in separatedAssignableRoles)
                    {
                        if (li.Value == s)
                        {
                            li.Selected = true;
                            separatedAssignableRoles.Remove(s);
                            break;
                        }
                        else
                        {
                            li.Selected = false;
                        }
                    }
                }
                else
                {
                    li.Selected = false;
                }
            }
        }

        private string AssignableRolesCBLGetAssigned()
        {
            var sb = new StringBuilder();
            foreach (ListItem li in cblAssignableRoles.Items)
            {
                if (li.Selected)
                {
                    sb.Append(li.Value);
                    sb.Append(",");
                }
            }
            if (sb.Length > 0) sb.Length--;
            return sb.ToString();
        }

        private void AssignableRolesCBLSetClearAll(bool selected)
        {
            foreach (ListItem li in cblAssignableRoles.Items)
            {
                li.Selected = selected;
            }
        }
        
        private void BindEditForm(RoleBasedLimits roleBasedLimits)
        {
            lblSelectedRoleName.Text = roleBasedLimits.RoleBasedLimitID == -1 ? roleBasedLimits.RoleName + " *" : roleBasedLimits.RoleName;
            tbAllocatedInvitations.Value = roleBasedLimits.AllocatedInvitations;
            tpAllocationPeriod.Value = roleBasedLimits.AllocationPeriod;
            AssignableRolesCBLSetAssigned(roleBasedLimits.AssignableRoles);
        }

        private void FillFromEditForm(ref RoleBasedLimits roleBasedLimits)
        {
            roleBasedLimits.AllocatedInvitations = (int)tbAllocatedInvitations.Value;
            roleBasedLimits.AllocationPeriod = tpAllocationPeriod.Value;
            roleBasedLimits.AssignableRoles = AssignableRolesCBLGetAssigned();
        }

        public string LocalizeString(string resourceKey)
        {
            return Localization.GetString(resourceKey, LocalResourceFile);
        }
    }
}