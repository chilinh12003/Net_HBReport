using System;
using System.Collections.Generic;
using System.Web;
using MyBase.MyWap;
using MyUtility;
using MyLoad_Wap.Advertise;

namespace MyWap_Advertise.Adv
{
    /// <summary>
    /// Summary description for GetMSISDN
    /// </summary>
    public class GetMSISDN : MyWapBase
    {
        public override void WriteHTML()
        {
            try
            {
                LoadGetMSISDN mLoad = new LoadGetMSISDN(this);
                Write(mLoad.GetHTML());
            }
            catch (Exception ex)
            {
                mLog.Error(ex);
            }
        }
    }
}