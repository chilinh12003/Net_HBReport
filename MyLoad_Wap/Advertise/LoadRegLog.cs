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
    public class LoadRegLog : MyLoadBase
    {
        int AdvertiseID = 0;

        MyHBReport.Adv.Advertise.AdvertiseObject mAdvObj = new MyHBReport.Adv.Advertise.AdvertiseObject();
        MyHBReport.Adv.Advertise mAvertise = new MyHBReport.Adv.Advertise();
        public LoadRegLog(MyWapBase CurrentWapBase)
        {
            this.CurrentWapBase = CurrentWapBase;
            Init();
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
                    return string.Empty;
                }

                if (string.IsNullOrEmpty(CurrentWapBase.MSISDN))
                {
                    GetMSISDN mVNPGet = new GetMSISDN();
                    CurrentWapBase.MSISDN = mVNPGet.GetPhoneNumber();
                }

                GetAdvertise();

                if (mAdvObj.IsNull)
                {
                    return string.Empty;
                }

                if (mAdvObj.mStatus != MyHBReport.Adv.Advertise.Status.Active)
                {
                    return string.Empty;
                }

                //nếu không có số điện thoại hoặc browser trên desktop thì chuyển sang VNP confirm
                if (string.IsNullOrEmpty(CurrentWapBase.MSISDN))
                {
                    return string.Empty;
                }               

                WapRegLog mWapRegLog = new WapRegLog(mAdvObj.ConnectionName);

                if (mAdvObj.PassPercent > 0)
                {
                    DataTable mTable_Count = mWapRegLog.Select(8, mAdvObj.MapPartnerID.ToString());
                    int RegCount = 0;
                    if (mTable_Count.Rows.Count > 0)
                        int.TryParse(mTable_Count.Rows[0][0].ToString(), out RegCount);

                    if (mAdvObj.CheckPass(RegCount))
                    {
                        mAdvObj.InsertWapRegLog(CurrentWapBase.MSISDN, "Redirect:" + mAdvObj.GetRedirectLink);
                        CurrentWapBase.Response.Redirect(mAdvObj.GetRedirectLink, false);
                        return string.Empty;
                    }
                }
                else
                {
                    mAdvObj.InsertWapRegLog(CurrentWapBase.MSISDN, string.Empty);
                }
            }
            catch (Exception ex)
            {
                mLog.Error(ex);
            }
            return string.Empty;
        }
    }

}
