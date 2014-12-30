using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Data;
using MyUtility;
using MyHBReport.Adv;
namespace MyAdmin.APISync
{

    /// <summary>
    /// Summary description for SyncSub
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class SyncSub : System.Web.Services.WebService
    {
        MyLog mLog = new MyLog(typeof(SyncSub));

        Advertise mAvertise = new Advertise();

        string GetPassword(int PartnerID)
        {
            try
            {
                string Password = "Dj@#1$%f2" + MySecurity.Encrypt_MD5(PartnerID.ToString()).Substring(0, 10);
                return Password;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        bool ValidKey(int PartnerID, string RequestTime, string key)
        {
            try
            {
                string Check = PartnerID.ToString() + RequestTime + GetPassword(PartnerID);
                string Check_EnCode = MySecurity.Encrypt_MD5(Check);
                return key.Equals(Check_EnCode, StringComparison.CurrentCultureIgnoreCase);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        string JSON_Template = "\"result_code\": {ResultCode}," +
                                    "\"results\": [" +
                                    "{" +
                                    "\"NumRegister\": {NumRegister}," +
                                    "\"NumActive\": {NumActive}," +
                                    "\"NumCancel\": {NumCancel}," +
                                    "\"ChargingTotal\": {ChargingTotal}," +
                                    "\"ChargingSuccess\": {ChargingSuccess}," +
                                    "\"Revenue\": {Revenue}" +
                                    "}" +
                                    "]";
        private string BuildResult(string ResultCode, string NumRegister, string NumActive, string NumCancel, string ChargingTotal, string ChargingSuccess, string Revenue)
        {
            try
            {
                return JSON_Template.Replace("{ResultCode}", ResultCode)
                                        .Replace("{NumRegister}", NumRegister)
                                        .Replace("{NumActive}", NumActive)
                                        .Replace("{NumCancel}", NumCancel)
                                        .Replace("{ChargingTotal}", ChargingTotal)
                                        .Replace("{ChargingSuccess}", ChargingSuccess)
                                        .Replace("{Revenue}", Revenue);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private Advertise.AdvertiseObject GetAdvertise(int AdvertiseID)
        {
            Advertise.AdvertiseObject mAdvObj = new Advertise.AdvertiseObject();
            try
            {
                if (AdvertiseID < 1)
                    return mAdvObj;

                DataTable mTable = mAvertise.Select(5, AdvertiseID.ToString());
                if (mTable == null || mTable.Rows.Count < 1)
                    return mAdvObj;

                mAdvObj = mAdvObj.Convert(mTable.Rows[0]);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return mAdvObj;
        }

        [WebMethod]
        public string GetReport(int PartnerID, int aid, string RequestTime, string key)
        {
            Advertise.AdvertiseObject mAdvObj = new MyHBReport.Adv.Advertise.AdvertiseObject();
            try
            {

                if (!ValidKey(PartnerID, RequestTime, key))
                {
                    return BuildResult("0", "0", "0", "0", "0", "0", "0");
                }

                DataTable mTable = new DataTable();
                DateTime BeginDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);
                DateTime EndDate = BeginDate.AddDays(1);
                if (aid > 0)
                {
                    mAdvObj = GetAdvertise(aid);
                    if (mAdvObj.IsNull)
                    {
                        return BuildResult("0", "0", "0", "0", "0", "0", "0");
                    }


                    MyFamousMan.Report.RP_Sub_Partner mRP_Sub = new MyFamousMan.Report.RP_Sub_Partner(mAdvObj.ConnectionName);
                    mTable = mRP_Sub.Select(4, BeginDate.ToString(MyConfig.DateFormat_InsertToDB),
                                                       EndDate.ToString(MyConfig.DateFormat_InsertToDB), mAdvObj.MapPartnerID.ToString());
                }
                else
                {
                    MyFamousMan.Report.RP_Sub_Partner mRP_Sub = new MyFamousMan.Report.RP_Sub_Partner("SQLConnection_FamousMan");
                    mTable = mRP_Sub.Select(4, BeginDate.ToString(MyConfig.DateFormat_InsertToDB),
                                                        EndDate.ToString(MyConfig.DateFormat_InsertToDB), PartnerID.ToString());
                }

                if (mTable.Rows.Count < 1)
                {
                    return BuildResult("1", "0", "0", "0", "0", "0", "0");
                }
                else
                {
                    return BuildResult("1", mTable.Rows[0]["SubTotal"].ToString(), mTable.Rows[0]["SubActive"].ToString(), 
                                            mTable.Rows[0]["UnsubTotal"].ToString(), mTable.Rows[0]["RenewTotal"].ToString(),
                                            mTable.Rows[0]["RenewSuccess"].ToString(), mTable.Rows[0]["SaleRenew"].ToString());
                    
                }
            }
            catch (Exception ex)
            {
                mLog.Error(ex);
                return BuildResult("-1", "0", "0", "0", "0", "0", "0");
            }
            finally
            {
                string format = "Request GetReport -->PartnerID:{0} | aid:{1} | RequestTime:{2} | key:{3}";
                mLog.Info(string.Format(format, new string[] { PartnerID.ToString(), aid.ToString(), RequestTime, key }));
            }
        }
    }
}
