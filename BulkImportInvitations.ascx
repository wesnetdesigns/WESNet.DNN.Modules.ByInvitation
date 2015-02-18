<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="BulkImportInvitations.ascx.cs" Inherits="WESNet.DNN.Modules.ByInvitation.BulkImportInvitations" %>
<%@ Import Namespace="DotNetNuke.Web.UI.Webcontrols" %>
<%@ Import Namespace="WESNet.DNN.Modules.ByInvitation.Controls" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Register TagPrefix="dnn" TagName="FilePickerUploader" Src="~/controls/filepickeruploader.ascx" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.UI.WebControls" Assembly="DotNetNuke.Web" %>
<%@ Register TagPrefix="wes" TagName="DataFieldMappingGrid" Src="~/DesktopModules/WESNet_ByInvitation/Controls/DataFieldMappingGrid.ascx" %>

<div class="dnnForm WESNet_ByInvitation dnnClear" id="WESNet_ByInvitation_BulkImport">
    <asp:Wizard ID="wizBulkImport" runat="server"  DisplaySideBar="false" ActiveStepIndex="0" Width="98%"
        CellPadding="5" CellSpacing="5" 
        DisplayCancelButton="True" 
        CancelButtonType="Link"
        StartNextButtonType="Link"
        StepNextButtonType="Link" 
        StepPreviousButtonType="Link"
        FinishCompleteButtonType="Link" 
        >
        <CancelButtonStyle CssClass="dnnSecondaryAction" />
        <StartNextButtonStyle CssClass="dnnPrimaryAction" />
        <StepPreviousButtonStyle CssClass="dnnSecondaryAction" />
        <StepNextButtonStyle CssClass="dnnPrimaryAction" />
        <FinishCompleteButtonStyle CssClass="dnnPrimaryAction" />
        <StepStyle VerticalAlign="Top" />
        <NavigationButtonStyle BorderStyle="None" BackColor="Transparent" />
        <HeaderTemplate>
            <h4><asp:Label ID="lblTitle" CssClass="Head" runat="server"><%=GetActiveStepText("Title") %></asp:Label></h4>
            <asp:Label ID="lblHelp" CssClass="WizardText" runat="server"><%=GetActiveStepText("Help") %></asp:Label>
            <asp:PlaceHolder ID="phMessage" runat="server" Visible="false"></asp:PlaceHolder>
        </HeaderTemplate>
        <StartNavigationTemplate>
            <ul class="dnnActions dnnClear" id="ulStartNavigation" runat="server">
    	        <li><asp:LinkButton id="nextButtonStart" runat="server" CssClass="dnnPrimaryAction" CommandName="MoveNext" resourcekey="Next" /></li>
                <li><asp:LinkButton id="cancelButtonStart" runat="server" CssClass="dnnSecondaryAction" CommandName="Cancel" resourcekey="Cancel" Causesvalidation="False" /></li>
            </ul>
        </StartNavigationTemplate>
        <StepNavigationTemplate>
            <ul class="dnnActions dnnClear">
                <li><asp:LinkButton ID="previousButtonStep" runat="server" CssClass="dnnSecondaryAction" CommandName="MovePrevious" resourceKey="Previous" /></li>
    	        <li><asp:LinkButton id="nextButtonStep" runat="server" CssClass="dnnPrimaryAction" CommandName="MoveNext" resourcekey="Next" /></li>
                <li><asp:LinkButton id="cancelButtonStep" runat="server" CssClass="dnnSecondaryAction" CommandName="Cancel" resourcekey="Cancel" Causesvalidation="False" /></li>
            </ul>
        </StepNavigationTemplate>
        <FinishNavigationTemplate>
            <ul class="dnnActions dnnClear">
                <li><asp:LinkButton ID="previousButtonFinish" runat="server" CssClass="dnnSecondaryAction" CommandName="MovePrevious" resourceKey="Previous" /></li>
    	        <li><asp:LinkButton id="finishButtonStep" runat="server" CssClass="dnnPrimaryAction" CommandName="MoveComplete" resourcekey="Finish" /></li>
            </ul>
        </FinishNavigationTemplate>
        <WizardSteps>
            <asp:WizardStep ID="Step0" runat="Server" Title="Introduction" StepType="Start" AllowReturn="true">
                <div id="divResumeWizard" runat="server" visible="false">
                    <asp:Label ID="lblResumeWizard" runat="server" CssClass="WizardHead"><%=LocalizeString("ResumeWizard") %></asp:Label>
                    <ul class="dnnActions dnnClear">
                        <li><asp:LinkButton ID="cmdResume" runat="server" CssClass="dnnPrimaryAction" resourcekey="cmdResume" CausesValidation="false" /></li>
                        <li><asp:LinkButton ID="cmdStartOver" runat="server" CssClass="dnnSecondaryAction" resourcekey="cmdStartOver" CausesValidation="false" /></li>
                        <li><asp:LinkButton ID="cmdCancel" runat="server" CssClass="dnnSecondaryAction" resourceKey="cmdCancel" CausesValidation="false" /></li>
                    </ul>
                </div>
                <div id="divStep0Form" runat="server" class="dnnForm">
                    <div class="dnnFormItem">
                        <dnn:Label ID="plImportSourceType" runat="server" ControlName="ddlImportFileType" />
                        <asp:DropDownList ID="ddlImportSourceType" runat="server">
                            <asp:ListItem Value="0" ResourceKey="CSV"></asp:ListItem>
                            <asp:ListItem Value="1" ResourceKey="Excel"></asp:ListItem>
                            <asp:ListItem Value="2" ResourceKey="XML"></asp:ListItem>
                            <asp:ListItem Value="3" ResourceKey="DatabaseFile"></asp:ListItem>
                            <asp:ListItem Value="4" ResourceKey="DatabaseConnection"></asp:ListItem>
                        </asp:DropDownList>
                    </div>                 
                </div>               
            </asp:WizardStep>
            <asp:WizardStep ID="Step1" runat="server" Title="SelectDataSource" StepType="Step" AllowReturn="true">
                <div class="dnnForm">
                    <div class="dnnFormItem">
                        <asp:Label ID="lblSpecificFileTypeHelp" runat="server" cssClass="WizardText" />
                    </div>
                    <div id="divFileUpload" runat="server" class="dnnFormItem">
                        <dnn:Label ID="plImportFile" runat="server" ControlName="ctlImportFile" />
                        <asp:DropDownList ID="ddlImportFilename" runat="server" />
                        <input id="ctlImportFile" type="file" size="30" runat="server" class="normalFileUpload" />              
                    </div>
                    <div id="divConnectionStringKey" runat="server" class="dnnFormItem" visible="false">
                        <dnn:Label id="plConnectionStringKey" runat="server" ControlName="ddlConnectionStringKey" />
                        <asp:DropDownList ID="ddlConnectionStringKey" runat="server" AutoPostBack ="true"></asp:DropDownList>
                    </div>
                    <div id="divDatabaseConnection" runat="server">
                        <div id="divSQLConnection" runat="server">
                            <div class="dnnFormItem">
                                <dnn:Label ID="plDataSource" runat="server" ControlName="tbDataSource" />
                               <asp:TextBox ID="tbDataSource" runat="server"></asp:TextBox>                        
                            </div>
                            <div class="dnnFormItem">
                                <dnn:Label ID="plDatabase" runat="server" ControlName="tbServer" />
                                <asp:TextBox ID="tbDatabase" runat="server"></asp:TextBox>                        
                            </div>
                            <div class="dnnFormItem">
                                <dnn:Label id="plIntegratedSecurity" runat="server" ControlName="cbIntegratedSecurity" />
                                <asp:CheckBox ID="cbIntegratedSecurity" runat="server" CssClass="normalCheckBox" />
                            </div>
                        </div>
                        <div id="divDBCredentials" runat="server">
                            <div class="dnnFormItem">
                                <dnn:Label ID="plDbUser" runat="server" ControlName="tbDbUser" />
                                <asp:TextBox ID="tbDbUser" runat="server"></asp:TextBox>                        
                            </div>
                            <div class="dnnFormItem">
                                <dnn:Label ID="plDbPassword" runat="server" ControlName="tbDbPassword" />
                                <asp:TextBox ID="tbDbPassword" runat="server" TextMode="Password"></asp:TextBox>                    
                            </div>
                        </div>
                    </div>
                </div>
            </asp:WizardStep>
            <asp:WizardStep ID="Step2" runat="server" Title="Review" StepType="Step" AllowReturn="true">
                <div class="dnnForm">                  
                    <div id="divCSVOptions" runat="server">
                        <div class="dnnFormItem">
                            <dnn:Label ID="plCSVDelimiter" runat="server" ControlName="rblCSVDelimiter"></dnn:Label>
                            <asp:RadioButtonList ID="rblCSVDelimiter" runat="server" class="dnnFormRadioButtons" RepeatDirection="Horizontal" AutoPostBack="true">
                                <asp:ListItem Value="0" ResourceKey="Comma"></asp:ListItem>
                                <asp:ListItem Value="1" ResourceKey="SemiColon"></asp:ListItem>
                                <asp:ListItem Value="2" ResourceKey="Tab"></asp:ListItem>
                                <asp:ListItem Value="3" ResourceKey="Space"></asp:ListItem>
                                <asp:ListItem Value="4" ResourceKey="Pipe"></asp:ListItem>
                            </asp:RadioButtonList>                      
                        </div>
                        <div class="dnnFormItem">
                            <dnn:Label ID="plCSVFirstRow" runat="server" ControlName="cbCSVFirstRow"></dnn:Label>
                            <asp:CheckBox ID="cbCSVFirstRow" runat="server" AutoPostBack="true" />
                        </div>
                    </div>
                    <div id="divExcelOptions" runat="server">
                        <div class="dnnFormItem">
                            <dnn:Label ID="plExcelFirstRow" runat="server" ControlName="cbExcelFirstRow"></dnn:Label>
                            <asp:CheckBox ID="cbExcelFirstRow" runat="server" Checked="true" AutoPostBack="true" />
                        </div>
                    </div>
                    <div id="divXMLOptions" runat="server">
                        <div class="dnnFormItem">
                            <dnn:Label id="plItemNodeName" runat="server" ControlName="ddlItemNodeName"></dnn:Label>
                            <asp:DropDownList ID="ddlItemNodeName" runat="server" AutoPostBack ="true" />
                        </div>
                    </div>
                    <div id="divDatabaseOptions" runat="server">
                        <div class="dnnFormItem">
                            <dnn:Label ID="plDatabaseTableName" runat="server" ControlName="ddlDatabaseTableName" />
                            <asp:DropDownList ID="ddlDatabaseTableName" runat="server" AutoPostBack="true"></asp:DropDownList>
                        </div>
                    </div>
                    <div id="divPreviewData" runat="server">
                        <div class="dnnFormItem">
                            <dnn:Label ID="plPreviewData" runat="server" ControlName="lblPreviewData" /><br />
                            <asp:GridView ID="grdPreviewData" runat="server" CssClass ="dnnGrid previewData" HeaderStyle-CssClass="dnnGridHeader"
                                RowStyle-CssClass="dnnGridItem" AlternatingRowStyle-CssClass ="dnnGridAltItem" AutoGenerateColumns="true" Width="95%">
                                    <Columns>
                                        <asp:TemplateField ShowHeader="false">
                                            <ItemTemplate>
                                                <asp:Label ID="lblRowNumber" runat="server"></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                    </Columns>
                            </asp:GridView>
                        </div>
                    </div>
                    <div class="dnnFormItem">
                        <dnn:Label ID="plDataFieldMapping" runat="server" ControlName="grdDataFieldMapping" />
                        <wes:DataFieldMappingGrid id="grdDataFieldMap" runat="server"></wes:DataFieldMappingGrid>
                    </div>
                </div>
            </asp:WizardStep>
            <asp:WizardStep ID="Step3" runat="server" Title="SetCreationOptions" StepType="Step">
                <div class="dnnForm">
                    <div id="divSenderEmail" class="dnnFormItem">
                        <dnn:Label ID="plSenderEmail" runat="server" ControlName="tbSenderEmail"  />
                        <asp:TextBox ID="tbSenderEmail" runat="server"/>
                        <asp:RegularExpressionValidator ID="valSenderEmail" ControlToValidate="tbSenderEmail" runat="server"
                        ResourceKey="valSenderEmail.RegexInvalid" Display="Dynamic" CssClass="dnnFormMessage dnnFormError" SetFocusOnError="true" />
                    </div>
                    <div id = "divCultureCode" runat="server" class="dnnFormItem">
                        <dnn:Label ID="plRecipientCultureCode" runat="server" ControlName="ctlCultureCode" />
                        <dnn:DnnLanguageComboBox id="ctlCultureCode" runat="server"></dnn:DnnLanguageComboBox>
                    </div>
                    <div id="divPersonalNote" runat="server" class="dnnFormItem">
                        <dnn:Label ID="plPersonalNote" runat="server" ControlName="tbPersonalNote" />
                        <asp:TextBox ID="tbPersonalNote" runat="server" TextMode="MultiLine" Rows="10" />
                    </div> 
                     <div class="dnnFormItem" id="divRedirection" runat="server">
                        <dnn:Label ID="plRedirectOnFirstLogin" runat="server" ControlName="ddlRedirectOnFirstLogin" />
                        <asp:DropDownList ID="ddlRedirectOnFirstLogin" runat="server" DataTextField="IndentedTabName" DataValueField="TabName"></asp:DropDownList>
                    </div>
                    <div class ="dnnFormItem">
                        <dnn:Label id="plSendMethod" runat="server" ControlName="rblSendMethod" />
                        <asp:RadioButtonList ID="rblSendMethod" runat="server" CssClass="dnnFormRadioButtons">
                            <asp:ListItem Value="0" ResourceKey="Sync" Selected ="true"></asp:ListItem>
                            <asp:ListItem Value="1" ResourceKey="Async"></asp:ListItem>
                            <asp:ListItem Value="2" ResourceKey="Scheduled"></asp:ListItem>
                        </asp:RadioButtonList>
                    </div>
                    <div id="divScheduledJobOptions" runat="server">
                        <div class="dnnFormItem">
                            <dnn:Label id="plScheduledJobStartsAt" runat="server" ControlName="ctlScheduledJobStartsAt" />
                            <dnn:DnnDateTimePicker id="ctlScheduledJobStartsAt" runat="server" CssClass="WESNet_ShortDatePicker"></dnn:DnnDateTimePicker>
                        </div>
                        <div class="dnnFormItem">
                            <dnn:Label id="plSendBatchSize" runat="server" ControlName="ctlSendBatchSize" />
                            <dnn:DnnNumericTextBox id="ctlSendBatchSize" runat="server" Type="Number" NumberFormat-DecimalDigits="0" MinValue="0" MaxValue="500"></dnn:DnnNumericTextBox>
                        </div>
                        <div class="dnnFormItem">
                            <dnn:Label id="plSendBatchInteval" runat="server" />
                            <dnn:DnnNumericTextBox id="ctlSendBatchInteval" runat="server" Type="Number" NumberFormat-DecimalDigits="0" MinValue="0" MaxValue="2880"></dnn:DnnNumericTextBox>
                        </div>
                    </div>
                </div>
            </asp:WizardStep>
            <asp:WizardStep ID="Step4" runat="server" Title="CreateAndSendInvitations" StepType="Step">
                <div class="dnnForm">
                    <asp:Label ID="lblSelectedSendMethodHelp" runat="server" CssClass ="dnnFormMessage dnnFormInformation" />
                    <div id="divProgressBar" runat="server" class="ProgressBar">
                        Bulk import is starting . . . do NOT close or navigate from this page until completed!!! <br />
                        <asp:Image id="imgProgressBar" runat="server" ImageUrl="~/DesktopModules/WESNet_ByInvitation/images/bulkimportprogressbar.gif" />
                    </div>                    
                </div>
            </asp:WizardStep>
            <asp:WizardStep ID="Step5" runat="server" Title="Completion" StepType="Finish">
                <div class="dnnForm">
                    <dnn:Label ID="plCompletionSummary" runat="server" ControlName="divCompletionSummary" />
                    <div id="divCompletionSummary" runat="server" class="CompletionSummary"></div>
                </div>   
            </asp:WizardStep>
        </WizardSteps>
    </asp:Wizard>
    <script type="text/javascript">
        (function ($, Sys) {
            function setUpWESNet_BulkImportInvitations() {

                var uploadFileText = '<%= DotNetNuke.UI.Utilities.ClientAPI.GetSafeJSString(LocalizeString("UploadFile")) %>';
                var $ddlImportFilename = $('#<%=ddlImportFilename.ClientID%>');
                var fileInputWrapper = $("<span class='dnnInputFileWrapper dnnSecondaryAction'></span>").text(uploadFileText);
                var $importFile = $('#<%=ctlImportFile.ClientID%>');
                $importFile.wrap(fileInputWrapper);
                
                $importFile.change(function() {
                    var filename = $(this).val();
                    if (filename != '') {
                        var lastIdx = filename.lastIndexOf('\\') + 1;
                        filename = filename.substring(lastIdx, filename.length);
                        $ddlImportFilename.hide();
                    } else {
                        filename = uploadFileText;
                        $ddlImportFilename.show();
                    }
                    $(this).parent().get(0).childNodes[0].nodeValue = filename;        
                });

                $('#<%=divDBCredentials.ClientID %>').toggle("<%=cbIntegratedSecurity.Checked %>" == "False");
                $('#<%=divScheduledJobOptions.ClientID %>').toggle("<%=rblSendMethod.SelectedValue %>" == "2");

                $('#<%=cbIntegratedSecurity.ClientID %>').change(function () {
                    $('#<%=divDBCredentials.ClientID %>').toggle(!this.checked);
                });

                $('#<%=rblSendMethod.ClientID %> input[type="radio"]').change(function () {
                     $('#<%=divScheduledJobOptions.ClientID %>').toggle($(this).val() == "2");                        
                });

                var $divProgressBar = $('#<%=divProgressBar.ClientID%>');

                function ShowProgressBar() {
                    setTimeout(function () {
                        var modal = $('<div />');
                        modal.addClass("progressBarModal");
                        $('body').append(modal);
                        $divProgressBar.show();
                        var top = Math.max($(window).height() / 2 - $divProgressBar[0].offsetHeight / 2, 0);
                        var left = Math.max($(window).width() / 2 - $divProgressBar[0].offsetWidth / 2, 0);
                        $divProgressBar.css({ top: top, left: left });
                    }, 200);
                }

                var step = <%=wizBulkImport.ActiveStepIndex%>;

                var $nextButtonStep = $('#<%=wizBulkImport.ClientID + "_StepNavigationTemplateContainerID_nextButtonStep"%>');
                if (step == 4 && $nextButtonStep)
                {
                    $nextButtonStep.click(function() {
                        ShowProgressBar();
                    });
                }

            };
            
            $(document).ready(function () {
            setUpWESNet_BulkImportInvitations();
            Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function () {
                setUpWESNet_UserExport();
            });
        });

     }(jQuery, window.Sys));
    </script>
</div>
