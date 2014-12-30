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
    public class LoadRegPartner : MyLoadBase
    {
        int AdvertiseID = 0;

        MyHBReport.Adv.Advertise.AdvertiseObject mAdvObj = new MyHBReport.Adv.Advertise.AdvertiseObject();
        MyHBReport.Adv.Advertise mAvertise = new MyHBReport.Adv.Advertise();

        public LoadRegPartner(MyWapBase CurrentWapBase)
        {
            mTemplatePath = "~/Templates/Static/RegPartner.html";
            this.CurrentWapBase = CurrentWapBase;
            Init();
        }
        public string getRemoteAddr()
        {
            try
            {
                string ipaddress = MyCurrent.CurrentPage.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                if (string.IsNullOrEmpty(ipaddress))
                {
                    ipaddress = MyCurrent.CurrentPage.Request.ServerVariables["REMOTE_ADDR"] == null ? string.Empty : MyCurrent.CurrentPage.Request.ServerVariables["REMOTE_ADDR"];
                }
                ipaddress = ipaddress.Trim();
                string[] arr = ipaddress.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                if (arr.Length > 1)
                {
                    if (!string.IsNullOrEmpty(arr[arr.Length - 1]))
                    {
                        return arr[arr.Length - 1];
                    }
                    else if (string.IsNullOrEmpty(arr[arr.Length - 1]) && !string.IsNullOrEmpty(arr[0]))
                    {
                        return arr[0];
                    }
                    else
                        return ipaddress;

                }
                return ipaddress;
            }
            catch (Exception ex)
            {
                mLog.Error(ex);
                throw ex;
            }
        }

        public bool CheckIP_VNP()
        {

            try
            {
                string IPCheck = string.Empty;
                string F5IPPattern = "(^(10)(\\.([0-9]|[1-9][0-9]|1[0-9][0-9]|2[0-4][0-9]|25[0-5])){3}$)|(^(113\\.185\\.)([1-9]|1[0-9]|2[0-9]|3[0-1])(\\.([0-9]|[1-9][0-9]|1[0-9][0-9]|2[0-4][0-9]|25[0-5])))";
                string WAPGWIPPattern = "(^172.16.30.1[1-2]$)|(113.185.0.16)";

                IPCheck = getRemoteAddr();

                if (string.IsNullOrEmpty(IPCheck))
                    return false;

                string[] arr = IPCheck.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string item in arr)
                {
                    if (string.IsNullOrEmpty(item.Trim()))
                    {
                        continue;
                    }

                    if (item.StartsWith("113.185."))
                        return true;

                    if (Regex.IsMatch(item.Trim(), F5IPPattern, RegexOptions.IgnoreCase))
                        return true;

                    if (Regex.IsMatch(item.Trim(), WAPGWIPPattern, RegexOptions.IgnoreCase))
                        return true;
                }
            }
            catch (Exception ex)
            {
                mLog.Error(ex);
            }
            return false;
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


                ////nếu không phải ip của vinaphone thì ko cho vào
                //if (!CheckIP_VNP())
                //{
                //    if (string.IsNullOrEmpty(mAdvObj.GetRedirectLink))
                //    {
                //        LoadNotify mNotify = new LoadNotify();
                //        return mNotify.GetHTML();
                //    }
                //    else
                //    {
                //        CurrentWapBase.Response.Redirect(mAdvObj.GetRedirectLink, false);
                //        return string.Empty;
                //    }
                //}

                //#region MyRegion
                //Subscriber mSub = new Subscriber(mAdvObj.ConnectionName);
                //DataTable mTable = mSub.Select(1, CurrentWapBase.MSISDN);
                //if (mTable == null || mTable.Rows.Count < 1)
                //{
                //    UnSubscriber mUnSub = new UnSubscriber(mAdvObj.ConnectionName);
                //    mTable = mUnSub.Select(1, CurrentWapBase.MSISDN);
                //}
                //if (mTable != null && mTable.Rows.Count > 0)
                //{
                //    //nếu đã từng sử dụng dịch vụ --> qua VNP confirm
                //    CurrentWapBase.Response.Redirect(mAdvObj.ConfirmLink, false);
                //    return string.Empty;
                //}
                //#endregion

                WapRegLog mWapRegLog = new WapRegLog(mAdvObj.ConnectionName);

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

                string HTML = MyFile.ReadFile(MyFile.GetFullPathFile(mTemplatePath));
                
                HTML = HTML.Replace("{DNS}", MyConfig.Domain);
                HTML = HTML.Replace("{RedirectDelay}", mAdvObj.RedirectDelay.ToString());
                HTML = HTML.Replace("{NotConfirmLink}", mAdvObj.NotConfirmLink);
                HTML = HTML.Replace("{LogMSISDNLink}", mAdvObj.LogMSISDNLink);
                HTML = HTML.Replace("{aid}", AdvertiseID.ToString());
                return HTML;
            }
            catch (Exception ex)
            {
                mLog.Error(ex);
            }
            return string.Empty;
        }
    }

}
