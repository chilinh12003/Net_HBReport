using System;
using System.Collections.Generic;
using System.Web;
using MyBase.MyWap;
using MyUtility;
using MyLoad_Wap.Advertise;

namespace MyWap_Advertise.Adv
{
    /// <summary>
    /// Summary description for Redirect
    /// </summary>
    public class Redirect : MyWapBase
    {
        public override void WriteHTML()
        {
            try
            {
                Response.Redirect("http://wapgate.vinaphone.com.vn/MobileAds2014/reg/HBTPTT/CP/TRIEUPHUTT/TRIEUPHUTT/1", false);
            }
            catch(Exception ex)
            {
                MyLogfile.WriteLogError("_Error", ex, false, MyNotice.EndUserError.LoadDataError, "Chilinh");
                MyLogfile.WriteLogData("_Error", ex.Message);
            }
        }
    }
}