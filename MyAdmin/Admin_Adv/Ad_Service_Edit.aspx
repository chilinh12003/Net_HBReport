<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/Admin.Master" AutoEventWireup="true" CodeBehind="Ad_Service_Edit.aspx.cs" Inherits="MyAdmin.Admin_Adv.Ad_Service_Edit" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cph_Header" runat="server">
    <link href="../CSS/autocomplete.css" rel="stylesheet" type="text/css" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cph_Tools" runat="server">
    <a href="Ad_Service.aspx" runat="server" id="link_Cancel"><span class="Cancel"></span>Hủy </a>
    <asp:LinkButton runat="server" ID="lbtn_Save" OnClick="lbtn_Save_Click" OnClientClick="return CheckAll();">
     <span class="Save"></span>
            Lưu
    </asp:LinkButton>
    <asp:LinkButton runat="server" ID="lbtn_Accept" OnClick="lbtn_Apply_Click" OnClientClick="return CheckAll();">
     <span class="Accept"></span>
            Apply
    </asp:LinkButton>
    <a href="Ad_Service_Edit.aspx" runat="server" id="link_Add"><span class="Add"></span>Thêm </a>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cph_ToolBox" runat="server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="cph_Search" runat="server">
    <div class="Edit_Left">
        <div class="Edit_Title">
            Tên dịch vụ
        </div>
        <div class="Edit_Control">
             <input type="text" runat="server" id="tbx_ServiceName" />
        </div>
          <div class="Edit_Title">
            Dịch vụ cha
        </div>
        <div class="Edit_Control">
            <select runat="server" id="sel_Service">
            </select>
        </div>
          <div class="Edit_Title">
                Kích hoạt:</div>
            <div class="Edit_Control">
                <input type="checkbox" runat="server" id="chk_Active" checked="checked" />
            </div>
        <div class="Edit_Title">
            ConnectionName
        </div>
        <div class="Edit_Control">
             <input type="text" runat="server" id="tbx_ConnectionName" style="width:100%;" />
        </div>
         <div class="Edit_Title">
            Độ ưu tiên
        </div>
        <div class="Edit_Control">
            <input type="text" runat="server" id="tbx_Priority" value="0" onkeypress="return isNumberKey_int(event);" />
            </div>
        <div class="Edit_Title">
            MapServiceID
        </div>
        <div class="Edit_Control">
            <input type="text" runat="server" id="tbx_MapServiceID" value="0" onkeypress="return isNumberKey_int(event);" />
            </div>

    </div>
    <div class="Edit_Right">
        <div class="Properties_Header">
            <div class="Properties_Header_In">
                Thông tin chi tiết khác
            </div>
        </div>
        <div class="Properties">               
          
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="cph_Content" runat="server">
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="cph_Javascript" runat="server">
</asp:Content>
