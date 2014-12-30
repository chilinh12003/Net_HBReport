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
    public partial class Ad_Service_Edit : System.Web.UI.Page
    {
        public GetRole mGetRole;
        MyLog mLog = new MyLog(typeof(Ad_Service_Edit));
        Service mData = new Service();

        int EditID = 0;

        public string ParentPath = "../Admin_Adv/Ad_Service.aspx";

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
                            if (mRow["ParentID"] != DBNull.Value)
                            {
                                mRow["ServiceName"] = "-->" + mRow["ServiceName"].ToString();
                            }
                        }
                        if(EditID > 0)
                        {
                            mTable_Service.DefaultView.RowFilter = "ServiceID <> " + EditID;
                        }
                        sel_Service.DataSource = mTable_Service;
                        sel_Service.DataValueField = "ServiceID";
                        sel_Service.DataTextField = "ServiceName";
                        sel_Service.DataBind();
                        sel_Service.Items.Insert(0, new ListItem("--Không chọn--", "0"));
                        break;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
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
                if (EditID > 0)
                {
                    lbtn_Save.Visible = lbtn_Accept.Visible = mGetRole.EditRole;
                    link_Add.Visible = mGetRole.AddRole;
                }
                else
                {
                    lbtn_Save.Visible = lbtn_Accept.Visible = link_Add.Visible = mGetRole.AddRole;
                }
                chk_Active.Disabled = !mGetRole.PublishRole;

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
                    mGetRole = new GetRole(MySetting.AdminSetting.ListPage.Adv_Service, Member.MemberGroupID());
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
                //Lấy memberID nếu là trước hợp Sửa
                EditID = Request.QueryString["ID"] == null ? 0 : int.Parse(Request.QueryString["ID"]);

                MyAdmin.MasterPages.Admin mMaster = (MyAdmin.MasterPages.Admin)Page.Master;
                mMaster.str_PageTitle = mGetRole.PageName;
                mMaster.str_TitleSearchBox = "Thông tin về " + mGetRole.PageName;

                if (!IsPostBack)
                {
                    BindCombo(1);

                    //Nếu là Edit
                    if (EditID > 0)
                    {
                        DataTable mTable = mData.Select(1, EditID.ToString());

                        //Lưu lại thông tin OldData để lưu vào MemberLog
                        ViewState["OldData"] = MyXML.GetXML(mTable);

                        if (mTable != null && mTable.Rows.Count > 0)
                        {
                            #region MyRegion
                            DataRow mRow = mTable.Rows[0];
                            tbx_ServiceName.Value = mRow["ServiceName"].ToString();

                            if (mRow["ParentID"] != DBNull.Value)
                                sel_Service.SelectedIndex = sel_Service.Items.IndexOf(sel_Service.Items.FindByValue(mRow["ParentID"].ToString()));

                            tbx_ConnectionName.Value = mRow["ConnectionName"].ToString();
                            tbx_Priority.Value = mRow["Priority"].ToString();
                            tbx_MapServiceID.Value = mRow["MapServiceID"].ToString();
                            chk_Active.Checked = (bool)mRow["IsActive"];
                            #endregion
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                mLog.Error(MyNotice.AdminError.LoadDataError, true, ex);
            }

        }
        private void AddNewRow(ref DataSet mSet)
        {
            MyConvert.ConvertDateColumnToStringColumn(ref mSet);
            DataRow mNewRow = mSet.Tables["Child"].NewRow();

            if (EditID > 0)
                mNewRow["ServiceID"] = EditID;

            mNewRow["ServiceName"] = tbx_ServiceName.Value;


            mNewRow["IsActive"] = chk_Active.Checked;

            if (sel_Service.SelectedIndex > 0 && sel_Service.Items.Count > 0)
            {
                mNewRow["ParentID"] = int.Parse(sel_Service.Value);
            }

            int Priority = 0;
            if (int.TryParse(tbx_Priority.Value, out Priority))
            {
                mNewRow["Priority"] = Priority;
            }

            int MapServiceID = 0;
            if (int.TryParse(tbx_MapServiceID.Value, out MapServiceID))
            {
                mNewRow["MapServiceID"] = MapServiceID;
            }

            mNewRow["ConnectionName"] = tbx_ConnectionName.Value;

            mSet.Tables["Child"].Rows.Add(mNewRow);
            mSet.AcceptChanges();
        }

        private void Save(bool IsApply)
        {
            try
            {
                DataSet mSet = mData.CreateDataSet();
                AddNewRow(ref mSet);
                //Nếu là Edit
                if (EditID > 0)
                {
                    if (mData.Update(0, mSet.GetXml()))
                    {
                        #region Log member
                        MemberLog mLog = new MemberLog();
                        MemberLog.ActionType Action = MemberLog.ActionType.Update;
                        mLog.Insert("Service", ViewState["OldData"].ToString(), mSet.GetXml(), Action, true, string.Empty);
                        #endregion

                        if (IsApply)
                            MyMessage.ShowMessage("Cập nhật dữ liệu thành công.");
                        else
                        {
                            Response.Redirect(ParentPath, false);
                        }
                    }
                    else
                    {
                        MyMessage.ShowMessage("Cập nhật dữ liệu (KHÔNG) thành công!");
                    }
                }
                else
                {
                    if (mData.Insert(0, mSet.GetXml()))
                    {
                        #region Log member
                        MemberLog mLog = new MemberLog();
                        MemberLog.ActionType Action = MemberLog.ActionType.Insert;
                        mLog.Insert("Service", string.Empty, mSet.GetXml(), Action, true, string.Empty);
                        #endregion

                        if (IsApply)
                            MyMessage.ShowMessage("Cập nhật dữ liệu thành công.");
                        else
                        {
                            Response.Redirect(ParentPath, false);
                        }
                    }
                    else
                    {
                        MyMessage.ShowMessage("Cập nhật dữ liệu (KHÔNG) thành công!");
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        protected void lbtn_Save_Click(object sender, EventArgs e)
        {
            try
            {
                Save(false);
            }
            catch (Exception ex)
            {
                mLog.Error(MyNotice.AdminError.SaveDataError, true, ex);
            }
        }

        protected void lbtn_Apply_Click(object sender, EventArgs e)
        {
            try
            {
                Save(true);
            }
            catch (Exception ex)
            {
                mLog.Error(MyNotice.AdminError.SaveDataError, true, ex);
            }
        }
    }
}