using System;
using System.Collections.Generic;
using System.Web;
using System.Web.SessionState;
using MyUtility;
namespace MyWap_Advertise.Adv
{
    /// <summary>
    /// Summary description for AjaxLog
    /// </summary>
    public class AjaxLog : IHttpHandler, IRequiresSessionState
    {

        public void ProcessRequest(HttpContext context)
        {
            try
            {
                string Para = context.Request.QueryString["para"];
                if(string.IsNullOrEmpty(Para))
                {
                    MyLogfile.WriteLogData("_Script", "Para:null");
                }
                else
                    MyLogfile.WriteLogData("_Script", "Para:"+Para);

                context.Response.ContentType = "text/plain";
                context.Response.Write("Hello World");
            }
            catch(Exception ex)
            {
                MyLogfile.WriteLogError(ex);
            }
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}