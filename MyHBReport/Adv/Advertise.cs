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

            /// <summary>
            /// Tránh tình trang KH đã hủy nhưng lại bị DK tiếp thì MaxRequest sẽ quy định được request bao nhiêu lần
            /// </summary>
            public int MaxRequest = 0;

            /// <summary>
            /// % sẽ pass sang 1 link khác.
            /// </summary>
            public int PassPercent = 0;
            public string PassLink = string.Empty;
            public string UsedLink = string.Empty;

            /// <summary>
            /// Link để ghi log cho các dịch vụ là của đối tác 
            /// </summary>
            public string LogMSISDNLink = string.Empty;


            public string GetUsedLink
            {
                get
                {
                    if (!string.IsNullOrEmpty(UsedLink))
                        return UsedLink;
                    else if (!string.IsNullOrEmpty(NotConfirmLink))
                        return NotConfirmLink;
                    else if (!string.IsNullOrEmpty(ConfirmLink))
                        return ConfirmLink;
                    else
                        return RedirectLink;
                }
            }

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

            /// <summary>
            /// Lấy link cần redirect. nếu ko có link Redirect thì sẽ lấy confirmLink
            /// </summary>
            public string GetRedirectLink
            {
                get
                {
                    if (!string.IsNullOrEmpty(RedirectLink))
                        return RedirectLink;
                    else if (!string.IsNullOrEmpty(NotConfirmLink))
                        return NotConfirmLink;
                    else if (!string.IsNullOrEmpty(ConfirmLink))
                        return ConfirmLink;
                    else
                        return UsedLink;
                }
            }

            /// <summary>
            /// Kiem tra xem co duoc pass du lieu hay khong
            /// </summary>
            /// <param name="RegCount"></param>
            /// <returns></returns>
            public bool CheckPass(int RegCount)
            {
                if (PassPercent <= 0)
                    return false;

                int RegMod = RegCount % 10;

                switch(PassPercent)
                {
                    case 1:
                        if (RegMod == 4)
                            return true;
                        break;
                    case 2:
                        if (RegMod == 5 || RegMod == 8)
                            return true;
                        break;
                    case 3:
                        if (RegMod == 4 || RegMod == 6 || RegMod == 9)
                            return true;
                        break;
                    case 4:
                        if (RegMod == 2 || RegMod == 4 || RegMod == 6 || RegMod == 9)
                            return true;
                        break;
                    case 5:
                        if (RegMod == 2 || RegMod == 4 || RegMod == 5 || RegMod == 7 || RegMod == 9)
                            return true;
                        break;

                }
                return false;
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

                mObj.MaxRequest = (int)mRow["MaxRequest"];
                mObj.PassPercent = (int)mRow["PassPercent"];

                mObj.PassLink = mRow["PassLink"].ToString(); 
                mObj.UsedLink = mRow["UsedLink"].ToString();
                mObj.LogMSISDNLink = mRow["LogMSISDNLink"].ToString();
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

            /// <summary>
            /// Update xuống wapreglog
            /// </summary>
            /// <param name="MSISDN"></param>
            /// <param name="Note"></param>
            /// <returns></returns>
            public bool InsertWapRegLog(string MSISDN, string Note)
            {
                try
                {
                    WapRegLog mWapRegLog = new WapRegLog(this.ConnectionName);

                    DataSet mSet = mWapRegLog.CreateDataSet();
                    MyConvert.ConvertDateColumnToStringColumn(ref mSet);
                    DataRow mRow = mSet.Tables[0].NewRow();
                    mRow["MSISDN"] = MSISDN;
                    mRow["PID"] = 0;
                    mRow["PartnerID"] = this.MapPartnerID;
                    mRow["CreateDate"] = DateTime.Now.ToString(MyConfig.DateFormat_InsertToDB);
                    mRow["StatusID"] = (int)WapRegLog.Status.NewCreate;
                    mRow["Note"] = Note;
                    mSet.Tables[0].Rows.Add(mRow);

                    return mWapRegLog.Insert(0, mSet.GetXml());
                }
                catch(Exception ex)
                {
                    throw ex;
                }
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
            [DescriptionAttribute("Confirm qua wap dịch vụ")]
            RegConfirmByWap=4,
            [DescriptionAttribute("Không Confirm qua wap dịch vụ")]
            RegNotConfirmByWap = 5,
            [DescriptionAttribute("Vượt Confirm với DV của đối tác")]
            PassVNPConfirm_Party = 6,
            [DescriptionAttribute("Confirm bên Vinaphone với DV của đối tác")]
            VNPConfirm_Party = 7,
            [DescriptionAttribute("Không Confirm qua wap DV Với Số ĐT 11 Số")]
            RegNotConfirmByWap_11 = 8,

            [DescriptionAttribute("Không Confirm qua wap DV với TB lần đầu SD")]
            RegNotConfirmByWapFirstUse = 9,

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