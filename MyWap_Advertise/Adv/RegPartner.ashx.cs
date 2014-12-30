using System;
using System.Collections.Generic;
using System.Web;
using MyBase.MyWap;
using MyUtility;
using MyLoad_Wap.Advertise;

namespace MyWap_Advertise.Adv
{
    /// <summary>
    /// Đăng ký cho các dịch vụ mà đối tác xây dựng
    /// </summary>
    public class RegPartner : MyWapBase
    {
        public override void WriteHTML()
        {
            try
            {
                LoadRegPartner mLoad = new LoadRegPartner(this);
                Write(mLoad.GetHTML());
            }
            catch (Exception ex)
            {
                mLog.Error(ex);
            }
        }
    }
}