using System;
using System.Collections.Generic;
using System.Web;
using MyBase.MyWap;
using MyUtility;
using MyLoad_Wap.Advertise;
namespace MyWap_Advertise.Adv
{
    /// <summary>
    /// Summary description for MobileAdv
    /// </summary>
    public class MobileAdv : MyWapBase
    {
        public override void WriteHTML()
        {
            try
            {               
                LoadAdvertise mLoad = new LoadAdvertise(this);
                Write(mLoad.GetHTML());
            }
            catch(Exception ex)
            {
                mLog.Error(ex);
            }
        }
    }
}