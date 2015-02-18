<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ProcessInvitation.ascx.cs" Inherits="WESNet.DNN.Modules.ByInvitation.ProcessInvitation" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Register TagPrefix="dnn" Assembly="DotNetNuke" Namespace="DotNetNuke.UI.WebControls" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.UI.WebControls" Assembly="DotNetNuke.Web" %>

<div class="WESNet_ByInvitation">
    <div id="divMessage" runat="server" class="Message"></div>
    <div id="divReasonDeclined" runat="server" class="dnnForm dnnInvitationDeclinedForm dnnClear" visible="false">
        <div class="dnnFormItem">
            <dnn:Label ID="plReasonDeclined" runat="server" ControlName="txtReasonDeclined" />
            <asp:TextBox ID="txtReasonDeclined" runat="server" TextMode="MultiLine" Rows="10" MaxLength="1000"></asp:TextBox>
        </div>
    </div>
    <div id="divCredentials" class="Credentials" runat="server" visible="false">
        <div class="dnnFormItem dnnFormHelp dnnClear">
            <p class="dnnFormRequired"><span><%=LocalizeString("RequiredFields")%></span></p>
        </div>
        <div id="dnnUserCredentialsForm" class="dnnForm dnnUserCredentialsForm dnnClear">
            <div id="divUsername" runat="server" class="dnnFormItem">
                <dnn:Label ID="plUsername" runat="server" ControlName="tbUsername" CssClass="dnnFormRequired" />
                <asp:TextBox ID="tbUsername" runat="server"></asp:TextBox>
                <asp:RequiredFieldValidator ID="valUsername" ControlToValidate="tbUsername" runat="server"
                    ResourceKey="valUsername.Required" Display="Dynamic" CssClass="dnnFormMessage dnnFormError" SetFocusOnError="true" Enabled="false" />
                <asp:RegularExpressionValidator ID="val2Username" ControlToValidate="tbUsername" runat="server"
                    ResourceKey="valUsername.RegexInvalid" Display="Dynamic" CssClass="dnnFormMessage dnnFormError" SetFocusOnError="true" />
            </div>
            <div class="dnnFormItem">
                <dnn:Label ID="plFirstName" runat="server" ControlName="tbFirstName" />
                <asp:TextBox ID="tbFirstName" runat="server" />
                <asp:RequiredFieldValidator ID="valFirstName" ControlToValidate="tbFirstName" runat="server"
                    ResourceKey="valFirstName.Required" Display="Dynamic" CssClass="dnnFormMessage dnnFormError" SetFocusOnError="true" />
            </div>
            <div class="dnnFormItem">
                <dnn:Label ID="plLastName" runat="server" ControlName="tbLastName" />
                <asp:TextBox ID="tbLastName" runat="server" />
                <asp:RequiredFieldValidator ID="valLastName" ControlToValidate="tbLastName" runat="server" InitialValue=""
                    ResourceKey="valLastName.Required" Display="Dynamic" CssClass="dnnFormMessage dnnFormError" SetFocusOnError="true" />
            </div>
            <div id="divDisplayName" runat="server" class="dnnFormItem">
                <dnn:Label ID="plDisplayName" runat="server" ControlName="tbDisplayName" />
                <asp:TextBox ID="tbDisplayName" runat="server"></asp:TextBox>
                <asp:RequiredFieldValidator ID="valDisplayName" ControlToValidate="tbDisplayName" runat="server"
                    ResourceKey="valDisplayName.Required" Display="Dynamic" CssClass="dnnFormMessage dnnFormError" SetFocusOnError="true" Enabled="false" />
                <asp:RegularExpressionValidator ID="val2DisplayName" ControlToValidate="tbDisplayName" runat="server"
                    ResourceKey="valDisplayName.RegexInvalid" Display="Dynamic" CssClass="dnnFormMessage dnnFormError" SetFocusOnError="true" />
            </div>
            <div id="divPassword" runat="server">
                <div id="divTemporaryPassword" runat="server" class="dnnFormItem">
                    <dnn:Label ID="plTemporaryPassword" runat="server" ControlName="tbTemporaryPassword" CssClass="dnnFormRequired" />
                    <asp:TextBox ID="tbTemporaryPassword" runat="server" TextMode="Password" class="dnnFormRequired"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="valTemporaryPassword" ControlToValidate="tbTemporaryPassword" runat="server"
                        ResourceKey="valTemporaryPassword.Required" Display="Dynamic" CssClass="dnnFormMessage dnnFormError" SetFocusOnError="true" />
                </div>
                <div class="dnnFormItem">
                    <dnn:Label ID="plPassword" runat="server" ControlName="tbPassword" />
                    <asp:TextBox ID="tbPassword" runat="server" TextMode="Password"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="valPassword" ControlToValidate="tbPassword" runat="server"
                        ResourceKey="valPassword.Required" Display="Dynamic" CssClass="dnnFormMessage dnnFormError" SetFocusOnError="true" Enabled="false" />
                    <asp:RegularExpressionValidator ID="val2Password" ControlToValidate="tbPassword" runat="server"
                        ResourceKey="valPassword.RegexInvalid" Display="Dynamic" CssClass="dnnFormMessage dnnFormError" SetFocusOnError="true" />
                </div>
                <div class="dnnFormItem" id="divPassword2" runat="server">
                    <dnn:Label ID="plPassword2" runat="server" ControlName="tbPassword2" CssClass="dnnFormRequired" />
                    <asp:TextBox ID="tbPassword2" runat="server" TextMode="Password"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="valPassword2" ControlToValidate="tbPassword2" runat="server"
                        ResourceKey="valPassword2.Required" Display="Dynamic" CssClass="dnnFormMessage dnnFormError" SetFocusOnError="true" />
                    <asp:CompareValidator ID="val2Password2" ControlToValidate="tbPassword2" ControlToCompare="tbPassword" Type="String" runat="server"
                        ResourceKey="valPassword2.CompareInvalid" Display="Dynamic" CssClass="dnnFormMessage dnnFormError" SetFocusOnError="true" />
                </div>
                <div id="divQuestionAndAnswer" runat="server">
                    <div class="dnnFormItem">
                        <dnn:Label ID="plQuestion" runat="server" ControlName="tbQuestion" />
                        <asp:TextBox ID="tbQuestion" runat="server" CssClass="dnnFormRequired"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="valQuestion" ControlToValidate="tbQuestion" runat="server"
                            ResourceKey="valQuestion.Required" Display="Dynamic" CssClass="dnnFormMessage dnnFormError" SetFocusOnError="true" />
                    </div>
                    <div class="dnnFormItem">
                        <dnn:Label ID="plAnswer" runat="server" ControlName="tbAnswer" />
                        <asp:TextBox ID="tbAnswer" runat="server" CssClass="dnnFormRequired"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="valAnswer" ControlToValidate="tbAnswer" runat="server"
                            ResourceKey="valAnswer.Required" Display="Dynamic" CssClass="dnnFormMessage dnnFormError" SetFocusOnError="true" />
                    </div>
                </div>
                <div class="dnnFormItem" id="divPersistLogin" runat="server">
                    <dnn:Label id="plPersistLogin" runat="server" ControlName="cbPersistLogin" />
                    <asp:CheckBox ID="cbPersistLogin" runat="server" />
                </div>
            </div>
        </div>
    </div>
    <div id="divProfile" runat="server" class="dnnProfileDetails" visible="false">
        <div class="dnnForm dnnProfile dnnClear">
            <dnn:ProfileEditorControl id="ctlProfileEditor" runat="Server"
                GroupByMode="Section"
                ViewStateMode="Disabled"
                enableClientValidation="true" />
            <div class="dnnClear"></div>
            <ul id="actionsRow" runat="server" class="dnnActions dnnClear">
                <li><asp:LinkButton class="dnnPrimaryAction" ID="cmdUpdateProfile" runat="server" resourcekey="cmdUpdateProfile" /></li>
            </ul>
        </div>
    </div>
    <div id="divCaptcha" runat="server" visible="false">
        <dnn:DnnCaptcha ID="ctlCaptcha" runat="server" EnableRefreshImage="True" Width="300px" CaptchaImage-AudioFilesPath="~/DesktopModules/WESNet_ByInvitation/App_Data/RadCaptcha"></dnn:DnnCaptcha>
    </div>
    <ul id="divActions" runat="server" class="dnnActions dnnClear" visible="false">
        <li>
            <asp:LinkButton ID="cmdAcceptInvitation" runat="server" CssClass="dnnPrimaryAction" ResourceKey="cmdAcceptInvitation" /></li>
        <li>
            <asp:LinkButton ID="cmdDeclineInvitation" runat="server" CssClass="dnnPrimaryAction" ResourceKey="cmdDeclineInvitation" /></li>
        <li>
            <asp:LinkButton ID="cmdCancel" runat="server" CssClass="dnnSecondaryAction" resourcekey="cmdCancel" CausesValidation="false" /></li>
    </ul>
</div>
<script language="javascript" type="text/javascript">
    /*globals jQuery, window, Sys */
    (function ($, Sys) {
        function setUpProfileEditor() {
            $('.dnnButtonDropdown').dnnSettingDropdown();
            $('#<%=ctlProfileEditor.ClientID%>').dnnPanels();
    }

    $(document).ready(function () {
        setUpProfileEditor();
        Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function () {
            setUpProfileEditor();
        });
    });
}(jQuery, window.Sys));
</script>

