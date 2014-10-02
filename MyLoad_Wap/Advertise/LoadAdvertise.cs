using System;
using System.Collections.Generic;
using System.Text;
using MyBase.MyLoad;
using MyBase.MyWap;
using MyVNPCharging.VNP;
using System.Data;
using MyUtility;
using MyHBReport.Adv;
namespace MyLoad_Wap.Advertise
{
    public class LoadAdvertise:MyLoadBase
    {
        int AdvertiseID = 0;
       MyHBReport.Adv.Advertise.AdvertiseObject mAdvObj = new MyHBReport.Adv.Advertise.AdvertiseObject();
       MyHBReport.Adv.Advertise mAvertise = new MyHBReport.Adv.Advertise();
        public LoadAdvertise(MyWapBase CurrentWapBase)
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
            catch(Exception ex)
            {
                MyLogfile.WriteLogError(ex);
                throw ex;
            }
        }
        // Hàm trả về chuỗi có chứa mã HTML
        protected override string BuildHTML()
        {
            try
            {
                if(string.IsNullOrEmpty( CurrentWapBase.Request.QueryString["aid"]))
                {
                    int.TryParse(CurrentWapBase.Request.QueryString["aid"], out AdvertiseID);
                }

                if (string.IsNullOrEmpty(CurrentWapBase.MSISDN))
                {
                    GetMSISDN mVNPGet = new GetMSISDN();
                    CurrentWapBase.MSISDN = mVNPGet.GetPhoneNumber();
                }
                GetAdvertise();

                if (!string.IsNullOrEmpty(CurrentWapBase.MSISDN))
                {
                    WapRegLog mWapRegLog = new WapRegLog(mAdvObj.ConnectionName);
                   
                    DataSet mSet = mWapRegLog.CreateDataSet();
                    MyConvert.ConvertDateColumnToStringColumn(ref mSet);
                    DataRow mRow = mSet.Tables[0].NewRow();
                    mRow["MSISDN"] = CurrentWapBase.MSISDN;
                    mRow["PID"] = 0;
                    mRow["PartnerID"] = mAdvObj.MapPartnerID;
                    mRow["CreateDate"] = DateTime.Now.ToString(MyConfig.DateFormat_InsertToDB);
                    mSet.Tables[0].Rows.Add(mRow);

                    mWapRegLog.Insert(0, mSet.GetXml());
                }

                //nếu không có số điện thoại hoặc browser trên desktop thì chuyển sang VNP confirm
                if (string.IsNullOrEmpty(CurrentWapBase.MSISDN) || CurrentWapBase.CheckDevice(MyConfig.DeviceType.is_full_desktop)
                    || mAdvObj.mStatus != MyHBReport.Adv.Advertise.Status.Active
                    || mAdvObj.mMethod == MyHBReport.Adv.Advertise.Method.VNPConfirm)
                {
                    CurrentWapBase.Response.Redirect(mAdvObj.ConfirmLink);
                }
                else
                {
                    LoadNotConfirm mLoad = new LoadNotConfirm(mAdvObj.ConfirmLink, mAdvObj.NotConfirmLink, mAdvObj.RedirectDelay);
                    return mLoad.GetHTML();
                }
                return string.Empty;
            }
            catch (Exception ex)
            {
                MyLogfile.WriteLogData("_Error", ex.Source + "|" + ex.StackTrace + "|" + ex.Message);
                throw ex;

            }
        }
    }
}
