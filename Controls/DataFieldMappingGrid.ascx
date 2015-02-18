<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DataFieldMappingGrid.ascx.cs" Inherits="WESNet.DNN.Modules.ByInvitation.Controls.DataFieldMappingGrid" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>

<dnn:DnnJsInclude runat="server" PathNameAlias="SharedScripts" FilePath="knockout.js" Priority="101" />

<div id="divDataFieldMappingGrid" runat="server">
    <table id="tblDataFieldMappingGrid" class="dnnGrid" >
        <thead>
            <tr class="dnnGridHeader">
                <th ></th>
                <!-- ko foreach: InvitationFields -->
                <th style="text-align: center; vertical-align:bottom" data-bind="text: LocalizedFieldName, css:{ dnnFormRequired: IsRequired }"></th>
                <!-- /ko -->
            </tr>
        </thead>
        <tbody data-bind="foreach: { data: ImportFields, as: 'importField' }">
            <tr data-bind="css: $root.itemOrAltItemCSS($index())">
                <td style="width:100px; text-align:right" data-bind="text: ImportFieldName"></td>
                <!-- ko foreach: $root.InvitationFields -->
                <td style="width:32px;text-align:center">
                    <span style="width: 16px; height: 16px; line-height: 16px; display: inline-block"
                         data-bind="click: function () { $root.toggleMapping(importField.ImportFieldIndex, $data.FieldIndex); }, css: $root.mappingCSS(importField.ImportFieldIndex, $data.FieldIndex)"></span>
                </td>
                <!-- /ko -->
            </tr>
        </tbody>
    </table>
    <asp:HiddenField ID="hfInvitationFields" runat="server" />
    <asp:HiddenField ID="hfImportFields" runat="server" />
    <script type="text/javascript">
        $(function () {
            var settings = {
                hfImportFields: '#<%= hfImportFields.ClientID %>',
                hfInvitationFields: '#<%= hfInvitationFields.ClientID %>',
                mappedAltText: '<%= MappedAltText %>',
                unmappedAltText: '<%= UnmappedAltText %>'
        };
        $('#<%= divDataFieldMappingGrid.ClientID %>').dataFieldMappingGrid(settings);
    });
</script>
</div>
