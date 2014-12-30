using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using MyUtility;
using MyHBReport.Adv;
using MyHBReport.Permission;

namespace MyAdmin.Admin_Adv
{
    public partial class Ad_Advertise : System.Web.UI.Page
    {
        public GetRole mGetRole;
        MyLog mLog = new MyLog(typeof(Ad_Advertise));
        public int PageIndex = 1;

        Advertise mData = new Advertise();

        private void BindCombo(int type)
        {
            try
            {
                switch (type)
                {
                    case 1://Dịch vụ
                        Service mService = new Service();
                        DataTable mTable_Service = mService.Select(2);
                        foreach (DataRow mRow in mTable_Service.Rows)
                        {
                            if(mRow["ParentID"] != DBNull.Value)
                            {
                                mRow["ServiceName"] = "-->" + mRow["ServiceName"].ToString();
                            }
                        }
                        sel_Service.DataSource = mTable_Service;
                        sel_Service.DataValueField = "ServiceID";
                        sel_Service.DataTextField = "ServiceName";
                        sel_Service.DataBind();

                        sel_Service.Items.Insert(0, new ListItem("--Dịch vụ--", "0"));
                        break;
                    case 2://Đối tác
                        Partner mPartner = new Partner();
                        DataTable mTable_Partner = mPartner.Select(2, string.Empty);
                        foreach(DataRow mRow in mTable_Partner.Rows)
                        {
                            if(mRow["ParentID"] != DBNull.Value)
                            {
                                mRow["PartnerName"] = "-->" + mRow["PartnerName"].ToString();
                            }
                        }
                        sel_Partner.DataSource = mTable_Partner;
                        sel_Partner.DataValueField = "PartnerID";
                        sel_Partner.DataTextField = "PartnerName";
                        sel_Partner.DataBind();

                        sel_Partner.Items.Insert(0, new ListItem("--Đối tác--", "0"));
                        break;
                    case 3://Phương thức quảng cáo
                        sel_Method.DataSource = MyEnum.CrateDatasourceFromEnum(typeof(Advertise.Method),true);
                        sel_Method.DataTextField = "Text";
                        sel_Method.DataValueField = "ID";
                        sel_Method.DataBind();
                        sel_Method.Items.Insert(0, new ListItem("--Phương thức quảng cáo--", "0"));
                        break;
                    case 4://Tình trạng
                        sel_Status.DataSource = MyEnum.CrateDatasourceFromEnum(typeof(Advertise.Status), true);
                        sel_Status.DataTextField = "Text";
                        sel_Status.DataValueField = "ID";
                        sel_Status.DataBind();
                        sel_Status.Items.Insert(0, new ListItem("--Tình trạng--", "0"));
                        break;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void BindData()
        {
            Admin_Paging1.ResetLoadData();
        }

        private bool CheckPermission()
        {
            try
            {
                if (mGetRole.ViewRole == false)
                {
                    Response.Redirect(mGetRole.URLNotView, false);
                    return false;
                }

                link_Add.Visible = mGetRole.AddRole;
                link_Edit.Visible = mGetRole.EditRole;
                //lbtn_Active.Visible = mGetRole.PublishRole;
                //lbtn_UnActive.Visible = mGetRole.PublishRole;
                lbtn_Delete.Visible = mGetRole.DeleteRole;

            }
            catch (Exception ex)
            {
                mLog.Error(MyNotice.AdminError.CheckPermissionError, true, ex);
                return false;
            }
            return true;
        }

        protected void Page_Init(object sender, EventArgs e)
        {
            bool IsRedirect = false;
            try
            {
                //Phân quyền
                if (ViewState["Role"] == null)
                {
                    mGetRole = new GetRole(MySetting.AdminSetting.ListPage.Adv_Advertise, Member.MemberGroupID());
                }
                else
                {
                    mGetRole = (GetRole)ViewState["Role"];
                }

                if (!CheckPermission())
                {
                    IsRedirect = true;
                }
            }
            catch (Exception ex)
            {
                mLog.Error(MyNotice.AdminError.LoadDataError, true, ex);
            }
            if (IsRedirect)
            {
                Response.End();
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {

                MyAdmin.MasterPages.Admin mMaster = (MyAdmin.MasterPages.Admin)Page.Master;
                mMaster.str_PageTitle = mGetRole.PageName;

                if (!IsPostBack)
                {
                    ViewState["SortBy"] = string.Empty;
                    BindCombo(1);
                    BindCombo(2);
                    BindCombo(3);
                    BindCombo(4);
                }

                Admin_Paging1.rpt_Data = rpt_Data;
                Admin_Paging1.GetData_Callback_Change += new MyAdmin.Admin_Control.Admin_Paging.GetData_Callback(Admin_Paging1_GetData_Callback_Change);
                Admin_Paging1.GetTotalPage_Callback_Change += new MyAdmin.Admin_Control.Admin_Paging.GetTotalPage_Callback(Admin_Paging1_GetTotalPage_Callback_Change);
            }
            catch (Exception ex)
            {
                mLog.Error(MyNotice.AdminError.LoadDataError, true, ex);
            }
        }

        int Admin_Paging1_GetTotalPage_Callback_Change()
        {
            try
            {
                int SearchType = 0;
                string str_SearchContent = null;
                string SortBy = ViewState["SortBy"].ToString();
                str_SearchContent = tbx_Search.Value.Length < 1 ? null : MyText.ValidSearchContent(tbx_Search.Value);

                int ServiceID = 0;
                int PartnerID = 0;
                int MethodID = 0;
                int StatusID = 0;

                int.TryParse(sel_Service.Value, out ServiceID);
                int.TryParse(sel_Partner.Value, out PartnerID);
                int.TryParse(sel_Method.Value, out MethodID);
                int.TryParse(sel_Status.Value, out StatusID);

                return mData.TotalRow(SearchType, str_SearchContent, ServiceID, PartnerID, MethodID, StatusID);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        DataTable Admin_Paging1_GetData_Callback_Change()
        {
            try
            {
                int SearchType = 0;
                string str_SearchContent = null;
                string SortBy = ViewState["SortBy"].ToString();
                str_SearchContent = tbx_Search.Value.Length < 1 ? null : MyText.ValidSearchContent(tbx_Search.Value);

                int ServiceID = 0;
                int PartnerID = 0;
                int MethodID = 0;
                int StatusID = 0;

                int.TryParse(sel_Service.Value, out ServiceID);
                int.TryParse(sel_Partner.Value, out PartnerID);
                int.TryParse(sel_Method.Value, out MethodID);
                int.TryParse(sel_Status.Value, out StatusID);

                PageIndex = (Admin_Paging1.mPaging.CurrentPageIndex - 1) * Admin_Paging1.mPaging.PageSize + 1;

                return mData.Search(SearchType, Admin_Paging1.mPaging.BeginRow, Admin_Paging1.mPaging.EndRow, str_SearchContent, ServiceID, PartnerID, MethodID, StatusID, SortBy);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        protected void lbtn_Sort_Click(object sender, EventArgs e)
        {
            try
            {
                lbtn_Sort_1.CssClass = "Sort";
                lbtn_Sort_2.CssClass = "Sort";
                lbtn_Sort_3.CssClass = "Sort";
                lbtn_Sort_4.CssClass = "Sort";
                lbtn_Sort_5.CssClass = "Sort";
                lbtn_Sort_6.CssClass = "Sort";
                //lbtn_Sort_7.CssClass = "Sort";

                LinkButton mLinkButton = (LinkButton)sender;
                ViewState["SortBy"] = mLinkButton.CommandArgument;

                if (mLinkButton.CommandArgument.IndexOf(" ASC") >= 0)
                {
                    mLinkButton.CssClass = "SortActive_Up";
                    mLinkButton.CommandArgument = mLinkButton.CommandArgument.Replace(" ASC", " DESC");
                }
                else
                {
                    mLinkButton.CssClass = "SortActive_Down";
                    mLinkButton.CommandArgument = mLinkButton.CommandArgument.Replace(" DESC", " ASC");
                }

                BindData();
            }
            catch (Exception ex)
            {
               mLog.Error(MyNotice.AdminError.SortError, true, ex);
            }
        }

        protected void lbtn_Delete_Click(object sender, EventArgs e)
        {
            try
            {
                char[] key_1 = { '|' };

                string[] arr_1 = hid_ListCheckAll.Value.Split(key_1);


                DataSet dds_Parent = new DataSet("Parent");
                DataTable tbl_Child = new DataTable("Child");
                DataColumn col_1 = new DataColumn("ID", typeof(int));
                tbl_Child.Columns.Add(col_1);

                for (int i = 0; i < arr_1.Length; i++)
                {
                    DataRow mRow = tbl_Child.NewRow();

                    mRow["ID"] = int.Parse(arr_1[i]);

                    tbl_Child.Rows.Add(mRow);
                }
                tbl_Child.AcceptChanges();

                dds_Parent.Tables.Add(tbl_Child);
                dds_Parent.AcceptChanges();

                if (mData.Delete(0, dds_Parent.GetXml()))
                {
                    #region Log member
                    MemberLog mLog = new MemberLog();
                    MemberLog.ActionType Action = MemberLog.ActionType.Delete;
                    mLog.Insert("Advertise", string.Empty, dds_Parent.GetXml(), Action, true, string.Empty);
                    #endregion

                    MyMessage.ShowMessage("Xóa dữ liệu thành công.");
                    BindData();
                }
                else
                {
                    MyMessage.ShowMessage("Xóa dữ liệu KHÔNG thành công!");
                }
            }
            catch (Exception ex)
            {
                mLog.Error(MyNotice.AdminError.DeleteDataError, true, ex);
            }
        }

        protected void btn_Search_Click(object sender, EventArgs e)
        {
            try
            {
                BindData();
            }
            catch (Exception ex)
            {
                mLog.Error(MyNotice.AdminError.SeachError, true, ex);
            }
        }       
    }
}