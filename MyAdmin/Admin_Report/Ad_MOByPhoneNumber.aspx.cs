﻿using System;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using MyUtility;
using MyHBReport.ShortCode_6X83;
using MyHBReport.Permission;

namespace MyAdmin.Admin_Report
{
    public partial class Ad_MOByPhoneNumber : System.Web.UI.Page
    {
        public GetRole mGetRole;
        public int PageIndex = 1;
        MOMTLog mMOMTLog = new MOMTLog("MySQLConnection_VMS");

        private void BindCombo(int type)
        {
            try
            {
                switch (type)
                {
                    case 1:

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

            }
            catch (Exception ex)
            {
                MyLogfile.WriteLogError(ex, true, MyNotice.AdminError.CheckPermissionError, "Chilinh");
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
                    mGetRole = new GetRole(MySetting.AdminSetting.ListPage.Report_MOByPhoneNumber, Member.MemberGroupID());
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
                MyLogfile.WriteLogError(ex, true, MyNotice.AdminError.LoadDataError, "Chilinh");
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

                    tbx_FromDate.Value = MyConfig.StartDayOfMonth.ToString(MyConfig.ShortDateFormat);
                    tbx_ToDate.Value = DateTime.Now.ToString(MyConfig.ShortDateFormat);
                }

                Admin_Paging1.rpt_Data = rpt_Data;
                Admin_Paging1.GetData_Callback_Change += new MyAdmin.Admin_Control.Admin_Paging.GetData_Callback(Admin_Paging1_GetData_Callback_Change);
                Admin_Paging1.GetTotalPage_Callback_Change += new MyAdmin.Admin_Control.Admin_Paging.GetTotalPage_Callback(Admin_Paging1_GetTotalPage_Callback_Change);
            }
            catch (Exception ex)
            {
                MyLogfile.WriteLogError(ex, true, MyNotice.AdminError.LoadDataError, "Chilinh");
            }
        }

        int Admin_Paging1_GetTotalPage_Callback_Change()
        {
            try
            {
                if (!IsPostBack)
                    return 0;

                //string MSISDN = tbx_PhoneNumber.Value.Trim();
                //MyConfig.Telco mTelco = MyConfig.Telco.Nothing;
                //if (!MyCheck.CheckPhoneNumber(ref MSISDN, ref mTelco, "84"))
                //{
                //    MyMessage.ShowError("Số điện thoại không hợp lệ, xin vui lòng kiểm tra lại");
                //    return 0;
                //}
                //if ( mTelco != MyConfig.Telco.Mobifone)
                //{
                //    MyMessage.ShowError("Số điện thoại không thuộc nhà mạng Mobifone. Xin vui lòng kiểm tra lại");
                //    return 0;
                //}

                DateTime BeginDate = tbx_FromDate.Value.Length > 0 ? DateTime.ParseExact(tbx_FromDate.Value, "dd/MM/yyyy", null) : DateTime.MinValue;
                DateTime EndDate = tbx_ToDate.Value.Length > 0 ? DateTime.ParseExact(tbx_ToDate.Value, "dd/MM/yyyy", null) : DateTime.MinValue;

                if (BeginDate.Month != EndDate.Month)
                {
                    MyMessage.ShowError("Thời gian bắt đầu và thời gian kết thúc phải trong cùng 1 tháng, xin hãy chọn lại.");
                    return 0;
                }
                EndDate = EndDate.AddDays(1);

                return mMOMTLog.MOByPhoneNumber_RowCount(BeginDate, EndDate);
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
                if (!IsPostBack)
                    return new DataTable();

                //string MSISDN = tbx_PhoneNumber.Value.Trim();
                //MyConfig.Telco mTelco = MyConfig.Telco.Nothing;
                //if (!MyCheck.CheckPhoneNumber(ref MSISDN, ref mTelco, "84"))
                //{
                //    MyMessage.ShowError("Số điện thoại không hợp lệ, xin vui lòng kiểm tra lại");
                //    return new DataTable();
                //}
                //if (mTelco != MyConfig.Telco.Mobifone)
                //{
                //    MyMessage.ShowError("Số điện thoại không thuộc nhà mạng Mobifone. Xin vui lòng kiểm tra lại");
                //    return new DataTable();
                //}

                DateTime BeginDate = tbx_FromDate.Value.Length > 0 ? DateTime.ParseExact(tbx_FromDate.Value, "dd/MM/yyyy", null) : DateTime.MinValue;
                DateTime EndDate = tbx_ToDate.Value.Length > 0 ? DateTime.ParseExact(tbx_ToDate.Value, "dd/MM/yyyy", null) : DateTime.MinValue;

                if (BeginDate.Month != EndDate.Month)
                {
                    MyMessage.ShowError("Thời gian bắt đầu và thời gian kết thúc phải trong cùng 1 tháng, xin hãy chọn lại.");
                    return new DataTable();
                }

                EndDate = EndDate.AddDays(1);

                PageIndex = (Admin_Paging1.mPaging.CurrentPageIndex - 1) * Admin_Paging1.mPaging.PageSize + 1;

                return mMOMTLog.MOByPhoneNumber_Data(Admin_Paging1.mPaging.BeginRow, Admin_Paging1.mPaging.PageSize, BeginDate, EndDate, string.Empty);
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
                //lbtn_Sort_1.CssClass = "Sort";
                //lbtn_Sort_2.CssClass = "Sort";
                //lbtn_Sort_3.CssClass = "Sort";
                //lbtn_Sort_4.CssClass = "Sort";
                //lbtn_Sort_5.CssClass = "Sort";
                //lbtn_Sort_6.CssClass = "Sort";
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
                MyLogfile.WriteLogError(ex, true, MyNotice.AdminError.SortError, "Chilinh");
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
                MyLogfile.WriteLogError(ex, true, MyNotice.AdminError.SeachError, "Chilinh");
            }
        }

    }
}