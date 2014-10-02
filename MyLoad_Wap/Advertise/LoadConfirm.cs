using System;
using System.Collections.Generic;
using System.Text;
using MyBase.MyLoad;

namespace MyLoad_Wap.Advertise
{
    public class LoadConfirm:MyLoadBase
    {
        public LoadConfirm()
        {
            mTemplatePath = "~/Templates/Static/Register_Confirm.html";
            Init();
        }       
        // Hàm trả về chuỗi có chứa mã HTML
        protected override string BuildHTML()
        {
            try
            {
                string[] arr = { };
                return mLoadTempLate.LoadTemplateByArray(mTemplatePath, arr);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
