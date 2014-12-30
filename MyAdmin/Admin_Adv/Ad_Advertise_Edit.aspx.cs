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
    public partial class Ad_Advertise_Edit : System.Web.UI.Page
    {
        public GetRole mGetRole;
        MyLog mLog = new MyLog(typeof(Ad_Advertise_Edit));
        Advertise mData = new Advertise();
        int EditID = 0;

        public string ParentPath = "../Admin_Adv/Ad_Advertise.aspx";

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
                        sel_Service.DataSource = mTable_Service;
                        sel_Service.DataValueField = "ServiceID";
                        sel_Service.DataTextField = "ServiceName";
                        sel_Service.DataBind();
                        break;
                    case 2://Đối tác
                        Partner mPartner = new Partner();
                        DataTable mTable_Partner = mPartner.Select(2, string.Empty);
                        foreach (DataRow mRow in mTable_Partner.Rows)
                        {
                            if (mRow["ParentID"] != DBNull.Value)
                            {
                                mRow["PartnerName"] = "-->" + mRow["PartnerName"].ToString();
                            }
                        }
                        sel_Partner.DataSource = mTable_Partner;
                        sel_Partner.DataValueField = "PartnerID";
                        sel_Partner.DataTextField = "PartnerName";
                        sel_Partner.DataBind();
                        break;
                    case 3://Phương thức quảng cáo
                        sel_Method.DataSource = MyEnum.CrateDatasourceFromEnum(typeof(Advertise.Method), true);
                        sel_Method.DataTextField = "Text";
                        sel_Method.DataValueField = "ID";
                        sel_Method.DataBind();
                        break;
                    case 4://Tình trạng
                        sel_Status.DataSource = MyEnum.CrateDatasourceFromEnum(typeof(Advertise.Status), true);
                        sel_Status.DataTextField = "Text";
                        sel_Status.DataValueField = "ID";
                        sel_Status.DataBind();
                        break;
                    case 5: // Bind dữ liệu về giờ
                        sel_BeginHour.DataSource = MyEnum.GetDataFromTime(3, string.Empty, string.Empty);
                        sel_BeginHour.DataValueField = "ID";
                        sel_BeginHour.DataTextField = "Text";
                        sel_BeginHour.DataBind();

                        sel_BeginHour.Items.Insert(0, new ListItem("--Giờ--", "-1"));

                        break;
                    case 6: // Bind dữ liệu về Phút
                        sel_BeginMinute.DataSource = MyEnum.GetDataFromTime(4, string.Empty, string.Empty);
                        sel_BeginMinute.DataValueField = "ID";
                        sel_BeginMinute.DataTextField = "Text";
                        sel_BeginMinute.DataBind();
                        sel_BeginMinute.Items.Insert(0, new ListItem("--Phút--", "0"));
                        break;
                    case 7: // Bind dữ liệu về giờ
                        sel_EndHour.DataSource = MyEnum.GetDataFromTime(3, string.Empty, string.Empty);
                        sel_EndHour.DataValueField = "ID";
                        sel_EndHour.DataTextField = "Text";
                        sel_EndHour.DataBind();

                        sel_EndHour.Items.Insert(0, new ListItem("--Giờ--", "-1"));

                        break;
                    case 8: // Bind dữ liệu về Phút
                        sel_EndMinute.DataSource = MyEnum.GetDataFromTime(4, string.Empty, string.Empty);
                        sel_EndMinute.DataValueField = "ID";
                        sel_EndMinute.DataTextField = "Text";
                        sel_EndMinute.DataBind();
                        sel_EndMinute.Items.Insert(0, new ListItem("--Phút--", "0"));
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
                sel_Status.Disabled = !mGetRole.PublishRole;

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

                //Lấy memberID nếu là trước hợp Sửa
                EditID = Request.QueryString["ID"] == null ? 0 : int.Parse(Request.QueryString["ID"]);

                MyAdmin.MasterPages.Admin mMaster = (MyAdmin.MasterPages.Admin)Page.Master;
                mMaster.str_PageTitle = mGetRole.PageName;
                mMaster.str_TitleSearchBox = "Thông tin về " + mGetRole.PageName;

                if (!IsPostBack)
                {

                    BindCombo(1);
                    BindCombo(2);
                    BindCombo(3);
                    BindCombo(4);
                    BindCombo(5);

                    BindCombo(6);
                    BindCombo(7);
                    BindCombo(8);
                  
                    tbx_BeginDate.Value = DateTime.Now.ToString("dd/MM/yyyy");
                    tbx_EndDate.Value = DateTime.Now.ToString("dd/MM/yyyy");
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
                           

                            if (mRow["ServiceID"] != DBNull.Value)
                                sel_Service.SelectedIndex = sel_Service.Items.IndexOf(sel_Service.Items.FindByValue(mRow["ServiceID"].ToString()));

                            if (mRow["PartnerID"] != DBNull.Value)
                                sel_Partner.SelectedIndex = sel_Partner.Items.IndexOf(sel_Partner.Items.FindByValue(mRow["PartnerID"].ToString()));

                            if (mRow["MethodID"] != DBNull.Value)
                                sel_Method.SelectedIndex = sel_Method.Items.IndexOf(sel_Method.Items.FindByValue(mRow["MethodID"].ToString()));

                            if (mRow["StatusID"] != DBNull.Value)
                                sel_Status.SelectedIndex = sel_Status.Items.IndexOf(sel_Status.Items.FindByValue(mRow["StatusID"].ToString()));


                            tbx_AdvertiseName.Value = mRow["AdvertiseName"].ToString();
                            tbx_ConfirmLink.Value = mRow["ConfirmLink"].ToString();
                            tbx_NotConfirmLink.Value = mRow["NotConfirmLink"].ToString();
                            tbx_RedirectLink.Value = mRow["RedirectLink"].ToString();
                            tbx_PassLink.Value = mRow["PassLink"].ToString();
                            tbx_UsedLink.Value = mRow["UsedLink"].ToString();
                            tbx_LogMSISDNLink.Value = mRow["LogMSISDNLink"].ToString();

                            tbx_MaxReg.Value = mRow["MaxReg"].ToString();
                            tbx_MapPartnerID.Value = mRow["MapPartnerID"].ToString();
                            tbx_RedirectDelay.Value = mRow["RedirectDelay"].ToString();

                            tbx_MaxRequest.Value = mRow["MaxRequest"].ToString();
                            tbx_PassPercent.Value = mRow["PassPercent"].ToString();

                            if (mRow["BeginDate"] != DBNull.Value)
                            {
                                DateTime mDateTime = (DateTime)mRow["BeginDate"];
                                tbx_BeginDate.Value = mDateTime.ToString(MyConfig.ShortDateFormat);
                                sel_BeginHour.SelectedIndex = sel_BeginHour.Items.IndexOf(sel_BeginHour.Items.FindByValue(mDateTime.Hour.ToString()));
                                sel_BeginMinute.SelectedIndex = sel_BeginMinute.Items.IndexOf(sel_BeginMinute.Items.FindByValue(mDateTime.Minute.ToString()));
                            }
                            if (mRow["EndDate"] != DBNull.Value)
                            {
                                DateTime mDateTime = (DateTime)mRow["EndDate"];
                                tbx_EndDate.Value = mDateTime.ToString(MyConfig.ShortDateFormat);
                                sel_EndHour.SelectedIndex = sel_EndHour.Items.IndexOf(sel_EndHour.Items.FindByValue(mDateTime.Hour.ToString()));
                                sel_EndMinute.SelectedIndex = sel_EndMinute.Items.IndexOf(sel_EndMinute.Items.FindByValue(mDateTime.Minute.ToString()));
                            }
                           
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
            {

                mNewRow["AdvertiseID"] = EditID;
                mNewRow["UpdateDate"] = DateTime.Now.ToString(MyConfig.DateFormat_InsertToDB);
            }
            else
            {
                mNewRow["CreateDate"] = DateTime.Now.ToString(MyConfig.DateFormat_InsertToDB);
            }
          
            mNewRow["ServiceID"] = sel_Service.Value;
            mNewRow["ServiceName"] = sel_Service.Items[sel_Service.SelectedIndex].Text;

            mNewRow["PartnerID"] = int.Parse(sel_Partner.Value);
            mNewRow["PartnerName"] = sel_Partner.Items[sel_Partner.SelectedIndex].Text;

            mNewRow["StatusID"] = int.Parse(sel_Status.Value);
            mNewRow["StatusName"] = sel_Status.Items[sel_Status.SelectedIndex].Text;

            mNewRow["MethodID"] = int.Parse(sel_Method.Value);
            mNewRow["MethodName"] = sel_Method.Items[sel_Method.SelectedIndex].Text;

            mNewRow["AdvertiseName"] = tbx_AdvertiseName.Value;

            mNewRow["ConfirmLink"] = tbx_ConfirmLink.Value;
            mNewRow["NotConfirmLink"] = tbx_NotConfirmLink.Value;
            mNewRow["RedirectLink"] = tbx_RedirectLink.Value;
            mNewRow["PassLink"] = tbx_PassLink.Value;
            mNewRow["UsedLink"] = tbx_UsedLink.Value;
            mNewRow["LogMSISDNLink"] = tbx_LogMSISDNLink.Value; 

            int MaxReg = 0;
            if (int.TryParse(tbx_MaxReg.Value, out MaxReg))
            {
                mNewRow["MaxReg"] = MaxReg;
            }

            int MapPartnerID = 0;
            if (int.TryParse(tbx_MapPartnerID.Value, out MapPartnerID))
            {
                mNewRow["MapPartnerID"] = MapPartnerID;
            }

            int RedirectDelay = 0;
            if (int.TryParse(tbx_RedirectDelay.Value, out RedirectDelay))
            {
                mNewRow["RedirectDelay"] = RedirectDelay;
            }
            
								
            int MaxRequest = 0;
            if (int.TryParse(tbx_MaxRequest.Value, out MaxRequest))
            {
                mNewRow["MaxRequest"] = MaxRequest;
            }

            int PassPercent = 0;
            if (int.TryParse(tbx_PassPercent.Value, out PassPercent))
            {
                mNewRow["PassPercent"] = PassPercent;
            }

            if (tbx_BeginDate.Value.Length > 0)
            {
                int Hour = 0;
                int Minute = 0;
                int Second = 0;
                DateTime TempDate = DateTime.ParseExact(tbx_BeginDate.Value, "dd/MM/yyyy", null);

                if (sel_BeginHour.SelectedIndex > 0)
                    int.TryParse(sel_BeginHour.Value, out Hour);
                if (sel_BeginMinute.SelectedIndex > 0)
                    int.TryParse(sel_BeginMinute.Value, out Minute);

                mNewRow["BeginDate"] = new DateTime(TempDate.Year, TempDate.Month, TempDate.Day, Hour, Minute, Second).ToString(MyConfig.DateFormat_InsertToDB);
            }
              
            if (tbx_EndDate.Value.Length > 0)
            {
                int Hour = 0;
                int Minute = 0;
                int Second = 0;
                DateTime TempDate = DateTime.ParseExact(tbx_EndDate.Value, "dd/MM/yyyy", null);

                if (sel_EndHour.SelectedIndex > 0)
                    int.TryParse(sel_EndHour.Value, out Hour);
                if (sel_EndMinute.SelectedIndex > 0)
                    int.TryParse(sel_EndMinute.Value, out Minute);

                mNewRow["EndDate"] = new DateTime(TempDate.Year, TempDate.Month, TempDate.Day, Hour, Minute, Second).ToString(MyConfig.DateFormat_InsertToDB);
            }

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
                        mLog.Insert("Advertise", ViewState["OldData"].ToString(), mSet.GetXml(), Action, true, string.Empty);
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
                        mLog.Insert("Advertise", string.Empty, mSet.GetXml(), Action, true, string.Empty);
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
