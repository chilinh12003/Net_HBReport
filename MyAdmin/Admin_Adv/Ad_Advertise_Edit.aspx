<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/Admin.Master" AutoEventWireup="true" CodeBehind="Ad_Advertise_Edit.aspx.cs" Inherits="MyAdmin.Admin_Adv.Ad_Advertise_Edit" %>


<asp:Content ID="Content1" ContentPlaceHolderID="cph_Header" runat="server">

    <link href="../Calendar/dhtmlgoodies_calendar/dhtmlgoodies_calendar.css" rel="stylesheet"
        type="text/css" />

    <script src="../Calendar/dhtmlgoodies_calendar/dhtmlgoodies_calendar.js" type="text/javascript"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cph_Tools" runat="server">
    <a href="Ad_Advertise.aspx" runat="server" id="link_Cancel"><span class="Cancel"></span>Hủy </a>
    <asp:LinkButton runat="server" ID="lbtn_Save" OnClick="lbtn_Save_Click" OnClientClick="return CheckAll();">
     <span class="Save"></span>
            Lưu
    </asp:LinkButton>
    <asp:LinkButton runat="server" ID="lbtn_Accept" OnClick="lbtn_Apply_Click" OnClientClick="return CheckAll();">
     <span class="Accept"></span>
            Apply
    </asp:LinkButton>
    <a href="Ad_Advertise_Edit.aspx" runat="server" id="link_Add"><span class="Add"></span>Thêm </a>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cph_ToolBox" runat="server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="cph_Search" runat="server">
    <div class="Edit_Left">
        <div class="Edit_Title">
            Tên quảng cáo
        </div>
        <div class="Edit_Control">
            <input type="text" runat="server" id="tbx_AdvertiseName" />
        </div>
        <div class="Edit_Title">
            Dịch vụ
        </div>
        <div class="Edit_Control">
            <select runat="server" id="sel_Service">
            </select>
        </div>
        <div class="Edit_Title">Đối tác:</div>
        <div class="Edit_Control">
            <select runat="server" id="sel_Partner">
            </select>
        </div>
        <div class="Edit_Title">
            Hình thức quảng cáo
        </div>
        <div class="Edit_Control">
            <select runat="server" id="sel_Method"></select>
        </div>
        <div class="Edit_Title">
            Link confirm
        </div>
        <div class="Edit_Control">
            <input type="text" runat="server" id="tbx_ConfirmLink" style="width: 100%;" />
        </div>
        <div class="Edit_Title">
            Link không confirm
        </div>
        <div class="Edit_Control">
            <input type="text" runat="server" id="tbx_NotConfirmLink" style="width: 100%;" />
        </div>
        <div class="Edit_Title">
            Link Redirect
        </div>
        <div class="Edit_Control">
            <input type="text" runat="server" id="tbx_RedirectLink" style="width: 100%;" />
        </div>
         <div class="Edit_Title">
            Link Pass
        </div>
        <div class="Edit_Control">
            <input type="text" runat="server" id="tbx_PassLink" style="width: 100%;" />
        </div>
        <div class="Edit_Title">
            Used Link
        </div>
        <div class="Edit_Control">
            <input type="text" runat="server" id="tbx_UsedLink" style="width: 100%;" />
        </div>
        <div class="Edit_Title">
            Log MSISDN Link
        </div>
        <div class="Edit_Control">
            <input type="text" runat="server" id="tbx_LogMSISDNLink" style="width: 100%;" />
        </div>
        <div class="Edit_Title">
            Ngày bắt đầu
        </div>
        <div class="Edit_Control">
            <input type="text" runat="server" id="tbx_BeginDate" style="width: 70px;" />
            <input type="button" value="..." onclick="displayCalendar(document.getElementById('<%=tbx_BeginDate.ClientID %>'),'dd/mm/yyyy',this)" />
            <div>
                <label>
                    Giờ:</label>
            </div>
            <select runat="server" id="sel_BeginHour">
            </select>
            <div>
                <label>
                    Phút:</label>
            </div>
            <select runat="server" id="sel_BeginMinute">
            </select>
        </div>
        <div class="Edit_Title">
            Ngày kết thúc
        </div>
        <div class="Edit_Control">
            <input type="text" runat="server" id="tbx_EndDate" style="width: 70px;" />
            <input type="button" value="..." onclick="displayCalendar(document.getElementById('<%=tbx_EndDate.ClientID %>'),'dd/mm/yyyy',this)" />
            <div>
                <label>
                    Giờ:</label>
            </div>
            <select runat="server" id="sel_EndHour">
            </select>
            <div>
                <label>
                    Phút:</label>
            </div>
            <select runat="server" id="sel_EndMinute">
            </select>
        </div>
    </div>
    <div class="Edit_Right">
        <div class="Properties_Header">
            <div class="Properties_Header_In">
                Thông tin chi tiết khác
            </div>
        </div>
        <div class="Properties">

            <div class="Properties_Title">
                Tình trang
            </div>
            <div class="Properties_Control">
                <select runat="server" id="sel_Status"></select>
            </div>
            <div class="Properties_Title">
                Giới hạn DK
            </div>
            <div class="Properties_Control">
                <input type="text" runat="server" id="tbx_MaxReg" value="100" onkeypress="return isNumberKey_int(event);" />
            </div>
             <div class="Properties_Title">
                MapPartnerID
            </div>
            <div class="Properties_Control">
                <input type="text" runat="server" id="tbx_MapPartnerID" value="0" onkeypress="return isNumberKey_int(event);" />
            </div>
            <div class="Properties_Title">
                Redirect Delay
            </div>
            <div class="Properties_Control">
                <input type="text" runat="server" id="tbx_RedirectDelay" value="0" onkeypress="return isNumberKey_int(event);" />
            </div>
            <div class="Properties_Title">
                MaxRequest
            </div>
            <div class="Properties_Control">
                <input type="text" runat="server" id="tbx_MaxRequest" value="0" onkeypress="return isNumberKey_int(event);" />
            </div>
            <div class="Properties_Title">
                PassPercent
            </div>
            <div class="Properties_Control">
                <input type="text" runat="server" id="tbx_PassPercent" value="0" onkeypress="return isNumberKey_int(event);" />
            </div>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="cph_Content" runat="server">
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="cph_Javascript" runat="server">
</asp:Content>
