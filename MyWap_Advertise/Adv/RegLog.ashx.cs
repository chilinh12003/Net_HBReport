using System;
using System.Collections.Generic;
using System.Web;
using MyBase.MyWap;
using MyUtility;
using MyLoad_Wap.Advertise;

namespace MyWap_Advertise.Adv
{
    /// <summary>
    /// Summary description for RegLog
    /// </summary>
    public class RegLog : MyWapBase
    {
        public override void WriteHTML()
        {
            ContentType = "image/png";
            try
            {
                LoadRegLog mLoad = new LoadRegLog(this);
                mLoad.GetHTML();
            }
            catch (Exception ex)
            {
                mLog.Error(ex);
            }
        }
    }
}