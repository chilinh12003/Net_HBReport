using System;
using System.Collections.Generic;
using System.Text;
using MyConnect.SQLServer;
using System.Data;
using System.Data.SqlClient;
using MyUtility;
using System.ComponentModel;

namespace MyHBReport.Adv
{
    public class Advertise
    {
        public class AdvertiseObject
        {
            public int AdvertiseID = 0;
            public string AdvertiseName = string.Empty;
            public int ServiceID = 0;
            public string ServiceName = string.Empty;
            public int PartnerID = 0;
            public string PartnerName = string.Empty;
            public string ConfirmLink = string.Empty;
            public string NotConfirmLink = string.Empty;
            public DateTime BeginDate = DateTime.MinValue;
            public DateTime EndDate = DateTime.MaxValue;
            public int MaxReg = 0;
            public Method mMethod = Method.Nothing;
            public string RedirectLink = string.Empty;
            public Status mStatus = Status.Nothing;
            public DateTime CreateDate = DateTime.MinValue;
            public DateTime UpdateDate = DateTime.MinValue;

            /// <summary>
            /// ID của đối tác tương ứng trong DB của từng dịch vụ
            /// </summary>
            public int MapPartnerID = 0;

            /// <summary>
            /// Thời gian delay trước khi chuyển sang trang Vinaphone
            /// </summary>
            public int RedirectDelay = 0;
            /// <summary>
            /// Chuối connection lấy trong table Service
            /// </summary>
            public string ConnectionName = string.Empty;
            /// <summary>
            /// ServiceID trong DB của riêng dịch vụ. lấy trong table Service
            /// </summary>
            public int MapServiceID = 0;

            /// <summary>
            /// Không cho phép redirect sang VNP
            /// </summary>
            public bool NotRedirectToVNP = false;


            public bool IsNull
            {
                get
                {
                    if (AdvertiseID == 0)
                        return true;
                    else
                        return false;
                }
            }



            public AdvertiseObject Convert(DataRow mRow)
            {
                AdvertiseObject mObj = new AdvertiseObject();

                mObj.AdvertiseID = (int)mRow["AdvertiseID"];
                mObj.AdvertiseName = mRow["AdvertiseName"].ToString();
                mObj.ServiceID = (int)mRow["ServiceID"];
                mObj.ServiceName = mRow["ServiceName"].ToString();
                mObj.PartnerID = (int)mRow["PartnerID"];
                mObj.PartnerName = mRow["PartnerName"].ToString();
                mObj.ConfirmLink = mRow["ConfirmLink"].ToString();
                mObj.NotConfirmLink = mRow["NotConfirmLink"].ToString();
                mObj.BeginDate = (DateTime)mRow["BeginDate"];
                mObj.EndDate = (DateTime)mRow["EndDate"];
                mObj.MaxReg = (int)mRow["MaxReg"];
                mObj.mMethod = (Method)(int)mRow["MethodID"];
                mObj.RedirectLink = mRow["RedirectLink"].ToString();
                mObj.mStatus = (Status)(int)mRow["StatusID"];
                mObj.CreateDate = (DateTime)mRow["CreateDate"];
                mObj.UpdateDate = mRow["UpdateDate"] != DBNull.Value ? (DateTime)mRow["UpdateDate"] : DateTime.MinValue;
                mObj.MapPartnerID = (int)mRow["MapPartnerID"];
                mObj.RedirectDelay = (int)mRow["RedirectDelay"];

                if (mRow.Table.Columns.Contains("ConnectionName"))
                {
                    mObj.ConnectionName = mRow["ConnectionName"].ToString();
                }

                if (mRow.Table.Columns.Contains("MapServiceID"))
                {
                    if (mRow["MapServiceID"] != DBNull.Value)
                        mObj.MapServiceID = (int)mRow["MapServiceID"];
                }
                return mObj;
            }
        }

        public enum Status
        {
            [DescriptionAttribute("Nothing")]
            Nothing = 0,
            [DescriptionAttribute("Kích hoạt")]
            Active = 1,
            [DescriptionAttribute("Vượt quá giới hạn")]
            MaxReg = 2,
            [DescriptionAttribute("Tạm dừng")]
            Deactive = 3,
        }
        public enum Method
        {
            [DescriptionAttribute("Nothing")]
            Nothing = 0,
            [DescriptionAttribute("Confirm bên Vinaphone")]
            VNPConfirm = 1,
            [DescriptionAttribute("Vượt Confirm Vianphone")]
            PassVNPConfirm = 2,
            [DescriptionAttribute("Vượt Confirm Vianphone với TB lần đầu SD")]
            PassVNPConfirmFirstUse = 3,
            [DescriptionAttribute("DK Confirm qua wap DV")]
            RegConfirmByWap=4,
            [DescriptionAttribute("DK Không Confirm qua wap DV")]
            RegNotConfirmByWap = 5,

        }

        MyExecuteData mExec;
        MyGetData mGet;

        public Advertise()
        {
            mExec = new MyExecuteData();
            mGet = new MyGetData();
        }

        public Advertise(string KeyConnect_InConfig)
        {
            mExec = new MyExecuteData(KeyConnect_InConfig);
            mGet = new MyGetData(KeyConnect_InConfig);
        }

        public DataSet CreateDataSet()
        {
            try
            {
                string[] mPara = { "Type" };
                string[] mValue = { "0" };
                DataSet mSet = mGet.GetDataSet("Sp_Advertise_Select", mPara, mValue);
                if (mSet != null && mSet.Tables.Count >= 1)
                {
                    mSet.DataSetName = "Parent";
                    mSet.Tables[0].TableName = "Child";
                }
                return mSet;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Type"></param>
        /// <para>Type = 2:  Lất tất cả </para>
        /// <returns></returns>
        public DataTable Select(int Type)
        {
            try
            {
                string[] mPara = { "Type" };
                string[] mValue = { Type.ToString() };

                DataTable mTable = new DataTable();
                mTable = mGet.GetDataTable("Sp_Advertise_Select", mPara, mValue);
                return mTable;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Type">Cách thức lấy
        /// <para>Type = 1: Lấy chi tiết 1 Record (Para_1 = AdvertiseID)</para>
        /// <para>Type = 3: Lấy theo StatusID (Para_1 = StatusID)</para>
        /// <para>Type = 5: Lấy chi tiết 1 record bao gồm thông tin service (Para_1 = ID)</para>
        /// </param>
        /// <param name="Para_1"></param>
        /// <returns></returns>
        public DataTable Select(int Type, string Para_1)
        {
            try
            {
                string[] mPara = { "Type", "Para_1" };
                string[] mValue = { Type.ToString(), Para_1 };

                DataTable mTable = new DataTable();
                mTable = mGet.GetDataTable("Sp_Advertise_Select", mPara, mValue);
                return mTable;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Type"></param>
        /// <para>Type = 4: Lất theo ID, StatusID va gioi han theo ngay thang (@Para_1 = AdvertiseID,@Para_2 = StatusID, @Para_3 = CurrentDate)</para>
        /// <param name="Para_1"></param>
        /// <param name="Para_2"></param>
        /// <param name="Para_3"></param>
        /// <returns></returns>
        public DataTable Select(int Type, string Para_1, string Para_2, string Para_3)
        {
            try
            {
                string[] mPara = { "Type", "Para_1", "Para_2", "Para_3" };
                string[] mValue = { Type.ToString(), Para_1, Para_2, Para_3 };
                return mGet.GetDataTable("Sp_Advertise_Select", mPara, mValue);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool Insert(int? Type, string XMLContent)
        {
            try
            {
                string[] mpara = { "Type", "XMLContent" };
                string[] mValue = { Type.ToString(), XMLContent };
                if (mExec.ExecProcedure("Sp_Advertise_Insert", mpara, mValue) > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (SqlException ex)
            {
                throw ex;
            }
        }

        public bool Delete(int? Type, string XMLContent)
        {
            try
            {
                string[] mpara = { "Type", "XMLContent" };
                string[] mValue = { Type.ToString(), XMLContent };
                if (mExec.ExecProcedure("Sp_Advertise_Delete", mpara, mValue) > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (SqlException ex)
            {
                throw ex;
            }
        }

        public bool Update(int? Type, string XMLContent)
        {
            try
            {
                string[] mpara = { "Type", "XMLContent" };
                string[] mValue = { Type.ToString(), XMLContent };
                if (mExec.ExecProcedure("Sp_Advertise_Update", mpara, mValue) > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (SqlException ex)
            {
                throw ex;
            }
        }
               

        public int TotalRow(int? Type, string SearchContent, int ServiceID, int PartnerID, int MethodID, int StatusID)
        {
            try
            {
                string[] mPara = { "Type", "SearchContent", "ServiceID", "PartnerID", "MethodID", "StatusID",  "IsTotalRow" };
                string[] mValue = { Type.ToString(), SearchContent, ServiceID.ToString(), PartnerID.ToString(), MethodID.ToString(), StatusID.ToString(),true.ToString() };

                return (int)mGet.GetExecuteScalar("Sp_Advertise_Search", mPara, mValue);
            }
            catch (SqlException ex)
            {
                throw ex;
            }
        }

        public DataTable Search(int? Type, int BeginRow, int EndRow, string SearchContent, int ServiceID, int PartnerID, int MethodID, int StatusID, string OrderBy)
        {
            try
            {
                string[] mpara = { "Type", "BeginRow", "EndRow", "SearchContent", "ServiceID", "PartnerID", "MethodID", "StatusID","OrderBy", "IsTotalRow" };
                string[] mValue = { Type.ToString(), BeginRow.ToString(), EndRow.ToString(), SearchContent, ServiceID.ToString(), PartnerID.ToString(), MethodID.ToString(), StatusID.ToString(), OrderBy, false.ToString() };
                return mGet.GetDataTable("Sp_Advertise_Search", mpara, mValue);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}