<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ManageInvitations.ascx.cs" Inherits="WESNet.DNN.Modules.ByInvitation.ManageInvitations" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.UI.WebControls" Assembly="DotNetNuke.Web" %>
<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>

<div class="dnnForm dnnClear WESNet_ByInvitation_ManageInvitations" id="WESNet_ByInvitation_ManageInvitations">
    <div id="manageInvitationsContent" class="dnnClear">
        <fieldset>
            <div class="dnnFormExpandContent"><a href=""><%=Localization.GetString("ExpandAll", Localization.SharedResourceFile)%></a></div>
            <div id="divInvitingUserSelector" runat="server" class="dnnFormItem">
                <dnn:Label ID="plInvitingUser" runat="server" ControlName="ddlInvitingUser" />
                <asp:DropDownList ID="ddlInvitingUser" runat="server" DataTextField="InvitedByUserDisplayName" DataValueField="Value"></asp:DropDownList>
            </div>
            <div class="dnnFormItem">
                <dnn:Label ID="plDateRange" runat="server" ControlName="tbStartDate" />
                <div class="WESNet_DateRange">
                    <asp:Label ID="plFromDate" runat="server" ResourceKey="plFromDate" AssociatedControlID="tbFromDate"></asp:Label>
                    <dnn:DnnDatePicker ID="tbFromDate" runat="server" CssClass="WESNet_ShortDatePicker"></dnn:DnnDatePicker>
                    <asp:Label ID="plThroughDate" runat="server" ResourceKey="plThroughDate" AssociatedControlID="tbThroughDate"></asp:Label>
                    <dnn:DnnDatePicker ID="tbThroughDate" runat="server" CssClass="WESNet_ShortDatePicker"></dnn:DnnDatePicker>
                </div>
            </div>
            <div class="dnnFormItem">
                <dnn:Label ID="plShowArchived" runat="server" ControlName="cbShowArchived" />
                <asp:CheckBox ID="cbShowArchived" runat="server" />
                <asp:LinkButton ID="lnkSearchAgain" runat="server" CssClass="dnnSecondaryAction" resourcekey="lnkSearchAgain" CausesValidation="false" />
            </div>
        </fieldset>
        <h2 id="H1" class="dnnFormSectionHead"><a href="" class="dnnSectionExpanded"><%=LocalizeString("InvitingUserStats") + " - " + InvitingUserDisplayName %></a></h2>
        <fieldset>
            <div id="divInvitingUserStats" runat="server">
                <div class="dnnFormItem">
                    <dnn:Label id="plUserDisplayName" runat="server" ControlName="lblUserDisplayName" />
                    <asp:Label ID="lblUserDisplayName" runat="server"></asp:Label>
                </div>
                <div class="dnnFormItem">
                    <dnn:Label id="plTotalInvitationCount" runat="server" ControlName="lblTotalInvitationCount" />
                    <asp:Label ID="lblTotalInvitationCount" runat="server"></asp:Label>
                </div>
                <div id="divModeratedStats" runat="server">
                    <div class="dnnFormItem">
                        <dnn:Label id="plApprovedInvitationCount" runat="server" ControlName="lblApprovedInvitationCount" />
                        <asp:Label ID="lblApprovedInvitationCount" runat="server"></asp:Label>
                    </div>
                    <div class="dnnFormItem">
                        <dnn:Label id="plDisapprovedInvitationCount" runat="server" ControlName="lblDisapprovedInvitationCount" />
                        <asp:Label ID="lblDisapprovedInvitationCount" runat="server"></asp:Label>
                    </div>
                </div>
                <div class="dnnFormItem">
                    <dnn:Label id="plSentInvitationCount" runat="server" ControlName="lblSentInvitationCount" />
                    <asp:Label ID="lblSentInvitationCount" runat="server"></asp:Label>
                </div>
                <div class="dnnFormItem">
                    <dnn:Label id="plRetractedInvitationCount" runat="server" ControlName="lblRetractedInvitationCount" />
                    <asp:Label ID="lblRetractedInvitationCount" runat="server"></asp:Label>
                </div>
                <div class="dnnFormItem">
                    <dnn:Label id="plDeclinedInvitationCount" runat="server" ControlName="lblDeclinedInvitationCount" />
                    <asp:Label ID="lblDeclinedInvitationCount" runat="server"></asp:Label>
                </div>
                <div class="dnnFormItem">
                    <dnn:Label id="plAcceptedInvitationCount" runat="server" ControlName="lblAcceptedInvitationCount" />
                    <asp:Label ID="lblAcceptedInvitationCount" runat="server"></asp:Label>
                </div>
                <div class="dnnFormItem">
                    <dnn:Label id="plExpiredInvitationCount" runat="server" ControlName="lblExpiredInvitationCount" />
                    <asp:Label ID="lblExpiredInvitationCount" runat="server"></asp:Label>
                </div>
                <div class="dnnFormItem">
                    <dnn:Label id="plPendingInvitationCount" runat="server" ControlName="lblPendingInvitationCount" />
                    <asp:Label ID="lblPendingInvitationCount" runat="server"></asp:Label>
                </div>
                <div class="dnnFormItem">
                    <dnn:Label id="plLastInvitedOnDate" runat="server" ControlName="lblLastInvitedOnDate" />
                    <asp:Label ID="lblLastInvitedOnDate" runat="server"></asp:Label>
                </div>
            </div>
        </fieldset>
        <h2 id="dnnPanel-Unmoderated" class="dnnFormSectionHead"><a href="" class="dnnSectionExpanded"><%=LocalizeString("Unmoderated")%></a></h2>
        <fieldset>
            <div class="dnnFormItem">
                <dnn:DnnGrid ID="dgUnmoderatedInvitations" runat="server" AutoGenerateColumns="false" AllowSorting="true" AllowPaging="true"
                    ClientSettings_Selecting_AllowRowSelect="true" CssClass="dnnGrid">
                    <headerstyle cssclass="dnnGridHeader" horizontalalign="Center" />
                    <itemstyle cssclass="dnnGridItem" horizontalalign="Center" />
                    <alternatingitemstyle cssclass="dnnGridAltItem" horizontalalign="Center" />
                    <pagerstyle mode="NumericPages" />
                    <mastertableview datakeynames="InvitationID, InvitedByUserID" pagesize="10">
                        <SortExpressions>
                            <telerik:GridSortExpression FieldName="InvitedByUserDisplayName" SortOrder="None" />
                            <telerik:GridSortExpression FieldName="RecipientFullName" SortOrder="None" />
                            <telerik:GridSortExpression FieldName="RecipientEmail" SortOrder="None" />
                            <telerik:GridSortExpression FieldName="InvitedOnDate" SortOrder="None" />                          
                        </SortExpressions>
                        <Columns>
                            <dnn:DnnGridTemplateColumn UniqueName="Commands">
                                <ItemTemplate>
                                    <dnn:DnnImageButton id="btnApprove" IconKey="Approve" IconSize="16x16" CommandName="Approve" ResourceKey="Approve" runat="server" />
                                    <dnn:DnnImageButton id="btnDisapprove" IconKey="Reject" iconSize="16x16" CommandName="Disapprove" ResourceKey="Disapprove" runat="server" />
                                    <dnn:DnnImageButton ID="btnEditApprove" IconKey="Edit" IconSize="16x16" CommandName="EditApprove" ResourceKey="EditApprove"  runat="server" />
                                    <dnn:DnnImageButton id="btnDeleteUnmoderated" IconKey="Delete" IconSize="16x16" CommandName="Delete" ResourceKey="Delete" runat="server" />
                                </ItemTemplate>
                            </dnn:DnnGridTemplateColumn>
                            <dnn:DnnGridBoundColumn DataField="InvitedByUserDisplayName" HeaderText="InvitedBy"></dnn:DnnGridBoundColumn>
                            <dnn:DnnGridBoundColumn DataField="RecipientFullName" HeaderText="Recipient"></dnn:DnnGridBoundColumn>
                            <dnn:DnnGridBoundColumn DataField="RecipientEmail" HeaderText="Email"></dnn:DnnGridBoundColumn>
                            <dnn:DnnGridTemplateColumn HeaderText="InvitedOnDate">
                                <ItemTemplate>
                                    <asp:Label id="lblInvitedOnDateUnmoderated" runat="server" Text='<%# FormatedUTCDate((DateTime)Eval("InvitedOnDate")) %>'/>
                                </ItemTemplate>
                            </dnn:DnnGridTemplateColumn>             
                        </Columns>
                    </mastertableview>
                </dnn:DnnGrid>
            </div>
        </fieldset>
        <h2 id="dnnPanel-PendingInvitations" class="dnnFormSectionHead"><a href="" class="dnnSectionExpanded"><%=LocalizeString("PendingInvitations")%></a></h2>
        <fieldset>
            <div class="dnnFormItem">
                <dnn:DnnGrid ID="dgPendingInvitations" runat="server" AutoGenerateColumns="false" AllowSorting="true" AllowPaging="true"
                    ClientSettings_Selecting_AllowRowSelect="true" CssClass="dnnGrid">
                    <headerstyle cssclass="dnnGridHeader" horizontalalign="Center" />
                    <itemstyle cssclass="dnnGridItem" horizontalalign="Center" />
                    <alternatingitemstyle cssclass="dnnGridAltItem" horizontalalign="Center" />
                    <pagerstyle mode="NumericPages" />
                    <mastertableview datakeynames="InvitationID, InvitedByUserID" pagesize="10">
                        <SortExpressions>
                            <telerik:GridSortExpression FieldName="InvitedByUserDisplayName" SortOrder="None" />
                            <telerik:GridSortExpression FieldName="RecipientFullName" SortOrder="None" />
                            <telerik:GridSortExpression FieldName="RecipientEmail" SortOrder="None" />
                            <telerik:GridSortExpression FieldName="InvitedOnDate" SortOrder="None" />
                            <telerik:GridSortExpression FieldName="ExpiresOnDate" SortOrder="None" />                            
                        </SortExpressions>
                        <Columns>
                            <dnn:DnnGridTemplateColumn UniqueName="Commands">
                                <ItemTemplate>
                                    <dnn:DnnImageButton id="btnResendPending" IconKey="Email" IconSize="16x16" CommandName="Resend" ResourceKey="Resend" runat="server" />
                                    <dnn:DnnImageButton id="btnRetractPending" IconKey="Restore" iconSize="16x16" CommandName="Retract" ResourceKey="Retract" runat="server" />
                                    <dnn:DnnImageButton ID="btnEditPending" IconKey="Edit" IconSize="16x16" CommandName="Edit" ResourceKey="Edit" runat="server" />
                                </ItemTemplate>
                            </dnn:DnnGridTemplateColumn>
                            <dnn:DnnGridBoundColumn DataField="InvitedByUserDisplayName" HeaderText="InvitedBy"></dnn:DnnGridBoundColumn>
                            <dnn:DnnGridBoundColumn DataField="RecipientFullName" HeaderText="Recipient"></dnn:DnnGridBoundColumn>
                            <dnn:DnnGridBoundColumn DataField="RecipientEmail" HeaderText="Email"></dnn:DnnGridBoundColumn>
                            <dnn:DnnGridTemplateColumn HeaderText="InvitedOnDate">
                                <ItemTemplate>
                                    <asp:Label id="lblInvitedOnDatePending" runat="server" Text='<%# FormatedUTCDate((DateTime)Eval("InvitedOnDate")) %>'/>
                                </ItemTemplate>
                            </dnn:DnnGridTemplateColumn>
                            <dnn:DnnGridBoundColumn DataField="ResentCount" HeaderText="ResentCount"></dnn:DnnGridBoundColumn>
                            <dnn:DnnGridTemplateColumn HeaderText="LastResentOnDate">
                                <ItemTemplate>
                                    <asp:Label id="lblLastResentOnDate" runat="server" Text='<%# FormatedUTCDate((DateTime)Eval("LastResentOnDate")) %>'/>
                                </ItemTemplate>
                            </dnn:DnnGridTemplateColumn>
                            <dnn:DnnGridTemplateColumn HeaderText="ExpiresOnDate">
                                <ItemTemplate>
                                    <asp:Label id="lblExpiresOnDatePending" runat="server" Text='<%# FormatedUTCDate((DateTime)Eval("ExpiresOnDate")) %>'/>
                                </ItemTemplate>
                            </dnn:DnnGridTemplateColumn>                    
                        </Columns>
                    </mastertableview>
                </dnn:DnnGrid>
            </div>
        </fieldset>
        <h2 id="dnnPanel-RetractedInvitations" class="dnnFormSectionHead"><a href="" class="dnnSectionExpanded"><%=LocalizeString("RetractedInvitations")%></a></h2>
        <fieldset>
            <div class="dnnFormItem">
                <dnn:DnnGrid ID="dgRetractedInvitations" runat="server" AutoGenerateColumns="false" AllowMultiRowSelection="true" AllowSorting="true" AllowPaging="true"
                    ClientSettings_Selecting_AllowRowSelect="true" CssClass="dnnGrid">
                    <headerstyle cssclass="dnnGridHeader" horizontalalign="Center" />
                    <itemstyle cssclass="dnnGridItem" horizontalalign="Center" />
                    <alternatingitemstyle cssclass="dnnGridAltItem" horizontalalign="Center" />
                    <mastertableview datakeynames="InvitationID, InvitedByUserID">
                        <Columns>
                            <dnn:DnnGridTemplateColumn UniqueName="Commands">
                                <ItemTemplate>
                                    <dnn:DnnImageButton id="btnUnRetractRetracted" IconSize="16x16" IconKey="Refresh" CommandName="Unretract" ResourceKey="Unretract" runat="server" />
                                    <dnn:DnnImageButton ID="btnEdit" IconKey="Edit" IconSize="16x16" CommandName="Edit" ResourceKey="Edit" runat="server" />
                                </ItemTemplate>
                            </dnn:DnnGridTemplateColumn>
                            <dnn:DnnGridBoundColumn DataField="InvitedByUserDisplayName" HeaderText="InvitedBy"></dnn:DnnGridBoundColumn>
                            <dnn:DnnGridBoundColumn DataField="RecipientFullName" HeaderText="Recipient"></dnn:DnnGridBoundColumn>
                            <dnn:DnnGridBoundColumn DataField="RecipientEmail" HeaderText="Email"></dnn:DnnGridBoundColumn>
                            <dnn:DnnGridTemplateColumn HeaderText="InvitedOnDate">
                                <ItemTemplate>
                                    <asp:Label id="lblInvitedOnDateRetracted" runat="server" Text='<%# FormatedUTCDate((DateTime)Eval("InvitedOnDate")) %>'/>
                                </ItemTemplate>
                            </dnn:DnnGridTemplateColumn>
                            <dnn:DnnGridTemplateColumn HeaderText="RetractedOnDate">
                                <ItemTemplate>
                                    <asp:Label id="lblRetractedOnDateRetracted" runat="server" Text='<%# FormatedUTCDate((DateTime)Eval("RetractedOnDate")) %>'/>
                                </ItemTemplate>
                            </dnn:DnnGridTemplateColumn>
                            <dnn:DnnGridTemplateColumn HeaderText="ExpiresOnDate">
                                <ItemTemplate>
                                    <asp:Label id="lblExpiresOnDateRetracted" runat="server" Text='<%# FormatedUTCDate((DateTime)Eval("ExpiresOnDate")) %>'/>
                                </ItemTemplate>
                            </dnn:DnnGridTemplateColumn>                      
                        </Columns>
                    </mastertableview>
                </dnn:DnnGrid>
            </div>
        </fieldset>
        <h2 id="dnnPanel-Declined" class="dnnFormSectionHead"><a href="" class="dnnSectionExpanded"><%=LocalizeString("DeclinedInvitations")%></a></h2>
        <fieldset>
            <div class="dnnFormItem">
                <dnn:DnnGrid ID="dgDeclinedInvitations" runat="server" AutoGenerateColumns="false" AllowSorting="true" AllowPaging="true"
                    ClientSettings_Selecting_AllowRowSelect="true" CssClass="dnnGrid">
                    <headerstyle cssclass="dnnGridHeader" horizontalalign="Center" />
                    <itemstyle cssclass="dnnGridItem" horizontalalign="Center" />
                    <alternatingitemstyle cssclass="dnnGridAltItem" horizontalalign="Center" />
                    <mastertableview datakeynames="InvitationID, InvitedByUserID">
                        <Columns>
                            <dnn:DnnGridTemplateColumn UniqueName="Commands">
                                <ItemTemplate>
                                    <dnn:DnnImageButton ID="btnArchiveDeclined" IconSize="16x16" IconKey="Save" CommandName="Archive" ResourceKey="Archive"  runat="server" />
                                </ItemTemplate>
                            </dnn:DnnGridTemplateColumn>
                            <dnn:DnnGridBoundColumn DataField="InvitedByUserDisplayName" HeaderText="InvitedBy"></dnn:DnnGridBoundColumn>
                            <dnn:DnnGridBoundColumn DataField="RecipientFullName" HeaderText="Recipient"></dnn:DnnGridBoundColumn>
                            <dnn:DnnGridBoundColumn DataField="RecipientEmail" HeaderText="Email"></dnn:DnnGridBoundColumn>
                            <dnn:DnnGridTemplateColumn HeaderText="InvitedOnDate">
                                <ItemTemplate>
                                    <asp:Label id="lblInvitedOnDateDeclined" runat="server" Text='<%# FormatedUTCDate((DateTime)Eval("InvitedOnDate")) %>'/>
                                </ItemTemplate>
                            </dnn:DnnGridTemplateColumn>
                            <dnn:DnnGridBoundColumn DataField="ReasonDeclined" HeaderText="ReasonDeclined"></dnn:DnnGridBoundColumn>
                            <dnn:DnnGridTemplateColumn HeaderText="DeclinedOnDate">
                                <ItemTemplate>
                                    <asp:Label id="lblDeclinedOnDateDeclined" runat="server" Text='<%# FormatedUTCDate((DateTime)Eval("DeclinedOnDate")) %>'/>
                                </ItemTemplate>
                            </dnn:DnnGridTemplateColumn>                      
                        </Columns>
                    </mastertableview>
                </dnn:DnnGrid>
            </div>
        </fieldset>
        <h2 id="dnnPanel-ExpiredInvitations" class="dnnFormSectionHead"><a href="" class="dnnSectionExpanded"><%=LocalizeString("ExpiredInvitations")%></a></h2>
        <fieldset>
            <div class="dnnFormItem">
                <dnn:DnnGrid ID="dgExpiredInvitations" runat="server" AutoGenerateColumns="false" AllowSorting="true" AllowPaging="true"
                    ClientSettings_Selecting_AllowRowSelect="true" CssClass="dnnGrid">
                    <headerstyle cssclass="dnnGridHeader" horizontalalign="Center" />
                    <itemstyle cssclass="dnnGridItem" horizontalalign="Center" />
                    <alternatingitemstyle cssclass="dnnGridAltItem" horizontalalign="Center" />
                    <mastertableview datakeynames="InvitationID, InvitedByUserID">
                        <Columns>
                            <dnn:DnnGridTemplateColumn UniqueName="Commands">
                                <ItemTemplate>
                                    <dnn:DnnImageButton id="btnExtendExpired" IconSize="16x16" IconKey="Add" CommandName="Extend" ResourceKey="Extend" runat="server" />
                                    <dnn:DnnImageButton ID="btnArchiveExpired" IconKey="Save" IconSize="16x16" CommandName="Archive" ResourceKey="Archive"  runat="server" />
                                    <dnn:DnnImageButton id="btnDeleteExpired" IconKey="Delete" IconSize="16x16" CommandName="Delete" ResourceKey="Delete" runat="server" />
                                </ItemTemplate>
                            </dnn:DnnGridTemplateColumn>
                            <dnn:DnnGridBoundColumn DataField="InvitedByUserDisplayName" HeaderText="InvitedBy"></dnn:DnnGridBoundColumn>
                            <dnn:DnnGridBoundColumn DataField="RecipientFullName" HeaderText="Recipient"></dnn:DnnGridBoundColumn>
                            <dnn:DnnGridBoundColumn DataField="RecipientEmail" HeaderText="Email"></dnn:DnnGridBoundColumn>
                            <dnn:DnnGridTemplateColumn HeaderText="InvitedOnDate">
                                <ItemTemplate>
                                    <asp:Label id="lblInvitedOnDateExpired" runat="server" Text='<%# FormatedUTCDate((DateTime)Eval("InvitedOnDate")) %>'/>
                                </ItemTemplate>
                            </dnn:DnnGridTemplateColumn>
                            <dnn:DnnGridBoundColumn DataField="ResentCount" HeaderText="ResentCount"></dnn:DnnGridBoundColumn>
                            <dnn:DnnGridTemplateColumn HeaderText="ResentOnDate">
                                <ItemTemplate>
                                    <asp:Label id="lblLastResentOnDateExpired" runat="server" Text='<%# FormatedUTCDate((DateTime)Eval("LastResentOnDate")) %>'/>
                                </ItemTemplate>
                            </dnn:DnnGridTemplateColumn>
                            <dnn:DnnGridTemplateColumn HeaderText="ExpiresOnDate">
                                <ItemTemplate>
                                    <asp:Label id="lblExpiresOnDateExpired" runat="server" Text='<%# FormatedUTCDate((DateTime)Eval("ExpiresOnDate")) %>'/>
                                </ItemTemplate>
                            </dnn:DnnGridTemplateColumn>                      
                        </Columns>
                    </mastertableview>
                </dnn:DnnGrid>
            </div>
        </fieldset>
        <h2 id="dnnPanel-AcceptedInvitations" class="dnnFormSectionHead"><a href="" class="dnnSectionExpanded"><%=LocalizeString("AcceptedInvitations")%></a></h2>
        <fieldset>
            <div class="dnnFormItem">
                <dnn:DnnGrid ID="dgAcceptedInvitations" runat="server" AutoGenerateColumns="false" AllowSorting="true" AllowPaging="true"
                    ClientSettings_Selecting_AllowRowSelect="true" CssClass="dnnGrid">
                    <headerstyle cssclass="dnnGridHeader" horizontalalign="Center" />
                    <itemstyle cssclass="dnnGridItem" horizontalalign="Center" />
                    <alternatingitemstyle cssclass="dnnGridAltItem" horizontalalign="Center" />
                    <mastertableview datakeynames="InvitationID, InvitedByUserID">
                        <Columns>
                            <dnn:DnnGridTemplateColumn UniqueName="Commands">
                                <ItemTemplate>
                                    <dnn:DnnImageButton ID="btnArchiveAccepted" IconKey="Save" IonSize="16x16" CommandName="Archive" ResourceKey="Archive"  runat="server" />
                                </ItemTemplate>
                            </dnn:DnnGridTemplateColumn>
                            <dnn:DnnGridBoundColumn DataField="InvitedByUserDisplayName" HeaderText="InvitedBy"></dnn:DnnGridBoundColumn>
                            <dnn:DnnGridBoundColumn DataField="RecipientFullName" HeaderText="Recipient"></dnn:DnnGridBoundColumn>
                            <dnn:DnnGridBoundColumn DataField="RecipientEmail" HeaderText="Email"></dnn:DnnGridBoundColumn>
                            <dnn:DnnGridTemplateColumn HeaderText="InvitedOnDate">
                                <ItemTemplate>
                                    <asp:Label id="lblInvitedOnDateAccepted" runat="server" Text='<%# FormatedUTCDate((DateTime)Eval("InvitedOnDate")) %>'/>
                                </ItemTemplate>
                            </dnn:DnnGridTemplateColumn>
                            <dnn:DnnGridTemplateColumn HeaderText="AcceptedOnDate">
                                <ItemTemplate>
                                    <asp:Label id="lblAcceptedOnDateAccepted" runat="server" Text='<%# FormatedUTCDate((DateTime)Eval("AcceptedOnDate")) %>'/>
                                </ItemTemplate>
                            </dnn:DnnGridTemplateColumn>
                            <dnn:DnnGridBoundColumn DataField="AssignedUsername" HeaderText="Username"></dnn:DnnGridBoundColumn>                     
                        </Columns>
                    </mastertableview>
                </dnn:DnnGrid>
            </div>
        </fieldset>
    </div>
    <ul id="divActions" runat="server" class="dnnActions dnnClear">
        <li>
            <asp:LinkButton ID="cmdReturn" runat="server" CssClass="dnnSecondaryAction" resourcekey="cmdReturn" />
        </li>
    </ul>
</div>
<script language="javascript" type="text/javascript">
    /*globals jQuery, window, Sys */
    (function ($, Sys) {
        function setUpWESNet_ByInvitation_ManageInvitations() {
            $('#WESNet_ByInvitation_ManageInvitations').dnnPanels();
            $('#manageInvitationsContent .dnnFormExpandContent a').dnnExpandAll({
                expandText: '<%=Localization.GetString("ExpandAll", Localization.SharedResourceFile)%>',
                collapseText: '<%=Localization.GetString("CollapseAll", Localization.SharedResourceFile)%>',
                targetArea: '#manageInvitationsContent'
            });
            $('.imgButtonDelete').dnnConfirm({
                text: '<%= Localization.GetString("DeleteItem.Text", Localization.SharedResourceFile) %>',
                yesText: '<%= Localization.GetString("Yes.Text", Localization.SharedResourceFile) %>',
                noText: '<%= Localization.GetString("No.Text", Localization.SharedResourceFile) %>',
                title: '<%= Localization.GetString("Confirm.Text", Localization.SharedResourceFile) %>'
            });
        }

        $(document).ready(function () {
            setUpWESNet_ByInvitation_ManageInvitations();
            Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function () {
                setUpWESNet_ByInvitation_ManageInvitations();
            });
        });
    }(jQuery, window.Sys));
</script>
