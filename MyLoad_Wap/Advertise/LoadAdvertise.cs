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
namespace MyLoad_Wap.Advertise
{
    public class LoadAdvertise : MyLoadBase
    {
        int AdvertiseID = 0;
        DateTime ReloadTime = DateTime.MinValue;

        /// <summary>
        /// số giây được phép delay
        /// </summary>
        int MaxDelay = 60;
        string ReloadLink = string.Empty;

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
            catch (Exception ex)
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
                if (!string.IsNullOrEmpty(CurrentWapBase.Request.QueryString["aid"]))
                {
                    int.TryParse(CurrentWapBase.Request.QueryString["aid"], out AdvertiseID);
                }

                if (!string.IsNullOrEmpty(CurrentWapBase.Request.QueryString["para"]))
                {
                    string Para = CurrentWapBase.Request.QueryString["para"];
                    if(!string.IsNullOrEmpty(Para))
                    {
                        string Para_Decode = MySecurity.AES.Decrypt(Para,MySetting.AdminSetting.SpecialKey);
                        DateTime.TryParseExact(Para_Decode, "yyyyMMddHHmmss", null, System.Globalization.DateTimeStyles.None, out ReloadTime);
                    }
                    if(ReloadTime == DateTime.MinValue)
                    {
                        LoadNotify mNotify = new LoadNotify();
                        return mNotify.GetHTML();
                    }
                    if((DateTime.Now - ReloadTime).TotalSeconds > MaxDelay)
                    {
                        LoadNotify mNotify = new LoadNotify();
                        return mNotify.GetHTML();
                    }

                    ReloadLink = string.Empty;

                }
                else
                {
                    string Para = MySecurity.AES.Encrypt(DateTime.Now.ToString("yyyyMMddHHmmss"),MySetting.AdminSetting.SpecialKey);
                    Para = HttpUtility.UrlEncode(Para);

                    ReloadLink = MyConfig.Domain + "/MobileAdv.html?aid=" + AdvertiseID.ToString() + "&para=" + Para;
                }
                

                if (AdvertiseID < 1)
                {
                    LoadNotify mNotify = new LoadNotify();
                    return mNotify.GetHTML();
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
                    mRow["StatusID"] = (int)WapRegLog.Status.NewCreate;
                    mSet.Tables[0].Rows.Add(mRow);

                    mWapRegLog.Insert(0, mSet.GetXml());
                }

                //nếu không có số điện thoại hoặc browser trên desktop thì chuyển sang VNP confirm
                if (string.IsNullOrEmpty(CurrentWapBase.MSISDN))
                {
                    MyLogfile.WriteLogData("_adv", "Step_1_NO_MSISDN:Redirect:" + mAdvObj.ConfirmLink);
                    CurrentWapBase.Response.Redirect(mAdvObj.ConfirmLink,false);
                    return string.Empty;
                }
                //else if (CurrentWapBase.CheckDevice(MyConfig.DeviceType.is_full_desktop))
                //{
                //    MyLogfile.WriteLogData("_adv", "Step_1_is_full_desktop:Redirect:" + mAdvObj.ConfirmLink);
                //    CurrentWapBase.Response.Redirect(mAdvObj.ConfirmLink, false);
                //    return string.Empty;
                //}
                else if (mAdvObj.mStatus != MyHBReport.Adv.Advertise.Status.Active)
                {
                    MyLogfile.WriteLogData("_adv", "Step_1_DeActive:Redirect:" + mAdvObj.ConfirmLink);
                    CurrentWapBase.Response.Redirect(mAdvObj.ConfirmLink, false);
                    return string.Empty;
                }
                else if (mAdvObj.mMethod == MyHBReport.Adv.Advertise.Method.VNPConfirm)
                {
                    MyLogfile.WriteLogData("_adv", "Step_1_VNPConfirm:Redirect:" + mAdvObj.ConfirmLink);
                    CurrentWapBase.Response.Redirect(mAdvObj.ConfirmLink, false);
                    return string.Empty;
                }
                else
                {
                    if (mAdvObj.mMethod == MyHBReport.Adv.Advertise.Method.PassVNPConfirmFirstUse)
                    {
                        Subscriber mSub = new Subscriber(mAdvObj.ConnectionName);
                        DataTable mTable = mSub.Select(1, CurrentWapBase.MSISDN);
                        if (mTable == null || mTable.Rows.Count < 1)
                        {
                            UnSubscriber mUnSub = new UnSubscriber(mAdvObj.ConnectionName);
                            mTable = mUnSub.Select(1, CurrentWapBase.MSISDN);
                        }
                        if (mTable != null && mTable.Rows.Count > 0)
                        {
                            //nếu đã từng sử dụng dịch vụ --> qua VNP confirm
                            MyLogfile.WriteLogData("_adv", "Step_2:Redirect:" + mAdvObj.ConfirmLink);
                            CurrentWapBase.Response.Redirect(mAdvObj.ConfirmLink, false);
                            return string.Empty;
                        }
                    }

                    if (mAdvObj.mMethod == MyHBReport.Adv.Advertise.Method.PassVNPConfirm ||
                        mAdvObj.mMethod == MyHBReport.Adv.Advertise.Method.PassVNPConfirmFirstUse)
                    {
                        WapRegLog mWapRegLog = new WapRegLog(mAdvObj.ConnectionName);
                        string BeginDate = mAdvObj.BeginDate.ToString(MyConfig.DateFormat_InsertToDB);
                        string EndDate = mAdvObj.EndDate.ToString(MyConfig.DateFormat_InsertToDB);

                        DataTable mTable = mWapRegLog.Select(4, ((int)WapRegLog.Status.Registered).ToString(), BeginDate, EndDate);
                        if (mTable != null && mTable.Rows.Count > 0)
                        {
                            if ((int)mTable.Rows[0][0] > mAdvObj.MaxReg)
                            {
                                MyLogfile.WriteLogData("_adv", "Step_3:Redirect:" + mAdvObj.ConfirmLink);
                                CurrentWapBase.Response.Redirect(mAdvObj.ConfirmLink, false);
                                return string.Empty;
                            }
                        }
                    }

                    if(mAdvObj.mMethod == MyHBReport.Adv.Advertise.Method.RegConfirmByWap)
                    {
                        WapRegLog mWapRegLog = new WapRegLog(mAdvObj.ConnectionName);
                        string BeginDate = mAdvObj.BeginDate.ToString(MyConfig.DateFormat_InsertToDB);
                        string EndDate = mAdvObj.EndDate.ToString(MyConfig.DateFormat_InsertToDB);

                        DataTable mTable = mWapRegLog.Select(3, BeginDate, EndDate);
                        if (mTable != null && mTable.Rows.Count > 0)
                        {
                            if ((int)mTable.Rows[0][0] > mAdvObj.MaxReg)
                            {
                                MyLogfile.WriteLogData("_adv", "Step_4:Redirect:" + mAdvObj.ConfirmLink);
                                CurrentWapBase.Response.Redirect(mAdvObj.RedirectLink, false);
                                return string.Empty;
                            }
                        }
                        
                        //nếu là DK qua wap của dịch vụ mà phải confirm
                        MyLogfile.WriteLogData("_adv", "Step_5:Redirect:" + mAdvObj.ConfirmLink);
                        CurrentWapBase.Response.Redirect(mAdvObj.ConfirmLink, false);
                        return string.Empty;
                        
                    }

                    if (mAdvObj.mMethod == MyHBReport.Adv.Advertise.Method.RegNotConfirmByWap)
                    {
                        WapRegLog mWapRegLog = new WapRegLog(mAdvObj.ConnectionName);
                        string BeginDate = mAdvObj.BeginDate.ToString(MyConfig.DateFormat_InsertToDB);
                        string EndDate = mAdvObj.EndDate.ToString(MyConfig.DateFormat_InsertToDB);

                        DataTable mTable = mWapRegLog.Select(3, BeginDate, EndDate);
                        if (mTable != null && mTable.Rows.Count > 0)
                        {
                            if ((int)mTable.Rows[0][0] > mAdvObj.MaxReg)
                            {
                                MyLogfile.WriteLogData("_adv", "Step_6:Redirect:" + mAdvObj.ConfirmLink);
                                CurrentWapBase.Response.Redirect(mAdvObj.RedirectLink, false);
                                return string.Empty;
                            }
                        }

                        MyLogfile.WriteLogData("_adv", "Step_7:Redirect:" + mAdvObj.NotConfirmLink);
                        //nếu là DK qua wap của dịch vụ mà phải confirm
                        CurrentWapBase.Response.Redirect(mAdvObj.NotConfirmLink, false);
                        return string.Empty;
                    }

                    LoadNotConfirm mLoad = null;
                    if(string.IsNullOrEmpty(ReloadLink))
                    {
                        mLoad = new LoadNotConfirm(mAdvObj.NotConfirmLink, mAdvObj.NotConfirmLink, mAdvObj.RedirectDelay, ReloadLink);
                    }
                    else
                    {
                        mLoad = new LoadNotConfirm(mAdvObj.ConfirmLink, mAdvObj.NotConfirmLink, mAdvObj.RedirectDelay, ReloadLink);
                    }
                    
                    string HTML = mLoad.GetHTML();
                    MyLogfile.WriteLogData("_adv", "Step_8:Redirect:" + HTML);
                    return HTML;
                }
            }
            catch (Exception ex)
            {
                MyLogfile.WriteLogData("_Error", ex.Source + "|" + ex.StackTrace + "|" + ex.Message);
                MyLogfile.WriteLogError(ex);
                LoadNotify mNotify = new LoadNotify();
                return mNotify.GetHTML();
            }
        }
    }
}
