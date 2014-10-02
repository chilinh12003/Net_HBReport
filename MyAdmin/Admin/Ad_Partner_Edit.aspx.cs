using System;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using MyUtility;
using MyHBReport.Permission;
namespace MyAdmin.Admin
{
    public partial class Ad_Partner_Edit : System.Web.UI.Page
    {
        public GetRole mGetRole;
        Partner mPartner = new Partner();

        int EditID = 0;

        public string ParentPath = "../Admin/Ad_Partner.aspx";

        private void BindCombo(int Type)
        {
            switch (Type)
            {
                case 1:
                    sel_PartnerType.DataSource = MyEnum.CrateDatasourceFromEnum(typeof(Partner.PartnerType));
                    sel_PartnerType.DataTextField = "Text";
                    sel_PartnerType.DataValueField = "ID";
                    sel_PartnerType.DataBind();
                    sel_PartnerType.Items.Insert(0, new ListItem("--Nhóm đối tác--", "0"));
                    break;
                case 2:

                    DataTable mTable_Partner = mPartner.Select(2, string.Empty);

                    foreach (DataRow mRow in mTable_Partner.Rows)
                    {
                        if (mRow["ParentID"] != DBNull.Value)
                        {
                            mRow["PartnerName"] = "-->" + mRow["PartnerName"].ToString();
                        }
                    }
                    if (EditID > 0)
                    {
                        mTable_Partner.DefaultView.RowFilter = "PartnerID <> " + EditID;
                    }
                    sel_Partner.DataSource = mTable_Partner;
                    sel_Partner.DataValueField = "PartnerID";
                    sel_Partner.DataTextField = "PartnerName";
                    sel_Partner.DataBind();
                    sel_Partner.Items.Insert(0, new ListItem("--Không chọn--", "0"));
                    break;

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
                MyLogfile.WriteLogError(ex, true, MyNotice.AdminError.CheckPermissionError, "Chilinh");
                return false;
            }
            return true;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                //Phân quyền
                if (ViewState["Role"] == null)
                {
                    mGetRole = new GetRole(MySetting.AdminSetting.ListPage.Partner, Member.MemberGroupID());
                }
                else
                {
                    mGetRole = (GetRole)ViewState["Role"];
                }

                //Lấy memberID nếu là trước hợp Sửa
                EditID = Request.QueryString["ID"] == null ? 0 : int.Parse(Request.QueryString["ID"]);

                if (!CheckPermission())
                    return;

                MyAdmin.MasterPages.Admin mMaster = (MyAdmin.MasterPages.Admin)Page.Master;
                mMaster.str_TitleSearchBox = MyEnum.StringValueOf(MySetting.AdminSetting.ListPage.Partner);

                if (!IsPostBack)
                {
                    BindCombo(1);
                    BindCombo(2);
                    //Nếu là Edit
                    if (EditID > 0)
                    {
                        DataTable mTable = mPartner.Select(1, EditID.ToString());

                        //Lưu lại thông tin OldData để lưu vào MemberLog
                        ViewState["OldData"] = MyXML.GetXML(mTable);

                        if (mTable != null && mTable.Rows.Count > 0)
                        {
                            #region MyRegion
                            DataRow mRow = mTable.Rows[0];
                            tbx_PartnerUserName.Value = mRow["PartnerUserName"].ToString();
                            ViewState["Pass"] = tbx_PartnerPassword.Value = mRow["PartnerPassword"].ToString();
                            
                            ViewState["ValidString"] = mRow["ValidString"].ToString();

                            tbx_Address.Value = mRow["Address"].ToString();
                            tbx_Email.Value = mRow["Email"].ToString();
                            tbx_PartnerName.Value = mRow["PartnerName"].ToString();
                            tbx_Website.Value = mRow["Website"].ToString();
                            tbx_Phone.Value = mRow["Phone"].ToString();
                            chk_Active.Checked = (bool)mRow["IsActive"];

                            tbx_Priority.Value = mRow["Priority"].ToString();
                            if (mRow["ParentID"] != DBNull.Value)
                                sel_Partner.SelectedIndex = sel_Partner.Items.IndexOf(sel_Partner.Items.FindByValue(mRow["ParentID"].ToString()));


                            sel_PartnerType.SelectedIndex = sel_PartnerType.Items.IndexOf(sel_PartnerType.Items.FindByValue(mRow["PartnerTypeID"].ToString()));

                            #endregion
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MyLogfile.WriteLogError(ex, true, MyNotice.AdminError.LoadDataError, "Chilinh");
            }

        }

        private void AddNewRow(ref DataSet mSet)
        {
            MyConvert.ConvertDateColumnToStringColumn(ref mSet);
            DataRow mNewRow = mSet.Tables["Child"].NewRow();
            
            if (EditID > 0)
            {
                mNewRow["PartnerID"] = EditID;
                if (ViewState["Pass"].ToString() != tbx_PartnerPassword.Value)
                {
                    mNewRow["PartnerPassword"] = MySecurity.Encrypt_MD5(tbx_PartnerPassword.Value);
                    mNewRow["ValidString"] = mPartner.CreateValidString(tbx_PartnerUserName.Value, tbx_PartnerPassword.Value);
                }
                else
                {
                    mNewRow["ValidString"] = ViewState["ValidString"].ToString();
                    mNewRow["PartnerPassword"] = ViewState["Pass"].ToString();
                }
            }
            else
            {
                mNewRow["PartnerPassword"] = MySecurity.Encrypt_MD5(tbx_PartnerPassword.Value);
                mNewRow["ValidString"] = mPartner.CreateValidString(tbx_PartnerUserName.Value, tbx_PartnerPassword.Value);
                
            }
            mNewRow["CreateDate"] = DateTime.Now.ToString(MyConfig.DateFormat_InsertToDB);
            mNewRow["PartnerUserName"] = Microsoft.Security.Application.Encoder.HtmlEncode(tbx_PartnerUserName.Value);
            
            mNewRow["PartnerName"] = Microsoft.Security.Application.Encoder.HtmlEncode(tbx_PartnerName.Value);
            mNewRow["Phone"] = Microsoft.Security.Application.Encoder.HtmlEncode(tbx_Phone.Value);
            mNewRow["Email"] = Microsoft.Security.Application.Encoder.HtmlEncode(tbx_Email.Value);
            mNewRow["Phone"] = Microsoft.Security.Application.Encoder.HtmlEncode(tbx_Phone.Value);
            mNewRow["Address"] = Microsoft.Security.Application.Encoder.HtmlEncode(tbx_Address.Value);
            mNewRow["Website"] = Microsoft.Security.Application.Encoder.HtmlEncode(tbx_Website.Value);
            mNewRow["IsActive"] = chk_Active.Checked;
            if (sel_PartnerType.SelectedIndex >= 0 && sel_PartnerType.Items.Count > 0)
            {
                mNewRow["PartnerTypeID"] = int.Parse(sel_PartnerType.Value);
                mNewRow["PartnerTypeName"] = sel_PartnerType.Items[sel_PartnerType.SelectedIndex].Text;
            }
            if (sel_Partner.SelectedIndex > 0 && sel_Partner.Items.Count > 0)
            {
                mNewRow["ParentID"] = int.Parse(sel_Partner.Value);
            }

            int Priority = 0;
            if (int.TryParse(tbx_Priority.Value, out Priority))
            {
                mNewRow["Priority"] = Priority;
            }

            

            mSet.Tables["Child"].Rows.Add(mNewRow);
            mSet.AcceptChanges();
        }

        private void Save(bool IsApply)
        {
            try
            {

                DataSet mSet = mPartner.CreateDataSet();
                AddNewRow(ref mSet);
                //Nếu là Edit
                if (EditID > 0)
                {
                    if (mPartner.Update(0, mSet.GetXml()))
                    {
                        #region Log member
                        MemberLog mLog = new MemberLog();
                        MemberLog.ActionType Action = MemberLog.ActionType.Update;
                        mLog.Insert("Partner", ViewState["OldData"].ToString(), mSet.GetXml(), Action, true, string.Empty);
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
                    if (mPartner.Insert(0, mSet.GetXml()))
                    {
                        #region Log member
                        MemberLog mLog = new MemberLog();
                        MemberLog.ActionType Action = MemberLog.ActionType.Insert;
                        mLog.Insert("Partner", string.Empty, mSet.GetXml(), Action, true, string.Empty);
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
                MyLogfile.WriteLogError(ex, true, MyNotice.AdminError.SaveDataError, "Chilinh");
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
                MyLogfile.WriteLogError(ex, true, MyNotice.AdminError.SaveDataError, "Chilinh");
            }
        }
    }
}
