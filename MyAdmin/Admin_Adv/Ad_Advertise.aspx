<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/Admin.Master" AutoEventWireup="true" CodeBehind="Ad_Advertise.aspx.cs" Inherits="MyAdmin.Admin_Adv.Ad_Advertise" %>

<%@ Register Src="../Admin_Control/Admin_Paging.ascx" TagName="Admin_Paging" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph_Header" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cph_Tools" runat="server">
    <a href="javascript:void(0);" onclick="return EditData();" runat="server" id="link_Edit"><span class="Edit"></span>Sửa </a>
    <asp:LinkButton runat="server" ID="lbtn_Delete" OnClientClick="return BeforeDeteleData();" ToolTip="Xóa tất cả mục đã chọn" OnClick="lbtn_Delete_Click">
        <span class="Delete"></span>
            Xóa
    </asp:LinkButton>
    <a href="Ad_Advertise_Edit.aspx" runat="server" id="link_Add"><span class="Add"></span>Thêm </a>  
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cph_ToolBox" runat="server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="cph_Search" runat="server">
    <label>
        Từ khóa:</label>
    <input type="text" runat="server" id="tbx_Search" />

    <select runat="server" id="sel_Service"></select>
    <select runat="server" id="sel_Partner"></select>
    <select runat="server" id="sel_Method"></select>
    <select runat="server" id="sel_Status"></select>

    <asp:Button runat="server" ID="btn_Search" Text="Tìm kiếm" OnClick="btn_Search_Click" />
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="cph_Content" runat="server">
    <table class="Data" border="0" cellpadding="0" cellspacing="0">
        <tbody>
            <tr class="Table_Header">
                <th class="Table_TL"></th>
                <th width="10">STT
                </th>
                <th>
                    <asp:LinkButton runat="server" CssClass="Sort" ID="lbtn_Sort_1" CommandArgument="AdvertiseID ASC" OnClick="lbtn_Sort_Click">Mã</asp:LinkButton>
                </th>
                <th>
                    <asp:LinkButton runat="server" CssClass="Sort" ID="lbtn_Sort_2" CommandArgument="AdvertiseName ASC" OnClick="lbtn_Sort_Click">Tiêu đề</asp:LinkButton>
                </th>
                <th>Dịch vụ
                </th>
                <th>Đối tác
                </th>
                <th style="width:10%;">ConfirmLink
                </th>
                <th style="width:10%;">NotConfirmLink
                </th>
                <th style="width:10%;">Redirect
                </th>
                <th>
                    <asp:LinkButton runat="server" CssClass="Sort" ID="lbtn_Sort_3" CommandArgument="BeginDate ASC" OnClick="lbtn_Sort_Click">Bắt đầu</asp:LinkButton>
                </th>
                <th>
                    <asp:LinkButton runat="server" CssClass="Sort" ID="lbtn_Sort_4" CommandArgument="EndDate ASC" OnClick="lbtn_Sort_Click">Kết thúc</asp:LinkButton>
                </th>
                <th>Giới hạn DK
                </th>
                <th>
                    <asp:LinkButton runat="server" CssClass="Sort" ID="lbtn_Sort_5" CommandArgument="MethodName ASC" OnClick="lbtn_Sort_Click">Cách thức QC</asp:LinkButton>
                </th>
                <th>
                    <asp:LinkButton runat="server" CssClass="Sort" ID="lbtn_Sort_6" CommandArgument="StatusName ASC" OnClick="lbtn_Sort_Click">Tình trạng</asp:LinkButton>
                </th>
                <th align="center" width="10">
                    <input type="checkbox" onclick="SelectCheckBox_All(this);" />
                </th>
                <th class="Table_TR"></th>
            </tr>
            <asp:Repeater runat="server" ID="rpt_Data">
                <ItemTemplate>
                    <tr class="Table_Row_1">
                        <td class="Table_ML_1"></td>
                        <td>
                            <%#(Container.ItemIndex + PageIndex).ToString()%>
                        </td>
                        <td>
                            <%#Eval("AdvertiseID") %>
                        </td>
                        <td>
                            <a href="Ad_Advertise_Edit.aspx?ID=<%#Eval("AdvertiseID") %>"><%#Eval("AdvertiseName")%></a>
                        </td>
                        <td>
                            <%#Eval("ServiceName")%>
                        </td>
                        <td>
                            <%#Eval("PartnerName")%>
                        </td>
                        <td>
                            <%#Eval("ConfirmLink")%>
                        </td>
                        <td>
                            <%#Eval("NotConfirmLink")%>
                        </td>
                        <td>
                            <%#Eval("RedirectLink")%>
                        </td>
                        <td>
                            <%#Eval("BeginDate") == DBNull.Value ? string.Empty : ((DateTime)Eval("BeginDate")).ToString(MyUtility.MyConfig.LongDateFormat)%>
                        </td>
                        <td>
                            <%#Eval("EndDate") == DBNull.Value ? string.Empty : ((DateTime)Eval("EndDate")).ToString(MyUtility.MyConfig.LongDateFormat)%>
                        </td>
                        <td>
                            <%#Eval("MaxReg")%>
                        </td>
                        <td>
                            <%#Eval("MethodName")%>
                        </td>
                        <td>
                            <%#Eval("StatusName")%>
                        </td>
                        <td align="center" width="10">
                            <%#"<input type='checkbox' id='CheckAll_" + Container.ItemIndex.ToString() + "' value='" + Eval("AdvertiseID").ToString() + "' />"%>
                        </td>
                        <td class="Table_MR_1"></td>
                    </tr>
                </ItemTemplate>
                <AlternatingItemTemplate>
                    <tr class="Table_Row_2">
                        <td class="Table_ML_2"></td>
                        <td>
                            <%#(Container.ItemIndex + PageIndex).ToString()%>
                        </td>
                        <td>
                            <%#Eval("AdvertiseID") %>
                        </td>
                        <td>
                            <a href="Ad_Advertise_Edit.aspx?ID=<%#Eval("AdvertiseID") %>"><%#Eval("AdvertiseName")%></a>
                        </td>
                        <td>
                            <%#Eval("ServiceName")%>
                        </td>
                        <td>
                            <%#Eval("PartnerName")%>
                        </td>
                        <td>
                            <%#Eval("ConfirmLink")%>
                        </td>
                        <td>
                            <%#Eval("NotConfirmLink")%>
                        </td>
                        <td>
                            <%#Eval("RedirectLink")%>
                        </td>
                        <td>
                            <%#Eval("BeginDate") == DBNull.Value ? string.Empty : ((DateTime)Eval("BeginDate")).ToString(MyUtility.MyConfig.LongDateFormat)%>
                        </td>
                        <td>
                            <%#Eval("EndDate") == DBNull.Value ? string.Empty : ((DateTime)Eval("EndDate")).ToString(MyUtility.MyConfig.LongDateFormat)%>
                        </td>
                        <td>
                            <%#Eval("MaxReg")%>
                        </td>
                        <td>
                            <%#Eval("MethodName")%>
                        </td>
                        <td>
                            <%#Eval("StatusName")%>
                        </td>
                        <td align="center" width="10">
                            <%#"<input type='checkbox' id='CheckAll_" + Container.ItemIndex.ToString() + "' value='" + Eval("AdvertiseID").ToString() + "' />"%>
                        </td>
                        <td class="Table_MR_2"></td>
                    </tr>
                </AlternatingItemTemplate>
            </asp:Repeater>
        </tbody>
    </table>
    <div class="Table_Footer">
        <div class="Table_BL">
            <uc1:Admin_Paging ID="Admin_Paging1" runat="server" />
        </div>
        <div class="Table_BR">
        </div>
    </div>
    <div class="Div_Hidden">
        <input type="hidden" runat="server" id="hid_ListCheckAll" />
    </div>
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="cph_Javascript" runat="server">
    <script language="javascript" type="text/javascript">
        hid_ListCheckAll = document.getElementById("<%=hid_ListCheckAll.ClientID %>");

        ReCheck_CheckboxOnGrid();

        function EditData()
        {
            if (BeforeEditData())
            {
                document.location = 'Ad_Advertise_Edit.aspx?ID=' + hid_ListCheckAll.value;

                return true;
            }
            return false;
        }

        function Active()
        {
            if (GetAllCheck('Xin hãy chọn ít nhất một mục để kích hoạt'))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        function UnActive()
        {
            if (GetAllCheck('Xin hãy chọn ít nhất một mục để hủy kích hoạt'))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    </script>
</asp:Content>
