<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/Admin.Master" AutoEventWireup="true" CodeBehind="Ad_ChangePassword.aspx.cs" Inherits="MyAdmin.Admin.Ad_ChangePassword" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cph_Header" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cph_Tools" runat="server">
    <asp:LinkButton runat="server" ID="lbtn_Accept" OnClick="lbtn_Apply_Click">
     <span class="Accept"></span>
            Apply
    </asp:LinkButton>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cph_ToolBox" runat="server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="cph_Search" runat="server">
    <div class="Edit_Left">
        <div class="Edit_Title">
            Tên đăng nhập:
        </div>
        <div class="Edit_Control">
            <input type="text" runat="server" id="tbx_LoginName" />
        </div>
        <div class="Edit_Title">
            Mật khẩu cũ:
        </div>
        <div class="Edit_Control">
            <input type="password" runat="server" id="tbx_OldPass" />
        </div>
        <div class="Edit_Title">
            Mật khẩu mới:</div>
        <div class="Edit_Control">
            <input type="password" runat="server" id="tbx_NewPass" />
        </div>
        <div class="Edit_Title">
            Nhắc lại mật khẩu:</div>
        <div class="Edit_Control">
            <input type="password" runat="server" id="tbx_RepeatPass" />
        </div>
        <div class="Edit_Title"  style="height:40px;">
            &nbsp;</div>
        <div class="Edit_Control" style="height:50px;">
            <img id="img_captcha" src="../Captcha.ashx" style="vertical-align: middle; width: 150px; height: 50px; margin-left: 2px;" />
            <a style="vertical-align: middle;" title="Đỗi mã khác" href="javascript:void(0);" onclick="refresh_captcha('img_captcha');">
                <img src="../Images/Buttons/Refresh.png" /></a>
        </div>
         <div class="Edit_Title">
            Nhập mã xác thực:</div>
        <div class="Edit_Control">
            <input type="text" runat="server" id="tbx_Captcha" />
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="cph_Content" runat="server">
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="cph_Javascript" runat="server">
    <script language="javascript" type="text/javascript">
        function refresh_captcha(ctr_id) {
            var src = "../captcha.ashx" + "?time=" + (new Date()).getTime();
            $("#" + ctr_id).attr("src", src);
        }
    </script>
</asp:Content>
