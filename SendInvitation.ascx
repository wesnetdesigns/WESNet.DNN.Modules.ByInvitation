<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SendInvitation.ascx.cs" Inherits="WESNet.DNN.Modules.ByInvitation.SendInvitation" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.UI.WebControls" Assembly="DotNetNuke.Web" %>
<div class="WESNet_ByInvitation">
    <ul id="ulActionCommands" runat="server" class="dnnActions dnnClear">
		<li><asp:HyperLink ID="lnkManageInvitations" runat="server" ResourceKey="lnkManageInvitations" CssClass="dnnSecondaryAction"></asp:HyperLink></li>
        <li><asp:HyperLink ID="lnkBulkImportInvitations" runat="server" ResourceKey="lnkBulkImportInvitations" CssClass="dnnSecondaryAction"></asp:HyperLink></li>    
	</ul>
    <div id="divSendInvitation" runat="server" class="SendInvitation">
        <div id="divInvitationHtml" runat="server" class="InvitationHtml"></div>
        <div class="InvitationButton">
            <dnn:DnnRadButton id="btnSendInvitation" runat="server" ButtonType="LinkButton"
                CausesValidation="False" Localize="false"></dnn:DnnRadButton>
        </div>
    </div>
    <div id="divEditInvitation" runat="server" class="EditInvitation">
        <div class="dnnFormItem WESNet_RequiredIndicator">
			<span class="dnnFormRequired"><%=LocalizeString("RequiredFields")%></span>
		</div>
        <div id="dnnEditInvitationForm" class="dnnForm dnnEditInvitationForm dnnClear">
            <h2 id="dnnPanel-InvitationForm" class="dnnFormSectionHead"><a href="#" class="dnnSectionExpanded"><%=LocalizeString("InvitationForm")%></a></h2>
            <fieldset>
                <div id="divInvitedByUser" runat="server" class="dnnFormItem">
                    <dnn:Label id="plInvitedByUserFullName" runat="server" ControlName="tbInvitedByUserFullName" CssClass="dnnFormRequired" />
                    <asp:TextBox id="tbInvitedByUserFullName" runat="server"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="valInvitedByUserFullName" ControlToValidate="tbInvitedByUserFullName" runat="server"
                        ResourceKey="valInvitedByUserFullName.Required" Display="Dynamic" CssClass="dnnFormMessage dnnFormError" SetFocusOnError="true" InitialValue="" />
                    <dnn:Label ID="plInvitedByUserEmail" runat="server" ControlName="tbInvitedByUserEmail" CssClass ="dnnFormRequired"  />
                    <asp:TextBox ID="tbInvitedByUserEmail" runat="server"/>
                    <asp:RequiredFieldValidator ID="valInvitedByUserEmail" ControlToValidate="tbInvitedByUserEmail" runat="server"
                        ResourceKey="valInvitedByUserEmail.Required" Display="Dynamic" CssClass="dnnFormMessage dnnFormError" SetFocusOnError="true" InitialValue="" />
                    <asp:RegularExpressionValidator ID="val2InvitedByUserEmail" ControlToValidate="tbInvitedByUserEmail" runat="server"
                        ResourceKey="val2InvitedByUserEmail.RegexInvalid" Display="Dynamic" CssClass="dnnFormMessage dnnFormError" SetFocusOnError="true" />
                </div>
                <div class="dnnFormItem">
                    <dnn:Label ID="plRecipientEmail" runat="server" ControlName="tbRecipientEmail"  CssClass ="dnnFormRequired" />
                    <asp:TextBox ID="tbRecipientEmail" runat="server"/>
                    <asp:RequiredFieldValidator ID="valRecipientEmail" ControlToValidate="tbRecipientEmail" runat="server"
                        ResourceKey="valRecipientEmail.Required" Display="Dynamic" CssClass="dnnFormMessage dnnFormError" SetFocusOnError="true" />
                    <asp:RegularExpressionValidator ID="val2RecipientEmail" ControlToValidate="tbRecipientEmail" runat="server"
                        ResourceKey="valRecipientEmail.RegexInvalid" Display="Dynamic" CssClass="dnnFormMessage dnnFormError" SetFocusOnError="true" />
                </div>
                <div id="divRecipientEmail2" runat="server" class="dnnFormItem">
                    <dnn:Label ID="plRecipientEmail2" runat="server" ControlName="tbRecipientEmail2" CssClass ="dnnFormRequired" />
                    <asp:TextBox ID="tbRecipientEmail2" runat="server"/>
                    <asp:RequiredFieldValidator ID="valRecipientEmail2" ControlToValidate="tbRecipientEmail2" runat="server"
                        ResourceKey="valRecipientEmail2.Required" Display="Dynamic" CssClass="dnnFormMessage dnnFormError" SetFocusOnError="true" />
                    <asp:CompareValidator ID="val2RecipientEmail2" ControlToValidate="tbRecipientEmail2" ControlToCompare="tbRecipientEmail" Type="String" runat="server"
                        ResourceKey="valRecipientEmail2.CompareInvalid" Display="Dynamic" CssClass="dnnFormMessage dnnFormError" SetFocusOnError="true" />
                </div>
                <div id="divRecipientFirstName" runat="server" class="dnnFormItem">
                    <dnn:Label ID="plRecipientFirstName" runat="server" ControlName="tbRecipientFirstName" />
                    <asp:TextBox ID="tbRecipientFirstName" runat="server"/>
                    <asp:RequiredFieldValidator ID="valRecipientFirstName" ControlToValidate="tbRecipientFirstName" runat="server"
                        ResourceKey="valRecipientFirstName.Required" Display="Dynamic" CssClass="dnnFormMessage dnnFormError" SetFocusOnError="true" />
                </div>
                <div id="divRecipientLastName" runat="server" class="dnnFormItem">
                    <dnn:Label ID="plRecipientLastName" runat="server" ControlName="tbRecipientLastName" />
                    <asp:TextBox ID="tbRecipientLastName" runat="server"/>
                    <asp:RequiredFieldValidator ID="valRecipientLastName" ControlToValidate="tbRecipientLastName" runat="server" InitialValue=""
                        ResourceKey="valRecipientLastName.Required" Display="Dynamic" CssClass="dnnFormMessage dnnFormError" SetFocusOnError="true" />
                </div>
                <div id="divDisplayName" runat="server" class="dnnFormItem" >
                        <dnn:Label ID="plAssignedDisplayName" runat="server" ControlName="tbAssignedDisplayName" />
                        <asp:TextBox ID="tbAssignedDisplayName" runat="server"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="valAssignedDisplayName" ControlToValidate="tbAssignedDisplayName" runat="server"
                            ResourceKey="valAssignedDisplayName.Required" Display="Dynamic" CssClass="dnnFormMessage dnnFormError" SetFocusOnError="true" Enabled="false" />
                        <asp:RegularExpressionValidator ID="val2AssignedDisplayName" ControlToValidate="tbAssignedDisplayName" runat="server"
                            ResourceKey="valAssignedDisplayName.RegexInvalid" Display="Dynamic" CssClass="dnnFormMessage dnnFormError" SetFocusOnError="true" />
                </div>
                <div id = "divCultureCode" runat="server" class="dnnFormItem">
                    <dnn:Label ID="plRecipientCultureCode" runat="server" ControlName="ctlCultureCode" />
                    <dnn:DnnLanguageComboBox id="ctlCultureCode" runat="server"></dnn:DnnLanguageComboBox>
                </div>
                <div id="divPersonalNote" runat="server" class="dnnFormItem" visible="false">
                    <dnn:Label ID="plPersonalNote" runat="server" ControlName="tbPersonalNote" />
                    <asp:TextBox ID="tbPersonalNote" runat="server" TextMode="MultiLine" Rows="10" />                      
                </div>
                <div id="divCredentials" runat="server" visible="false">
                    <div id="divUsername" runat="server" class="dnnFormItem">
                        <dnn:Label ID="plAssignedUsername" runat="server" ControlName="tbAssignedUsername" />
                        <asp:TextBox ID="tbAssignedUsername" runat="server"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="valAssignedUsername" ControlToValidate="tbAssignedUsername" runat="server"
                            ResourceKey="valAssignedUsername.Required" Display="Dynamic" CssClass="dnnFormMessage dnnFormError" SetFocusOnError="true" Enabled="false" />
                        <asp:RegularExpressionValidator ID="val2AssignedUsername" ControlToValidate="tbAssignedUsername" runat="server"
                            ResourceKey="valAssignedUsername.RegexInvalid" Display="Dynamic" CssClass="dnnFormMessage dnnFormError" SetFocusOnError="true" />
                    </div>
                    <div id="divTemporaryPassword" runat="server">
                        <div class="dnnFormItem">
                            <dnn:Label ID="plTemporaryPassword" runat="server" ControlName="tbTemporaryPassword" />
                                <asp:CheckBox ID="cbGenerateRandomPassword" runat="server" ResourceKey="cbGenerateRandomPassword" /><br />
                                <asp:TextBox ID="tbTemporaryPassword" runat="server" TextMode="Password" CssClass="TemporaryPassword"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="valTemporaryPassword" ControlToValidate="tbTemporaryPassword" runat="server"
                                    ResourceKey="valTemporaryPassword.Required" Display="Dynamic" CssClass="dnnFormMessage dnnFormError" SetFocusOnError="true" Enabled="false" />
                                <asp:RegularExpressionValidator ID="val2TemporaryPassword" ControlToValidate="tbTemporaryPassword" runat="server"
                                ResourceKey="valTemporaryPassword.RegexInvalid" Display="Dynamic" CssClass="dnnFormMessage dnnFormError" SetFocusOnError="true" />
                        </div>
                        <div class="dnnFormItem" id="divTemporaryPassword2" runat="server" >
                            <dnn:Label ID="plTemporaryPassword2" runat="server" ControlName="tbTemporaryPassword2" />
                            <asp:TextBox ID="tbTemporaryPassword2" runat="server" TextMode="Password"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="valTemporaryPassword2" ControlToValidate="tbTemporaryPassword2" runat="server"
                                ResourceKey="valTemporaryPassword2.Required" Display="Dynamic" CssClass="dnnFormMessage dnnFormError" SetFocusOnError="true" Enabled="false" />
                            <asp:CompareValidator ID="val2TemporaryPassword2" ControlToValidate="tbTemporaryPassword2" ControlToCompare="tbTemporaryPassword" Type="String" runat="server"
                                ResourceKey="valTemporaryPassword2.CompareInvalid" Display="Dynamic" CssClass="dnnFormMessage dnnFormError" SetFocusOnError="true" />
                        </div>
                    </div>
                </div>
                <div class="dnnFormItem" id="divRedirection" runat="server" visible="false">
                    <dnn:Label ID="plRedirectOnFirstLogin" runat="server" ControlName="ddlRedirectOnFirstLogin" />
                    <asp:DropDownList ID="ddlRedirectOnFirstLogin" runat="server" DataTextField="IndentedTabName" DataValueField="TabID"></asp:DropDownList>
                </div>
            </fieldset>
        </div>
        <div id="dnnRolesAssignmentForm" runat="server" class="dnnForm dnnRolesAssignmentForm dnnClear">
            <h2 id="dnnPanel-Roles" class="dnnFormSectionHead"><a href="#"><%=LocalizeString("Roles")%></a></h2>
            <fieldset>
                <div class="dnnFormItem">
                    <dnn:Label ID="plAssignedRoles" runat="server" ControlName="dgAssignedRoles" />
                    <dnn:DnnGrid id="dgAssignedRoles" runat="server" AutoGenerateColumns="false" AllowSorting="true" CssClass="dnnGrid">
                        <MasterTableView DataKeyNames="AssignableRoleID, RoleID" EditMode="InPlace">
                            <Columns>
                                <dnn:DnnGridTemplateColumn UniqueName="Commands" ItemStyle-Width="50px">
                                    <ItemTemplate>
                                        <dnn:DnnImageButton IconKey="Edit" CommandName="Edit" id="imgButtonEdit" runat="server" ResourceKey="imgEdit" />
                                        <dnn:DnnImageButton IconKey="Delete" CommandName="Delete" id="imgButtonDelete" CssClass="imgButtonDelete" runat="server" ResourceKey="imgDelete" />
                                    </ItemTemplate>
                                    <EditItemTemplate>
                                        <dnn:DnnImageButton IconKey="Save" CommandName="Save" ID="imgButtonSave" runat="server" ResourceKey="imgSave" />
                                        <dnn:DnnImageButton IconKey="Cancel" CommandName="Cancel" ID="imgButtonCancel" runat="server" ResourceKey="imgCancel" />
                                    </EditItemTemplate>
                                </dnn:DnnGridTemplateColumn>
                                <dnn:DnnGridBoundColumn UniqueName="RoleName" DataField="RoleName" HeaderText="RoleName" ReadOnly="true" />
                                <dnn:DnnGridBoundColumn UniqueName="RoleDescription" DataField="RoleDescription" HeaderText="RoleDescription" ReadOnly="true" />
                                <dnn:DnnGridDateTimeColumn DataField="EffectiveDate" HeaderText="EffectiveDate" PickerType="DatePicker" />
                                <dnn:DnnGridDateTimeColumn DataField="ExpiryDate" HeaderText="ExpiryDate" PickerType="DatePicker" />
                             </Columns>
                        </MasterTableView>
                    </dnn:DnnGrid>
                </div>
                <div id="divAddRole" runat="server" class="dnnFormItem">
                    <dnn:Label ID="plAddRole" runat="server" ControlName="ddlRoles" />
                    <asp:DropDownList ID="ddlRoles" runat="server" DataTextField="RoleName" DataValueField="RoleID"></asp:DropDownList>
                    <dnn:dnnImageButton id="btnAddRole" runat="server" IconKey="Add" ResourceKey="btnAddRole" CssClass="AddRoleButton" CausesValidation="false"></dnn:dnnImageButton>
                </div>
            </fieldset>
        </div>
        <div id="divCaptcha" runat="server" visible="false">
            <dnn:DnnCaptcha ID="ctlCaptcha" runat="server" EnableRefreshImage="True" CaptchaImage-AudioFilesPath="~/DesktopModules/WESNet_ByInvitation/App_Data/RadCaptcha"></dnn:DnnCaptcha>
        </div>
        <div id="divMessage">
            <asp:PlaceHolder ID="phMessage" runat="server" EnableViewState="false"></asp:PlaceHolder>
        </div>
        <ul class="dnnActions dnnClear">
            <li><asp:LinkButton ID="cmdSend" runat="server" CssClass="dnnPrimaryAction" /></li>
            <li><asp:LinkButton ID="cmdUpdate" runat="server" CssClass = "dnnSecondaryAction" /></li>
            <li><asp:Hyperlink id="cmdCancel" runat="server" ResourceKey="cmdCancel" CssClass="dnnSecondaryAction" /></li>
        </ul>
    </div>
</div>
<script type="text/javascript">
    /*globals jQuery, window, Sys */
    (function ($, Sys) {
        function setUpWESNet_ByInvitation() {
            $('.WESNet_ByInvitation .dnnForm').dnnPanels();
            $('.imgButtonDelete').dnnConfirm({
                text: '<%= Localization.GetString("DeleteItem.Text", Localization.SharedResourceFile) %>',
                yesText: '<%= Localization.GetString("Yes.Text", Localization.SharedResourceFile) %>',
                noText: '<%= Localization.GetString("No.Text", Localization.SharedResourceFile) %>',
                title: '<%= Localization.GetString("Confirm.Text", Localization.SharedResourceFile) %>'
            });
            if ($("#<%=cbGenerateRandomPassword.ClientID %>").is(':checked')) {
                $("#<%=tbTemporaryPassword.ClientID %>").hide();
                $("#<%=divTemporaryPassword2.ClientID %>").hide();
            }
            $("#<%=cbGenerateRandomPassword.ClientID %>").click(function (e) {
                $("#<%=tbTemporaryPassword.ClientID %>").toggle();
                $("#<%=divTemporaryPassword2.ClientID %>").toggle();
            });
        }

        $(document).ready(function () {
            setUpWESNet_ByInvitation();
            Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function () {
                setUpWESNet_ByInvitation();
            });
        });
    } (jQuery, window.Sys));
</script>
