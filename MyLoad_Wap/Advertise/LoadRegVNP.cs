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
using System.Text.RegularExpressions;

namespace MyLoad_Wap.Advertise
{
    public class LoadRegVNP : MyLoadBase
    {
        int AdvertiseID = 0;
        string Para = string.Empty;

        MyHBReport.Adv.Advertise.AdvertiseObject mAdvObj = new MyHBReport.Adv.Advertise.AdvertiseObject();
        MyHBReport.Adv.Advertise mAvertise = new MyHBReport.Adv.Advertise();

        public LoadRegVNP(MyWapBase CurrentWapBase)
        {
            this.CurrentWapBase = CurrentWapBase;
            Init();
        }

        private static string _GetMSISDNLink = string.Empty;
        public static string GetMSISDNLink
        {
            get
            {
               
                if(string.IsNullOrEmpty(_GetMSISDNLink))
                {
                    string Temp = MyConfig.GetKeyInConfigFile("GetMSISDNLink");
                    if(string.IsNullOrEmpty(Temp))
                    {
                        _GetMSISDNLink = "http://qc.zonevui.vn/GetMSISDN.html?para=";
                    }
                    else
                    {
                        _GetMSISDNLink = Temp;
                    }
                }
                return _GetMSISDNLink;
            }
        }
 
        /// <summary>
        /// Lấy thông tin quản cáo
        /// </summary>
        private void GetAdvertise()
        {
            try
            {
                if (AdvertiseID < 1)
                    return;

                DataTable mTable = mAvertise.Select(5, AdvertiseID.ToString());
                if (mTable == null || mTable.Rows.Count < 1)
                    return;

                mAdvObj = mAdvObj.Convert(mTable.Rows[0]);
            }
            catch (Exception ex)
            {
                mLog.Error(ex);
                throw ex;
            }
        }
        // Hàm trả về chuỗi có chứa mã HTML
        protected override string BuildHTML()
        {
            try
            {
                if (!string.IsNullOrEmpty(CurrentWapBase.Request.QueryString["aid"]))
                {
                    int.TryParse(CurrentWapBase.Request.QueryString["aid"], out AdvertiseID);
                }

                if (AdvertiseID < 1)
                {
                    LoadNotify mNotify = new LoadNotify();
                    return mNotify.GetHTML();
                }

                if (CurrentWapBase.Request.QueryString["para"] == null)
                {
                    Para = MyConfig.Domain + "/RegVNP.html?aid=" + AdvertiseID.ToString();
                    string Para_Encode = MySecurity.AES.Encrypt(Para, MySetting.AdminSetting.SpecialKey);
                    Para_Encode = HttpUtility.UrlEncode(Para_Encode);

                    //Chạy sang lấy msisdn
                    CurrentWapBase.Response.Redirect(GetMSISDNLink + Para_Encode, false);
                    return string.Empty;
                }
                else
                {
                    Para = CurrentWapBase.Request.QueryString["para"];
                    CurrentWapBase.MSISDN = MySecurity.AES.Decrypt(Para, MySetting.AdminSetting.SpecialKey);
                }

                GetAdvertise();

                if (mAdvObj.IsNull)
                {
                      LoadNotify mNotify = new LoadNotify();
                    return mNotify.GetHTML();
                }

                if (mAdvObj.mStatus != MyHBReport.Adv.Advertise.Status.Active)
                {
                    if (string.IsNullOrEmpty(mAdvObj.GetRedirectLink))
                    {
                        LoadNotify mNotify = new LoadNotify();
                        return mNotify.GetHTML();
                    }
                    else
                    {
                        CurrentWapBase.Response.Redirect(mAdvObj.GetRedirectLink, false);
                        return string.Empty;
                    }
                }

                if (string.IsNullOrEmpty(CurrentWapBase.MSISDN))
                {
                    CurrentWapBase.Response.Redirect(mAdvObj.GetRedirectLink, false);
                    return string.Empty;
                }

                #region MyRegion
                Subscriber mSub = new Subscriber(mAdvObj.ConnectionName);
                DataTable mTable = mSub.Select(1, CurrentWapBase.MSISDN);
                if (mTable == null || mTable.Rows.Count < 1)
                {
                    UnSubscriber mUnSub = new UnSubscriber(mAdvObj.ConnectionName);
                    mTable = mUnSub.Select(1, CurrentWapBase.MSISDN);
                }

                if (mTable != null && mTable.Rows.Count > 0)
                {
                    //nếu đã từng sử dụng dịch vụ
                    CurrentWapBase.Response.Redirect(mAdvObj.GetUsedLink, false);
                    return string.Empty;
                }
                #endregion

                WapRegLog mWapRegLog = new WapRegLog(mAdvObj.ConnectionName);

                CurrentWapBase.Response.Redirect(mAdvObj.NotConfirmLink, false);

                //nếu cấu hình số lần request thì sẽ xử lý
                if (mAdvObj.MaxRequest > 0)
                {
                    DataTable mTable_Count = mWapRegLog.Select(6, CurrentWapBase.MSISDN);
                    if (mTable_Count.Rows.Count >= mAdvObj.MaxRequest)
                    {
                        CurrentWapBase.Response.Redirect(mAdvObj.GetRedirectLink, false);
                        return string.Empty;
                    }
                }

                if (mAdvObj.PassPercent > 0)
                {
                    DataTable mTable_Count = mWapRegLog.Select(8, mAdvObj.MapPartnerID.ToString());
                    int RegCount = 0;
                    if (mTable_Count.Rows.Count > 0)
                        int.TryParse(mTable_Count.Rows[0][0].ToString(), out RegCount);

                    if (mAdvObj.CheckPass(RegCount))
                    {
                        mAdvObj.InsertWapRegLog(CurrentWapBase.MSISDN, "Redirect:" + mAdvObj.PassLink);
                        CurrentWapBase.Response.Redirect(mAdvObj.PassLink, false);
                        return string.Empty;
                    }
                }
                
                mAdvObj.InsertWapRegLog(CurrentWapBase.MSISDN, string.Empty);

                
            }
            catch (Exception ex)
            {
                mLog.Error(ex);
            }
            return string.Empty;
        }
    }

}
