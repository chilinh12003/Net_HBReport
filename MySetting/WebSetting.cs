using System;
using System.Collections.Generic;

using System.Text;
using MyUtility;
namespace MySetting
{
    public class WebSetting
    {
        /// <summary>
        /// Dường dẫn mặc định cho hình ảnh
        /// </summary>
        public static string DefaultImagePath = "../Images/NoImage.png";


        public static int PageSize
        {
            get
            {
                try
                {
                    if (!string.IsNullOrEmpty(MyConfig.GetKeyInConfigFile("PageSize")))
                    {
                        return int.Parse(MyConfig.GetKeyInConfigFile("PageSize"));
                    }
                    else
                    {
                        return 4;
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }


        public static string GetMSISDN()
        {
            try
            {
                if (MyCurrent.CurrentPage.Session == null || MyCurrent.CurrentPage.Session["MSISDN"] == null)
                    return string.Empty;
                else
                    return MyCurrent.CurrentPage.Session["MSISDN"].ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static bool SetMSISDN(string MSISDN)
        {
            try
            {
                if (MyCurrent.CurrentPage.Session == null)
                    return false;

                MyCurrent.CurrentPage.Session["MSISDN"] = MSISDN;
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static string ShoreCode
        {
            get
            {
                return "9696";
            }
        }
        public static int MaxPID
        {
            get
            {
                return 50;
            }
        }
    }
}
