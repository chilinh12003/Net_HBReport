<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/Admin.Master" AutoEventWireup="true" CodeBehind="Ad_History_MO_MT.aspx.cs" Inherits="MyAdmin.Admin_CCare.Ad_History_MO_MT" %>


<asp:Content ID="Content1" ContentPlaceHolderID="cph_Header" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cph_Tools" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cph_ToolBox" runat="server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="cph_Search" runat="server">
<label>
        Số điện thoại:</label><input type="text" runat="server" id="tbx_PhoneNumber" />
    <label>
        Tháng cần xem</label>
    <select runat="server" id="sel_Month">
        <option value="1">1</option>
        <option value="2">2</option>
        <option value="3">3</option>
        <option value="4">4</option>
        <option value="5">5</option>
        <option value="6">6</option>
        <option value="7">7</option>
        <option value="8">8</option>
        <option value="9">9</option>
        <option value="10">10</option>
        <option value="11">11</option>
        <option value="12">12</option>
    </select>
    <label>
        Năm</label>
     <select runat="server" id="sel_Year">
       
    </select>
    <asp:Button runat="server" ID="tbn_Check" Text="Kiểm tra" OnClick="tbn_Check_Click" />
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="cph_Content" runat="server">
    
<table class="Data" border="0" cellpadding="0" cellspacing="0">
        <tbody>
            <tr class="Table_Header">
                <th class="Table_TL"></th>
                <th width="10">STT </th>
                <th><asp:LinkButton runat="server" CssClass="Sort" ID="lbtn_Sort_1" CommandArgument="ShortCode ASC" OnClick="lbtn_Sort_Click">Đầu số</asp:LinkButton></th>
                <th><asp:LinkButton runat="server" CssClass="Sort" ID="lbtn_Sort_2" CommandArgument="Keyword ASC" OnClick="lbtn_Sort_Click">Keyword</asp:LinkButton></th>
                <th><asp:LinkButton runat="server" CssClass="Sort" ID="lbtn_Sort_3" CommandArgument="MO ASC" OnClick="lbtn_Sort_Click">MO</asp:LinkButton></th>
                <th><asp:LinkButton runat="server" CssClass="Sort" ID="lbtn_Sort_4" CommandArgument="ReceiveDate DESC" OnClick="lbtn_Sort_Click">Ngày nhận MO</asp:LinkButton></th>
                <th style="width:30%;"><asp:LinkButton runat="server" CssClass="Sort" ID="lbtn_Sort_5" CommandArgument="MT ASC" OnClick="lbtn_Sort_Click">MT</asp:LinkButton></th>
                <th><asp:LinkButton runat="server" CssClass="Sort" ID="lbtn_Sort_6" CommandArgument="SendDate DESC" OnClick="lbtn_Sort_Click">Ngày trả MT</asp:LinkButton></th>
                <th class="Table_TR"></th>
            </tr>
            <asp:Repeater runat="server" ID="rpt_Data">
                <ItemTemplate>
                    <tr class="Table_Row_1">
                        <td class="Table_ML_1">
                        </td>
                        <td>
                            <%#(Container.ItemIndex + PageIndex).ToString()%>
                        </td>
                        <td>
                            <%#Eval("ShortCode")%>
                        </td>
                        <td>
                            <%#Eval("Keyword")%>
                        </td>
                        <td>
                            <%#Eval("MO")%>
                        </td>
                        <td>
                            <%# ((DateTime)Eval("ReceiveDate")).ToString(MyUtility.MyConfig.LongDateFormat)%>
                        </td>
                         <td>
                            <%#Eval("MT")%>
                        </td><td>
                            <%# ((DateTime)Eval("SendDate")).ToString(MyUtility.MyConfig.LongDateFormat)%>
                        </td>                        
                        <td class="Table_MR_1">
                        </td>
                    </tr>
                </ItemTemplate>
                <AlternatingItemTemplate>
                    <tr class="Table_Row_2">
                        <td class="Table_ML_2">
                        </td>
                         <td>
                            <%#(Container.ItemIndex + PageIndex).ToString()%>
                        </td>
                        <td>
                            <%#Eval("ShortCode")%>
                        </td>
                        <td>
                            <%#Eval("Keyword")%>
                        </td>
                        <td>
                            <%#Eval("MO")%>
                        </td>
                        <td>
                            <%# ((DateTime)Eval("ReceiveDate")).ToString(MyUtility.MyConfig.LongDateFormat)%>
                        </td>
                         <td>
                            <%#Eval("MT")%>
                        </td><td>
                            <%# ((DateTime)Eval("SendDate")).ToString(MyUtility.MyConfig.LongDateFormat)%>
                        </td>                        
                        <td class="Table_MR_2">
                        </td>
                    </tr>
                </AlternatingItemTemplate>
            </asp:Repeater>
        </tbody>
    </table>
    <div class="Table_Footer">
        <div class="Table_BL">
        </div>
        <div class="Table_BR">
        </div>
    </div>
    <div class="Div_Hidden">
        <input type="hidden" runat="server" id="hid_ListCheckAll" />
    </div>
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="cph_Javascript" runat="server">

</asp:Content>

