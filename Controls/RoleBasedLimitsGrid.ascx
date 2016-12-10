<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="RoleBasedLimitsGrid.ascx.cs" Inherits="WESNet.DNN.Modules.ByInvitation.Controls.RoleBasedLimitsGrid" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.UI.WebControls" Assembly="DotNetNuke.Web" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.UI.WebControls" Assembly="DotNetNuke.Web.Deprecated" %>
<%@ Register TagPrefix="wes" TagName="TimeSpanPicker" Src="~/DesktopModules/WESNet_ByInvitation/Controls/TimeSpanPicker.ascx" %>

<div class="dnnForm RoleBasedLimits dnnClear">
    <div class="dnnFormItem">
        <dnn:Label ID="plInvitingUserRoles" runat="server" ControlName="dgInvitingUserRoles" />
        <dnn:DnnGrid ID="dgInvitingUserRoles" runat="server" CssClass="dnnGrid" AutoGenerateColumns="false" ClientSettings-Selecting-AllowRowSelect="true">
            <mastertableview datakeynames="RoleBasedLimitID, RoleID">
                <Columns>
                    <dnn:DnnGridButtonColumn ButtonType="ImageButton" IconKey="Edit" CommandName="Select"></dnn:DnnGridButtonColumn>
                    <dnn:DnnGridButtonColumn ButtonType="ImageButton" IconKey="Delete" CommandName="Delete"></dnn:DnnGridButtonColumn>
                    <dnn:DnnGridBoundColumn HeaderText="RoleName" UniqueName="RoleName" DataField="RoleName"></dnn:DnnGridBoundColumn>
                    <dnn:DnnGridBoundColumn HeaderText="RoleDescription" UniqueName="RoleDescription" DataField="RoleDescription"></dnn:DnnGridBoundColumn>
                </Columns>
            </mastertableview>
        </dnn:DnnGrid>
    </div>
    <div id="divAddInvitingUserRole" runat="server" class="dnnFormItem">
        <dnn:Label ID="plAddInvitingUserRole" runat="server" ControlName="ddlInvitingUserRoles" />
        <asp:DropDownList ID="ddlInvitingUserRoles" runat="server" DataTextField="RoleName" DataValueField="RoleID"></asp:DropDownList>
        <dnn:dnnImageButton id="btnAddInvitingUserRole" runat="server" IconKey="Add" ResourceKey="btnAddInvitingUserRole" CssClass="AddRoleButton" CausesValidation="false"></dnn:dnnImageButton>
    </div>
    <div id="divEditRoleBasedLimits" runat="server" class="EditRoleBasedLimits" visible="false">
        <fieldset>
            <div class="dnnFormItem">
                <dnn:Label ID="plSelectedRoleName" runat="server" ControlName="lblSelectedRoleName" />
                <asp:Label ID="lblSelectedRoleName" runat="server"></asp:Label>
            </div>
            <div class="dnnFormItem">
                <dnn:Label ID="plAllocatedInvitations" runat="server" ControlName="tbAllocatedInvitations" />
                <dnn:DnnNumericTextBox ID="tbAllocatedInvitations" runat="server" Type="Number" DataType="Integer" NumberFormat-DecimalDigits="0" MinValue="0" MaxValue="9999" ShowSpinButtons="true"></dnn:DnnNumericTextBox>
            </div>
            <div class="dnnFormItem">
                <dnn:Label ID="plAllocationPeriod" runat="server" ControlName="tpAllocationPeriod" />
                <wes:TimeSpanPicker ID="tpAllocationPeriod" runat="server" />
            </div>
            <div class="dnnFormItem">
                <dnn:Label ID="plAssignableRoles" runat="server" ControlName="cblAssignableRoles" />
                <asp:CheckBoxList ID="cblAssignableRoles" runat="server" CssClass="dnnCheckBoxList" DataTextField="RoleName" DataValueField="RoleID" RepeatColumns="2"></asp:CheckBoxList>
            </div>
            <ul class="dnnActions dnnRight">
                <li><asp:LinkButton ID="cmdUpdate" runat="server" ResourceKey="cmdUpdate" CssClass="dnnPrimaryAction" /></li>
                <li><asp:LinkButton ID="cmdCancel" runat="server" ResourceKey="cmdCancel" CssClass="dnnSecondaryAction" /></li>
            </ul>
        </fieldset>
    </div>
</div>
