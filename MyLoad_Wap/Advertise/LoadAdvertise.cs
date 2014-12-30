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

                if (string.IsNullOrEmpty(CurrentWapBase.MSISDN))
                {
                    GetMSISDN mVNPGet = new GetMSISDN();
                    CurrentWapBase.MSISDN = mVNPGet.GetPhoneNumber();
                }

                GetAdvertise();

                if(mAdvObj.IsNull)
                {
                    LoadNotify mNotify = new LoadNotify();
                    return mNotify.GetHTML();
                }

                if (mAdvObj.mStatus != MyHBReport.Adv.Advertise.Status.Active)
                {
                    CurrentWapBase.Response.Redirect(mAdvObj.GetRedirectLink, false);
                    return string.Empty;
                }

                //nếu không có số điện thoại hoặc browser trên desktop thì chuyển sang VNP confirm
                if (string.IsNullOrEmpty(CurrentWapBase.MSISDN))
                {
                    CurrentWapBase.Response.Redirect(mAdvObj.GetRedirectLink, false);
                    return string.Empty;
                }

                //if (CurrentWapBase.CheckDevice(MyConfig.DeviceType.is_full_desktop))
                //{
                //    MyLogfile.WriteLogData("_adv", "Step_1_is_full_desktop:Redirect:" + mAdvObj.ConfirmLink);
                //    CurrentWapBase.Response.Redirect(mAdvObj.GetRedirectLink, false);
                //    return string.Empty;
                //} 

                WapRegLog mWapRegLog = new WapRegLog(mAdvObj.ConnectionName);

                //nếu cấu hình số lần request thì sẽ xử lý
                if(mAdvObj.MaxRequest > 0)
                {
                    DataTable mTable_Count = mWapRegLog.Select(6, CurrentWapBase.MSISDN);
                    if(mTable_Count.Rows.Count >=mAdvObj.MaxRequest)
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

                    if(mAdvObj.CheckPass(RegCount))
                    {
                        mAdvObj.InsertWapRegLog(CurrentWapBase.MSISDN, "Redirect:" + mAdvObj.GetRedirectLink);
                        CurrentWapBase.Response.Redirect(mAdvObj.GetRedirectLink, false);
                        return string.Empty;
                    }
                }

                if (mAdvObj.mMethod != MyHBReport.Adv.Advertise.Method.PassVNPConfirm_Party &&
                    mAdvObj.mMethod != MyHBReport.Adv.Advertise.Method.VNPConfirm_Party)
                {
                    mAdvObj.InsertWapRegLog(CurrentWapBase.MSISDN, string.Empty);
                }
          
                if (mAdvObj.mMethod == MyHBReport.Adv.Advertise.Method.VNPConfirm ||
                    mAdvObj.mMethod == MyHBReport.Adv.Advertise.Method.VNPConfirm_Party)
                {
                    CurrentWapBase.Response.Redirect(mAdvObj.ConfirmLink, false);
                    return string.Empty;
                }                   
                else
                {
                    if (mAdvObj.mMethod == MyHBReport.Adv.Advertise.Method.PassVNPConfirmFirstUse)
                    {
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
                            //nếu đã từng sử dụng dịch vụ --> qua VNP confirm
                            CurrentWapBase.Response.Redirect(mAdvObj.ConfirmLink, false);
                            return string.Empty;
                        } 
                        #endregion
                    }

                    if (mAdvObj.mMethod == MyHBReport.Adv.Advertise.Method.PassVNPConfirm ||
                        mAdvObj.mMethod == MyHBReport.Adv.Advertise.Method.PassVNPConfirmFirstUse)
                    {
                        #region MyRegion
                        string BeginDate = mAdvObj.BeginDate.ToString(MyConfig.DateFormat_InsertToDB);
                        string EndDate = mAdvObj.EndDate.ToString(MyConfig.DateFormat_InsertToDB);

                        DataTable mTable = mWapRegLog.Select(4, ((int)WapRegLog.Status.Registered).ToString(), BeginDate, EndDate);
                        if (mTable != null && mTable.Rows.Count > 0)
                        {
                            if ((int)mTable.Rows[0][0] > mAdvObj.MaxReg)
                            {
                                CurrentWapBase.Response.Redirect(mAdvObj.ConfirmLink, false);
                                return string.Empty;
                            }
                        } 
                        #endregion
                    }

                    if(mAdvObj.mMethod == MyHBReport.Adv.Advertise.Method.RegConfirmByWap)
                    {
                        #region MyRegion
                        string BeginDate = mAdvObj.BeginDate.ToString(MyConfig.DateFormat_InsertToDB);
                        string EndDate = mAdvObj.EndDate.ToString(MyConfig.DateFormat_InsertToDB);

                        DataTable mTable = mWapRegLog.Select(3, BeginDate, EndDate);
                        if (mTable != null && mTable.Rows.Count > 0)
                        {
                            if ((int)mTable.Rows[0][0] > mAdvObj.MaxReg)
                            {
                                CurrentWapBase.Response.Redirect(mAdvObj.RedirectLink, false);
                                return string.Empty;
                            }
                        }

                        //nếu là DK qua wap của dịch vụ mà phải confirm
                        CurrentWapBase.Response.Redirect(mAdvObj.ConfirmLink, false);
                        return string.Empty;
                        
                        #endregion
                    }

                    if (mAdvObj.mMethod == MyHBReport.Adv.Advertise.Method.RegNotConfirmByWap)
                    {
                        #region MyRegion
                        string BeginDate = mAdvObj.BeginDate.ToString(MyConfig.DateFormat_InsertToDB);
                        string EndDate = mAdvObj.EndDate.ToString(MyConfig.DateFormat_InsertToDB);

                        DataTable mTable = mWapRegLog.Select(3, BeginDate, EndDate);
                        if (mTable != null && mTable.Rows.Count > 0)
                        {
                            if ((int)mTable.Rows[0][0] > mAdvObj.MaxReg)
                            {
                                CurrentWapBase.Response.Redirect(mAdvObj.RedirectLink, false);
                                return string.Empty;
                            }
                        }

                        //nếu là DK qua wap của dịch vụ mà phải confirm
                        CurrentWapBase.Response.Redirect(mAdvObj.NotConfirmLink, false);
                        return string.Empty; 
                        #endregion
                    }
                    
                    if (mAdvObj.mMethod == MyHBReport.Adv.Advertise.Method.RegNotConfirmByWap_11)
                    {
                        if (CurrentWapBase.MSISDN.StartsWith("8491"))
                        {
                            //nếu là đầu 9 thì chuyển đến trang confirm
                            CurrentWapBase.Response.Redirect(mAdvObj.ConfirmLink, false);
                            return string.Empty;
                        }
                        else
                        {
                            #region MyRegion
                            string BeginDate = mAdvObj.BeginDate.ToString(MyConfig.DateFormat_InsertToDB);
                            string EndDate = mAdvObj.EndDate.ToString(MyConfig.DateFormat_InsertToDB);

                            DataTable mTable = mWapRegLog.Select(3, BeginDate, EndDate);
                            if (mTable != null && mTable.Rows.Count > 0)
                            {
                                if ((int)mTable.Rows[0][0] > mAdvObj.MaxReg)
                                {
                                    CurrentWapBase.Response.Redirect(mAdvObj.ConfirmLink, false);
                                    return string.Empty;
                                }
                            }

                            //nếu là DK qua wap của dịch vụ mà phải confirm
                            CurrentWapBase.Response.Redirect(mAdvObj.NotConfirmLink, false);
                            return string.Empty;
                            #endregion
                        }
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
                    return HTML;
                }
            }
            catch (Exception ex)
            {
                mLog.Error(ex);
                LoadNotify mNotify = new LoadNotify();
                return mNotify.GetHTML();
            }
        }

    }
}
