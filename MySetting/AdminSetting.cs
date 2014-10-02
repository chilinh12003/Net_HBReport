using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using MyUtility;
namespace MySetting
{
    public class AdminSetting
    {

        public enum ListPage
        {
            [DescriptionAttribute("Quản trị thể loại")]
            Categories,
            [DescriptionAttribute("Quản trị Menu")]
            MenuAdmin,
            [DescriptionAttribute("Cấu hình hệ thống")]
            SystemConfig,
            [DescriptionAttribute("Nhóm thành viên")]
            MemberGroup,
            [DescriptionAttribute("Quản trị tài khoản")]
            Member,
            [DescriptionAttribute("Phần quyền")]
            Permission,
            [DescriptionAttribute("Log thành viên")]
            MemberLog,
            [DescriptionAttribute("Đổi mật khẩu")]
            ChangePass,
            [DescriptionAttribute("Quản trị Đối tác")]
            Partner,
            [DescriptionAttribute("Quản trị Thể loại")]
            Category,


            [DescriptionAttribute("Lịch sử MO/MT của thuê bao")]
            History_MO_MT,

            [DescriptionAttribute("Gửi lại MT cho khách hàng")]
            ResendMT,

            [DescriptionAttribute("Thống kê số lượng MO theo ngày")]
            Report_MOByDay,

            [DescriptionAttribute("Lịch sử MO/MT của thuê bao")]
            Report_MOByPhoneNumber,

            //MTRAFFIC

            [DescriptionAttribute("Thống kê sản lượng theo giá")]
            MTraffic_RPPartnerPrice,
            [DescriptionAttribute("Thống kê sản lượng theo ngày")]
            MTraffic_RPPartnerDay,
            [DescriptionAttribute("Thống kê thuê bao")]
            MTraffic_RPPartnerSub,

            [DescriptionAttribute("Thống kê thuê bao")]
            RP_Sub,

            [DescriptionAttribute("Thống kê thuê bao MTraffic")]
            RP_Sub_MTraffic,

            [DescriptionAttribute("Thống kê thuê bao Triệu phú thể thao")]
            RP_Sub_SportMillion,

            //Quảng cáo
            [DescriptionAttribute("Dịch vụ")]
            Adv_Service,
            [DescriptionAttribute("Quản trị quảng cáo")]
            Adv_Advertise,

        }

        public struct ParaSave
        {
            /// <summary>
            /// Lưu trữ thông tin Serivice vào session
            /// </summary>
            public static string Service = "Service";
            public static string Partner = "Partner";

        }

        public static string MySQLConnection_Gateway
        {
            get
            {
                return "MySQLConnection_Gateway";
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

        /// <summary>
        /// Key dùng để mã hóa tạo chữ ký khi call WS đăng ký dịch vụ
        /// </summary>
        public static string RegWSKey = "wre34WD45F";

        /// <summary>
        /// Key dùng để mã hóa dữ liệu nhạy cảm
        /// </summary>
        public static string SpecialKey = "ChIlINh154";

        public static string AllowIPFile
        {
            get
            {
                string Temp = MyConfig.GetKeyInConfigFile("AllowIPFile");
                if (string.IsNullOrEmpty(Temp))
                    return @"~/App_Data/AllowIP.xml";
                else
                    return Temp;
            }
        }

        /// <summary>
        /// Tắt chức năng kiểm tra IP
        /// </summary>
        public static bool DisableCheckIP
        {
            get
            {
                string Temp = MyConfig.GetKeyInConfigFile("DisableCheckIP");
                if (string.IsNullOrEmpty(Temp))
                {

                    return false;
                }
                else
                {
                    Temp = Temp.Trim();
                    bool bValue = false;
                    bool.TryParse(Temp,out bValue);
                    return bValue;
                }
            }
        }
    }
}
