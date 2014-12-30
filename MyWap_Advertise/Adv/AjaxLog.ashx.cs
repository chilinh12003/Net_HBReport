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

        MyLog mLog = new MyLog(typeof(AjaxLog));
        public void ProcessRequest(HttpContext context)
        {
            try
            {
                string Para = context.Request.QueryString["para"];
                if(string.IsNullOrEmpty(Para))
                {
                    mLog.Debug( "Para:null");
                }
                else
                    mLog.Debug( "Para:"+Para);

                context.Response.ContentType = "text/plain";
                context.Response.Write("Hello World");
            }
            catch(Exception ex)
            {
                mLog .Error(ex);
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