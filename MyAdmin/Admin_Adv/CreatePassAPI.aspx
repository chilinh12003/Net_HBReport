<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/Admin.Master" AutoEventWireup="true" CodeBehind="CreatePassAPI.aspx.cs" Inherits="MyAdmin.Admin_Adv.CreatePassAPI" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph_Header" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cph_Tools" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cph_ToolBox" runat="server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="cph_Search" runat="server">
    <input type="text" runat="server" id="tbx_PartnerID" />
    <asp:Button runat="server" ID="btn_Gen" Text="Tạo" OnClick="btn_Gen_Click" />
    <input type="text" runat="server" id="tbx_Result" disabled="disabled" />
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="cph_Content" runat="server">
    <div runat="server" id="div_Result"></div>
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="cph_Javascript" runat="server">
</asp:Content>
