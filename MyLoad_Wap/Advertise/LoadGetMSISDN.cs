using System;
using System.Collections.Generic;
using System.Text;
using MyBase.MyLoad;
using MyBase.MyWap;
using MyVNPCharging.VNP;
using System.Data;
using MyUtility;
using MyHBReport.Adv;
using System.Web;
using System.Net;

namespace MyLoad_Wap.Advertise
{
    public class LoadGetMSISDN:MyLoadBase
    {

        public LoadGetMSISDN(MyWapBase CurrentWapBase)
        {
            this.CurrentWapBase = CurrentWapBase;
            Init();
        }       
        // Hàm trả về chuỗi có chứa mã HTML
        protected override string BuildHTML()
        {
            try
            {
                string Para_Return = string.Empty;
                string Para = CurrentWapBase.Request.QueryString["para"];
                string ToURL = string.Empty;

                if(string.IsNullOrEmpty(Para))
                {
                    LoadNotify mNotify = new LoadNotify();
                    return mNotify.GetHTML();
                }
                
                ToURL = MySecurity.AES.Decrypt(Para, MySetting.AdminSetting.SpecialKey);

                if(string.IsNullOrEmpty(ToURL))
                {
                    LoadNotify mNotify = new LoadNotify();
                    return mNotify.GetHTML();
                }

                if(ToURL.Contains("?"))
                {
                    ToURL += "&para=";
                }
                else
                {
                    ToURL += "?para=";
                }

                if (string.IsNullOrEmpty(CurrentWapBase.MSISDN))
                {
                    GetMSISDN mVNPGet = new GetMSISDN();
                    CurrentWapBase.MSISDN = mVNPGet.GetPhoneNumber();
                }
                ToURL += HttpUtility.UrlEncode(MySecurity.AES.Encrypt(CurrentWapBase.MSISDN, MySetting.AdminSetting.SpecialKey));

                CurrentWapBase.Response.Redirect(ToURL, false);

                return string.Empty;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
