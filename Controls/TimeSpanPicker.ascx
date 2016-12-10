<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TimeSpanPicker.ascx.cs" Inherits="WESNet.DNN.Modules.ByInvitation.Controls.TimeSpanPicker" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.UI.WebControls" Assembly="DotNetNuke.Web.Deprecated" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.UI.WebControls" Assembly="DotNetNuke" %>
<div class="WESNet_TimeSpanPicker">
    <dnn:DnnRadButton ID="btnSign" runat="server" ButtonType="ToggleButton" ToggleType="CustomToggle" CssClass="SignToggle" AutoPostBack="false" EnableViewState="true" ></dnn:DnnRadButton>
    <span id="spanDays" runat="server">
        <label for="<%=tbDays.ClientID %>"><%=LocalizeString("Days")%></label>
        <dnn:DnnNumericTextBox ID="tbDays" runat="server" Type="Number" NumberFormat-DecimalDigits="0" MinValue="0" MaxValue="366"></dnn:DnnNumericTextBox>
    </span>
    <span id="spanHours" runat="server">
        <label for="<%=tbHours.ClientID %>"><%=LocalizeString("Hours")%></label>
        <dnn:DnnNumericTextBox ID="tbHours" runat="server" Type="Number" NumberFormat-DecimalDigits="0" MinValue="0" MaxValue="23"></dnn:DnnNumericTextBox>
    </span>
    <span id="spanMinutes" runat="server">
        <label for="<%=tbMinutes.ClientID %>"><%=LocalizeString("Minutes")%></label>
        <dnn:DnnNumericTextBox ID="tbMinutes" runat="server" Type="Number" NumberFormat-DecimalDigits="0" MinValue="0" MaxValue="59"></dnn:DnnNumericTextBox>
    </span>
    <span id="spanSeconds" runat="server">
        <label for="<%=tbSeconds.ClientID %>"><%=LocalizeString("Seconds")%></label>
        <dnn:DnnNumericTextBox ID="tbSeconds" runat="server" Type="Number" NumberFormat-DecimalDigits="0" MinValue="0" MaxValue="59"></dnn:DnnNumericTextBox>
    </span>
    <span id="spanMilliseconds" runat="server">
        <label for="<%=tbMilliseconds.ClientID %>"><%=LocalizeString("Milliseconds")%></label>
        <dnn:DnnNumericTextBox ID="tbMilliseconds" runat="server" Type="Number"  NumberFormat-DecimalDigits="0" MinValue="0" MaxValue="999"></dnn:DnnNumericTextBox>
    </span>
</div>