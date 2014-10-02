using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using MyHBReport.Permission;
using MyHBReport.ShortCode_6X83;
using MyUtility;
namespace MyAdmin.Admin_CCare
{
    public partial class Ad_History_MO_MT : System.Web.UI.Page
    {

        public GetRole mGetRole;

        public int PageIndex = 1;

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
            }
            return true;
        }

        private void BindCombo(int BindType)
        {
            try
            {
                switch (BindType)
                {
                    case 1: //bind dữ liệu năm

                        DataTable mTable = new DataTable();
                        DataColumn col_ID = new DataColumn("ID", typeof(int));
                        DataColumn col_Text = new DataColumn("Text", typeof(string));
                        mTable.Columns.AddRange(new DataColumn[] { col_ID, col_Text });
                        int FromYear = DateTime.Now.Year - 5;
                        int ToYear = DateTime.Now.Year + 5;

                        for (int i = FromYear; i <= ToYear; i++)
                        {
                            DataRow mRow = mTable.NewRow();
                            mRow["ID"] = i;
                            mRow["Text"] = i.ToString();
                            mTable.Rows.Add(mRow);
                        }

                        sel_Year.DataSource = mTable;
                        sel_Year.DataTextField = "Text";
                        sel_Year.DataValueField = "ID";
                        sel_Year.DataBind();
                        break;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        protected void Page_Init(object sender, EventArgs e)
        {
            bool IsRedirect = false;
            try
            {
                //Phân quyền
                if (ViewState["Role"] == null)
                {
                    mGetRole = new GetRole(MySetting.AdminSetting.ListPage.History_MO_MT, Member.MemberGroupID());
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

                ViewState["SortBy"] = string.Empty;
                if (!IsPostBack)
                {
                    BindCombo(1);
                    sel_Year.SelectedIndex = sel_Year.Items.IndexOf(sel_Year.Items.FindByValue(DateTime.Now.Year.ToString()));
                }
            }
            catch (Exception ex)
            {
                MyLogfile.WriteLogError(ex, true, MyNotice.AdminError.LoadDataError, "Chilinh");
            }
        }

        protected void tbn_Check_Click(object sender, EventArgs e)
        {
            try
            {
                string SortBy = ViewState["SortBy"].ToString();
                DateTime ReportDate = new DateTime(int.Parse(sel_Year.Value), int.Parse(sel_Month.Value), 1);

                string MSISDN = tbx_PhoneNumber.Value.Trim();
                MyConfig.Telco mTelco = MyConfig.Telco.Nothing;
                if (!MyCheck.CheckPhoneNumber(ref MSISDN, ref mTelco, "84"))
                {
                    MyMessage.ShowError("Số điện thoại không hợp lệ, xin vui lòng kiểm tra lại");
                    return;
                }
                if (mTelco != MyConfig.Telco.Viettel && mTelco != MyConfig.Telco.Vinaphone && mTelco != MyConfig.Telco.Mobifone && mTelco != MyConfig.Telco.VietNamMobile)
                {
                    MyMessage.ShowError("Số điện thoại không thuộc 4 nhà mạng Viettel,Vinaphone, Mobifone, VietNamMobile. Xin vui lòng kiểm tra lại");
                    return;
                }

                MOMTLog mLog;
                if (mTelco == MyConfig.Telco.Vinaphone)
                {
                    mLog = new MOMTLog("MySQLConnection_GPC");
                }
                else if (mTelco == MyConfig.Telco.Mobifone)
                {
                    mLog = new MOMTLog("MySQLConnection_VMS");
                }
                else if (mTelco == MyConfig.Telco.Viettel)
                {
                    mLog = new MOMTLog("MySQLConnection_Viettel");
                }
                else if (mTelco == MyConfig.Telco.VietNamMobile)
                {
                    mLog = new MOMTLog("MySQLConnection_VietNamMobile");
                }
                else
                {
                    mLog = new MOMTLog();
                }

                rpt_Data.DataSource = mLog.Select(ReportDate, MSISDN, SortBy);
                rpt_Data.DataBind();
            }
            catch (Exception ex)
            {
                MyLogfile.WriteLogError(ex, true, MyNotice.AdminError.LoadDataError, "Chilinh");
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

                tbn_Check_Click(null, null);
            }
            catch (Exception ex)
            {
                MyLogfile.WriteLogError(ex, true, MyNotice.AdminError.SortError, "Chilinh");
            }
        }
    }
}