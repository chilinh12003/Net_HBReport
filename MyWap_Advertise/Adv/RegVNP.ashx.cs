using System;
using System.Collections.Generic;
using System.Web;
using MyBase.MyWap;
using MyUtility;
using MyLoad_Wap.Advertise;

namespace MyWap_Advertise.Adv
{
    /// <summary>
    /// Summary description for RegVNP
    /// </summary>
    public class RegVNP : MyWapBase
    {
        public override void WriteHTML()
        {
            try
            {
                LoadRegVNP mLoad = new LoadRegVNP(this);
                Write(mLoad.GetHTML());
            }
            catch (Exception ex)
            {
                mLog.Error(ex);
            }
        }
    }
}