using System;
using System.Collections.Generic;
using System.Text;
using MyBase.MyLoad;
using MyUtility;

namespace MyLoad_Wap.Advertise
{
    public class LoadNotConfirm :MyLoadBase
    {
        public string ConfirmLink = string.Empty;
        public string NotConfirmLink = string.Empty;
        public int RedirectDelay = 0;
        public string ReloadLink = string.Empty;

        public LoadNotConfirm(string ConfirmLink, string NotConfirmLink, int RedirectDelay, string ReloadLink)
        {
            this.ConfirmLink = ConfirmLink;
            this.NotConfirmLink = NotConfirmLink;
            this.RedirectDelay = RedirectDelay;
            this.ReloadLink = ReloadLink;
            mTemplatePath = "~/Templates/Static/Register_NotConfirm.html";
            Init();
        }       
        // Hàm trả về chuỗi có chứa mã HTML
        protected override string BuildHTML()
        {
            try
            {
                string HTML = MyFile.ReadFile(MyFile.GetFullPathFile(mTemplatePath));
                HTML = HTML.Replace("{DNS}", MyConfig.Domain);
                HTML = HTML.Replace("{ConfirmLink}", ConfirmLink);
                HTML = HTML.Replace("{NotConfirmLink}", NotConfirmLink);
                HTML = HTML.Replace("{RedirectDelay}", RedirectDelay.ToString());
                HTML = HTML.Replace("{ReloadLink}", ReloadLink);
                return HTML;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
