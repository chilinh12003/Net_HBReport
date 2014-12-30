using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using MyUtility;
using MyHBReport.Permission;
using System.Text;
using System.Collections.Generic;
using MyBase.MyWeb;
namespace MyAdmin.Admin_Adv
{
    public partial class CreatePassAPI : MyASPXBase
    {
        public GetRole mGetRole;
        private bool CheckPermission()
        {
            try
            {
                if (mGetRole.ViewRole == false && Member.MemberGroupID() != 1)
                {
                    Response.Redirect(mGetRole.URLNotView, false);
                    return false;
                }               

            }
            catch (Exception ex)
            {
                mLog.Error(MyNotice.AdminError.CheckPermissionError, true, ex);
                return false;
            }
            return true;
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            //Phân quyền
            if (ViewState["Role"] == null)
            {
                mGetRole = new GetRole(MySetting.AdminSetting.ListPage.Permission, Member.MemberGroupID());
            }
            else
            {
                mGetRole = (GetRole)ViewState["Role"];
            }

            if (!CheckPermission())
                return;

        }
        protected void btn_Gen_Click(object sender, EventArgs e)
        {
              
            try
            {
                string Password = "Dj@#1$%f2" + MySecurity.Encrypt_MD5(tbx_PartnerID.Value).Substring(0, 10);
                tbx_Result.Value = Password;

                //WS_SyncSub.SyncSubSoapClient mSync = new WS_SyncSub.SyncSubSoapClient();
                //string RequestTime = DateTime.Now.ToString("yyyyMMddHHmmss");
                //string Key = tbx_PartnerID.Value + RequestTime + Password;

                //div_Result.InnerHtml = mSync.GetReport( int.Parse( tbx_PartnerID.Value), 0, RequestTime, MySecurity.Encrypt_MD5(Key));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        
        }
    }
}