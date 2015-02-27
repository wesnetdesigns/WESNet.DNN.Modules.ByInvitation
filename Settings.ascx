<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Settings.ascx.cs" Inherits="WESNet.DNN.Modules.ByInvitation.Settings" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Register TagPrefix="dnn" TagName="texteditor" Src="~/controls/TextEditor.ascx" %>
<%@ Register TagPrefix="dnn" TagName="FilePickerUploader" Src="~/controls/filepickeruploader.ascx" %>
<%@ Register TagPrefix="wes" TagName="RoleBasedLimitsGrid" Src="~/DesktopModules/WESNet_ByInvitation/Controls/RoleBasedLimitsGrid.ascx" %>
<%@ Register TagPrefix="wes" TagName="TimeSpanPicker" Src="~/DesktopModules/WESNet_ByInvitation/Controls/TimeSpanPicker.ascx" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.UI.WebControls" Assembly="DotNetNuke.Web" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.UI.WebControls" Assembly="DotNetNuke" %>

<div id="WESNet_ByInvitation_Settings" class="dnnForm WESNet_ByInvitation_Settings dnnClear">
    <div class="dnnFormExpandContent">
        <a href=""><%=Localization.GetString("ExpandAll", Localization.SharedResourceFile)%></a>
    </div>
    <h2 id="Panel-LocalizedSettings" class="dnnFormSectionHead"><a href="" class="dnnSectionExpanded"><%=LocalizeString("LocalizedSettings")%></a></h2>
    <fieldset>
        <div class="dnnFormItem">
            <dnn:Label ID="plCultureCode" runat="server" ControlName="ctlCultureCode" />
            <dnn:DnnLanguageComboBox id="ctlCultureCode" runat="server" AutoPostBack="true" class="dnnCultureCode"></dnn:DnnLanguageComboBox>
        </div>
        <div id="divLocalizedFields" runat="server">
            <div class="dnnFormItem">
                <dnn:Label ID="plSendInvitationHtml" runat="server" ControlName="teSendInvitationHtml" />
                <dnn:TextEditor ID="teSendInvitationHtml" runat="server" />
            </div>
            <div class="dnnFormItem">
                <dnn:Label ID="plSendInvitationButtonText" runat="server" ControlName="tbSendInvitationButtonText" />
                <asp:TextBox ID="tbSendInvitationButtonText" runat="server"></asp:TextBox>
            </div>
            <div class="dnnFormItem">
                <dnn:Label ID="plSendInvitationButtonImage" runat="server" ControlName="imgEditSendInvitationButtonImage" />
                <dnn:FilePickerUploader ID="ctlSendInvitationButtonImage" runat="server" Required="True" />
            </div>
            <div class="dnnFormItem">
                <dnn:Label ID="plImageButtonSize" runat="server" ControlName="tbImageButtonWidth" />
                <dnn:DnnNumericTextBox ID="tbImageButtonWidth" DataType="Integer" runat="server" NumberFormat-DecimalDigits="0" ShowSpinButtons="true" MinValue="0" MaxValue="1024"></dnn:DnnNumericTextBox>&nbsp;X&nbsp;
                <dnn:DnnNumericTextBox ID="tbImageButtonHeight" DataType="Integer" runat="server" NumberFormat-DecimalDigits="0" ShowSpinButtons="true" MinValue="0" MaxValue="1024"></dnn:DnnNumericTextBox>
            </div>
            <div class="dnnFormItem">
                <dnn:Label ID="plInvitationSubject" runat="server" ControlName="tbInvitationSubject" />
                <asp:TextBox ID="tbInvitationSubject" runat="server"></asp:TextBox>
            </div>
            <div class="dnnFormItem">
                <dnn:Label ID="plInvitationBody" runat="server" ControlName="tbInvitationBody" />
                <div class="WESNet_InputGroup">
                    <asp:LinkButton ID="btnDefaultInvitationBody" runat="server" CssClass="LoadDefaultButton"
                            CausesValidation="false"  ResourceKey="btnDefaultInvitationBody"></asp:LinkButton>
                    <dnn:TextEditor ID="teInvitationBody" runat="server" />
                </div>           
            </div>
            <div class="dnnFormItem">               
                <dnn:Label ID="plEmailRegex" runat="server" ControlName="tbEmailRegex" />
                <div class="WESNet_InputGroup">
                    <asp:LinkButton ID="btnDefaultEmailRegex" runat="server" CssClass="LoadDefaultButton"
                      CausesValidation  ="false"  ResourceKey="btnDefaultEmailRegex"></asp:LinkButton>
                    <asp:TextBox ID="tbEmailRegex" runat="server"></asp:TextBox>
                </div>
            </div>
        </div>
    </fieldset>
    <h2 id="Panel-GeneralSettings" class="dnnFormSectionHead"><a href="" class="dnnSectionExpanded"><%=LocalizeString("GeneralSettings")%></a></h2>
    <fieldset>
        <div class="dnnFormItem">
            <dnn:Label ID="plValidityPeriod" runat="server" ControlName="tpValidityPeriod" />
            <wes:TimeSpanPicker id="tpValidityPeriod" runat="server"></wes:TimeSpanPicker>
        </div>
        <div class="dnnFormItem">
            <dnn:Label ID="plEnableAutoResend" runat="server" ControlName="cbEnableAutoResend"></dnn:Label>
            <asp:CheckBox ID="cbEnableAutoResend" runat="server" />
        </div>
        <div class="dnnFormItem">
            <dnn:Label ID="plMaxResends" runat="server" ControlName="tbMaxResends" />
            <dnn:DnnNumericTextBox ID="tbMaxResends" runat="server" NumberFormat-DecimalDigits="0" MinValue="0" MaxValue="99" ShowSpinButtons="true"></dnn:DnnNumericTextBox>
        </div>
        <div class="dnnFormItem">
            <dnn:Label ID="plResendInterval" runat="server" ControlName="tpResendInterval" />
            <wes:TimeSpanPicker id="tpResendInterval" runat="server"></wes:TimeSpanPicker>
        </div>
        <div class="dnnFormItem">
            <dnn:Label ID="plAutoDeleteArchiveExpiredInvitations" runat="server" ControlName ="rblAutoDeleteArchiveExpiredInvitations" />
            <asp:RadioButtonList ID="rblAutoDeleteArchiveExpiredInvitations" runat="server" CssClass="dnnRadioButtons">
                <asp:ListItem Value ="0" ResourceKey="NoAction"></asp:ListItem>
                <asp:ListItem Value ="1" ResourceKey="DeleteAction"></asp:ListItem>
                <asp:ListItem Value ="2" ResourceKey="ArchiveAction"></asp:ListItem>
            </asp:RadioButtonList>
        </div>
        <div class="dnnFormItem">
            <dnn:Label id="plDaysPastExpiration" runat="server" ControlName="tbDaysPastExpiration" />
            <dnn:DnnNumericTextBox id="tbDaysPastExpiration" runat="server" NumberFormat-DecimalDigits="0" MinValue="0" ShowSpinButtons="true"></dnn:DnnNumericTextBox>
        </div>
    </fieldset>
    <h2 id="Panel-FormSettings" class="dnnFormSectionHead"><a href="" class="dnnSectionExpanded"><%=LocalizeString("FormSettings") %></a></h2>
    <fieldset>
        <div class="dnnFormItem">
            <dnn:Label ID="plRequireRecipientEmailConfirm" runat="server" ControlName="cbRequireRecipientEmailConfirm" />
            <asp:CheckBox ID="cbRequireRecipientEmailConfirm" runat="server" />
        </div>
        <div class="dnnFormItem">
            <dnn:Label ID="plEnablePersonalNote" runat="server" ControlName="cbEnablePersonalNote" />
            <asp:CheckBox ID="cbEnablePersonalNote" runat="server" />
        </div>
        <div class="dnnFormItem">
            <dnn:Label ID="plTemporaryPasswordMode" runat="server" ControlName = "rblTemporaryPasswordMode" />
            <asp:RadioButtonList ID="rblTemporaryPasswordMode" runat="server" class="dnnFormRadioButtons">
                <asp:ListItem ResourceKey="NoTemporaryPassword" Value="0"></asp:ListItem>
                <asp:ListItem ResourceKey="AutoGenerateTemporaryPassword" Value="1"></asp:ListItem>
                <asp:ListItem ResourceKey="OptionalTemporaryPassword" Value="2"></asp:ListItem>
                <asp:ListItem ResourceKey="RequiredTemporaryPassword" Value="3"></asp:ListItem>
            </asp:RadioButtonList>
        </div>
        <div id="divRequireTemporaryPasswordConfirm" class="dnnFormItem">
            <dnn:Label ID="plRequireTemporaryPasswordConfirm" runat="server" ControlName="cbRequireTemporaryPasswordConfirm" />
            <asp:CheckBox ID="cbRequireTemporaryPasswordConfirm" runat="server" />
        </div>
    </fieldset>
    <h2 id="Panel-SecuritySettings" class="dnnFormSectionHead"><a href="" class="dnnSectionExpanded"><%=LocalizeString("SecuritySettings") %></a></h2>
    <fieldset>
        <div class="dnnFormItem">
            <dnn:Label ID="plEnableInvitationCaptcha" runat="server" ControlName="rblCaptchaUsage" />
            <asp:RadioButtonList ID="rblEnableInvitationCaptcha" runat="server" CssClass="dnnFormRadioButtons" >
                <asp:ListItem ResourceKey="NoCaptcha" Value = "0"></asp:ListItem>
                <asp:ListItem ResourceKey="AnonymousUsersOnly" Value = "1"></asp:ListItem>
                <asp:ListItem ResourceKey="AllUsers" Value="2"></asp:ListItem>
            </asp:RadioButtonList>
        </div>
        <div class="dnnFormItem">
            <dnn:Label ID="plEnableAcceptanceCaptcha" runat="server" ControlName="cbEnableAcceptanceCaptcha" />
            <asp:CheckBox ID="cbEnableAcceptanceCaptcha" runat="server" />
        </div>
        <div id="divCaptchaSettings" class="dnnFormItem">
            <div class="dnnFormItem">
                <dnn:Label ID="plEnableCaptchaAudio" runat="server" ControlName="cbEnableCaptchaAudio" />
                <asp:CheckBox ID="cbEnableCaptchaAudio" runat="server" />
            </div>
            <div class="dnnFormItem">
                <dnn:Label ID="plCaptchaIsCaseInsensitive" runat="server" ControlName="cbCaptchaIsCaseInsensitive" />
                <asp:CheckBox ID="cbCaptchaIsCaseInsensitive" runat="server" />            
            </div>
            <div class="dnnFormItem">
                <dnn:Label ID="plCaptchaLineNoise" runat="server" ControlName="ddlCaptchaLineNoise" />
                <asp:DropDownList ID="ddlCaptchaLineNoise" runat="server" CssClass="dnnFormRadioButtons">
                    <asp:ListItem ResourceKey="None" Value="0"></asp:ListItem>
                    <asp:ListItem ResourceKey="Low" Value="1"></asp:ListItem>
                    <asp:ListItem ResourceKey="Medium" Value="2"></asp:ListItem>
                    <asp:ListItem ResourceKey="High" Value="3"></asp:ListItem>
                    <asp:ListItem ResourceKey="Extreme" Value="4"></asp:ListItem>
                </asp:DropDownList>
            </div>
            <div class="dnnFormItem">
                <dnn:Label ID="plCaptchaBackgroundNoise" runat="server" ControlName="ddlCaptchaBackgroundNoise" />
                <asp:DropDownList ID="ddlCaptchaBackgroundNoise" runat="server" CssClass="dnnFormRadioButtons">
                    <asp:ListItem ResourceKey="None" Value="0"></asp:ListItem>
                    <asp:ListItem ResourceKey="Low" Value="1"></asp:ListItem>
                    <asp:ListItem ResourceKey="Medium" Value="2"></asp:ListItem>
                    <asp:ListItem ResourceKey="High" Value="3"></asp:ListItem>
                    <asp:ListItem ResourceKey="Extreme" Value="4"></asp:ListItem>
                </asp:DropDownList>
            </div>
        </div>
        <div class="dnnFormItem">
            <dnn:Label ID="plInvitationFiltering" runat="server" ControlName="ddlInvitationFiltering" />
            <asp:DropDownList ID="ddlInvitationFiltering" runat="server" CssClass="dnnFormRadioButtons">
                <asp:ListItem ResourceKey="NoFiltering" Value="0"></asp:ListItem>
                <asp:ListItem ResourceKey="SenderUserID" Value="1"></asp:ListItem>
                <asp:ListItem ResourceKey="SenderIPAddr" Value="2"></asp:ListItem>
                <asp:ListItem ResourceKey="SenderEmail" Value="3"></asp:ListItem>
            </asp:DropDownList>
        </div>
        <div class="dnnFormItem">
            <dnn:Label ID="plInvitationFilteringInterval" runat="server" ControlName="tbInvitationFilteringInterval" />
            <wes:TimeSpanPicker id="tpInvitationFilteringInterval" runat="server"></wes:TimeSpanPicker>
        </div>
        <div class="dnnFormItem">
            <dnn:Label ID="plMaxFailedAttempts" runat="server" ControlName ="tbMaxFailedAttempts" />
            <dnn:DnnNumericTextBox ID="tbMaxFailedAttempts" runat="server" NumberFormat-DecimalDigits="0" MinValue="0" MaxValue="10" ShowSpinButtons="true"></dnn:DnnNumericTextBox>
        </div>
        <div class="dnnFormItem">
            <dnn:Label ID="plLockoutDuration" runat="server" ControlName="tpLockoutDuration" />
            <wes:TimeSpanPicker id="tpLockoutDuration" runat="server"></wes:TimeSpanPicker>
        </div>
        <div class="dnnFormItem">
            <dnn:Label ID="plRequireModeration" runat="server" ControlName="cbRequireModeration" />
            <asp:CheckBox ID="cbRequireModeration" runat="server" />
        </div>
    </fieldset>
    <h2 id="Panel-ProcessorSettings" class="dnnFormSectionHead"><a href="" class="dnnSectionExpanded"><%=LocalizeString("ProcessorSettings") %></a></h2>
    <fieldset>
        <div class="dnnFormItem">
            <dnn:Label ID="plProcessorPageName" runat="server" ControlName="tbProcessorPageName" CssClass="dnnFormRequired" />
            <asp:TextBox ID="tbProcessorPageName" runat="server"></asp:TextBox>
            <asp:RequiredFieldValidator ID="valProcessorPageName" ControlToValidate="tbProcessorPageName" runat="server"
                        ResourceKey="valProcessorPageName.Required" Display="Dynamic" CssClass="dnnFormMessage dnnFormError" SetFocusOnError="true" />

        </div>
        <div class="dnnFormItem">
            <dnn:Label ID="plAllowUsernameModification" runat="server" ControlName="cbAllowUsernameModification" />
            <asp:CheckBox ID="cbAllowUsernameModification" runat="server" />
        </div>
        <div class="dnnFormItem">
            <dnn:Label ID="plAllowFirstLastNameModification" runat="server" ControlName="cbAllowFirstLastNameModification" />
            <asp:CheckBox ID="cbAllowFirstLastNameModification" runat="server" />
        </div>
        <div class="dnnFormItem">
            <dnn:Label ID="plAllowDisplayNameModification" runat="server" ControlName="cbAllowDisplayNameModification" />
            <asp:CheckBox ID="cbAllowDisplayNameModification" runat="server" />
        </div>
        <div class="dnnFormItem">
            <dnn:Label ID="plRequireTemporaryPasswordEntry" runat="server" ControlName="cbRequireTemporaryPasswordEntry" />
            <asp:CheckBox ID="cbRequireTemporaryPasswordEntry" runat="server" />
        </div>
        <div id="divRequirePasswordChangeOnAcceptance" class="dnnFormItem">
            <dnn:Label ID="plRequirePasswordChangeOnAcceptance" runat="server" ControlName="cbRequirePasswordChangeOnAcceptance" />
            <asp:CheckBox ID="cbRequirePasswordChangeOnAcceptance" runat="server" />
        </div>
        <div class="dnnFormItem">
            <dnn:Label ID="plRequirePasswordConfirmOnChange" runat="server" ControlName="cbRequirePasswordConfirmOnChange" />
            <asp:CheckBox ID="cbRequirePasswordConfirmOnChange" runat="server" />
        </div>
        <div class="dnnFormItem">
            <dnn:Label ID="plEnableReasonDeclinedField" runat="server" ControlName="cbEnableReasonDeclinedField" />
            <asp:CheckBox ID="cbEnableReasonDeclinedField" runat="server" />
        </div>
        <div class="dnnFormItem">
            <dnn:Label ID="plSuspendInvitationProcessing" runat="server" ControlName="cbSuspendInvitationProcessing" />
            <asp:CheckBox ID="cbSuspendInvitationProcessing" runat="server" />
        </div>
    </fieldset>
    <h2 id="Panel-NotificationSettings" class="dnnFormSectionHead"><a href="" class="dnnSectionExpanded"><%=LocalizeString("NotificationSettings") %></a></h2>
    <fieldset>
        <div class="dnnFormItem">
            <dnn:Label ID="plSendFrom" runat="server" ControlName="tbSendFrom" />
            <asp:TextBox ID="tbSendFrom" runat="server"></asp:TextBox>
        </div>
        <div class="dnnFormItem">
            <dnn:Label ID="plSendCopiesTo" runat="server" ControlName="tbSendCopiesTo" />
            <asp:TextBox ID="tbSendCopiesTo" runat="server"></asp:TextBox>
        </div>
        <div class="dnnFormItem">
            <dnn:Label ID="plSendCopiesToRoles" runat="server" ControlName="ctlSendCopiesToRoles" />
            <div class="dnnRolesGrid"><dnn:RolesSelectionGrid runat="server" ID="dgSelectedRoles" /></div>
        </div>
        <div class="dnnFormItem">
            <dnn:Label ID="plEnabledInvitingUserNotifications" runat="server" ControlName="cblEnabledInvitingUserNotifications" />
            <asp:CheckBoxList ID="cblEnabledInvitingUserNotifications" runat="server" CssClass="dnnCheckBoxList" RepeatColumns="2" RepeatDirection="Horizontal" />
        </div>
        <div class="dnnFormItem">
            <dnn:Label ID="plEnabledAdminUserNotifications" runat="server" ControlName="cblEnabledAdminUserNotifications" />
            <asp:CheckBoxList ID="cblEnabledAdminUserNotifications" runat="server" CssClass="dnnCheckBoxList" RepeatColumns="2" RepeatDirection="Horizontal" />
        </div>
    </fieldset>
    <h2 id="Panel-RoleBasedLimits" class="dnnFormSectionHead"><a href=""><%=LocalizeString("RoleBasedLimits") %></a></h2>
    <fieldset>
        <div class="dnnFormItem">
            <wes:RoleBasedLimitsGrid id="ctlRoleBasedLimitsGrid" runat="server"></wes:RoleBasedLimitsGrid>
        </div>
    </fieldset>
</div>
<script language="javascript" type="text/javascript">
    /*globals jQuery, window, Sys */
    (function ($, Sys) {
        function setUpWESNet_ByInvitation() {
            $('#WESNet_ByInvitation_Settings .dnnFormExpandContent a').dnnExpandAll({
                expandText: '<%=Localization.GetSafeJSString("ExpandAll", Localization.SharedResourceFile)%>',
                collapseText: '<%=Localization.GetSafeJSString("CollapseAll", Localization.SharedResourceFile)%>',
                targetArea: '#WESNet_ByInvitation_Settings'
            });
            toggleSection("divCaptchaSettings", $("#<%=rblEnableInvitationCaptcha.ClientID %> input:radio:checked").value == "0");
            $("#<%=rblEnableInvitationCaptcha.ClientID %> input:radio").click(function (e) {
                toggleSection('divCaptchaSettings', this.value != "0");
            });
            toggleSection("divRequireTemporaryPasswordConfirm", $("#<%=rblTemporaryPasswordMode.ClientID %> input:radio:checked").valueOf <= "1");
            toggleSection("divRequirePasswordChangeNextLogin", $("#<%=rblTemporaryPasswordMode.ClientID %> input:radio:checked").valueOf == "0");
            $("#<%=rblTemporaryPasswordMode.ClientID %> input:radio").click(function (e) {
                toggleSection('divRequireTemporaryPasswordConfirm', this.value > "1");
                toggleSection('divRequirePasswordChangeNextLogin', this.value > "0");
            });
        }

        $(document).ready(function () {
            setUpWESNet_ByInvitation();
            Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function () {
                setUpWESNet_ByInvitation();
            });
        });

        function toggleSection(id, isToggled) {
            $("div[id$='" + id + "']").toggle(isToggled);
        }
    } (jQuery, window.Sys));
</script>

