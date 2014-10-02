using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using MyConnect.MySQL;
using MyUtility;
using System.Collections;
using System.ComponentModel;


namespace MyHBReport.ShortCode_6X83
{
    public class MOMTLog
    {
        MySQLExecuteData mExec;
        MySQLGetData mGet;
        public MOMTLog()
        {
            mExec = new MySQLExecuteData();
            mGet = new MySQLGetData();
        }

        public MOMTLog(string KeyConnect_InConfig)
        {
            mExec = new MySQLExecuteData(KeyConnect_InConfig);
            mGet = new MySQLGetData(KeyConnect_InConfig);
        }

        public DataTable Select(DateTime ReportDate, string MSISDN, string OrderBy)
        {
            try
            {
                if (string.IsNullOrEmpty(OrderBy))
                    OrderBy = "ReceiveDate DESC ";
                DataTable mTable = new DataTable();

                string Format = "   SELECT 	@TableName_MO.User_ID AS MSISDN,@TableName_MO.Service_ID AS ShortCode, " +
                                    "        @TableName_MO.Command_Code AS Keyword,@TableName_MO.Info AS MO,@TableName_MO.Receive_Date AS ReceiveDate, " +
                                    "        @TableName_MT.Info AS MT,@TableName_MT.Done_Date AS SendDate " +
                                    "FROM 	@TableName_MO INNER JOIN @TableName_MT ON  " +
                                    "            @TableName_MO.Request_ID = @TableName_MT.Request_ID " +
                                    "WHERE @TableName_MO.User_ID = @MSISDN " +
                                    "ORDER BY  " + OrderBy;
                string TableName_MO = "sms_receive_log" + ReportDate.ToString("yyyyMM");
                string TableName_MT = "ems_send_log" + ReportDate.ToString("yyyyMM");
                Format = Format.Replace("@TableName_MO", TableName_MO).Replace("@TableName_MT", TableName_MT);
                string[] arr_para = {  "@MSISDN" };
                string[] arr_value = {  MSISDN };
                mTable = mGet.GetDataTableByQuery(Format, arr_para, arr_value);

                return mTable;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public int MOByPhoneNumber_RowCount(DateTime FromDate, DateTime ToDate)
        {
            try
            {
                string TableName_MO = "sms_receive_log" + FromDate.ToString("yyyyMM");

                string Query = "SELECT COUNT(1) FROM (" +
                                "   SELECT 	@TableName_MO.User_ID AS MSISDN " +
                                "   FROM 	@TableName_MO " +
                                "   WHERE   @TableName_MO.command_code IN ('GAME','CLIP','IMG','NH') AND @TableName_MO.Receive_Date BETWEEN @FromDate AND @ToDate ) AS TEMP ";

                Query = Query.Replace("@TableName_MO", TableName_MO);

                string[] arr_para = {  "@FromDate", "@ToDate" };
                string[] arr_value = { FromDate.ToString(MyConfig.DateFormat_InsertToDB), ToDate.ToString(MyConfig.DateFormat_InsertToDB) };

                DataTable mTable = mGet.GetDataTableByQuery(Query, arr_para, arr_value);
                return int.Parse(mTable.Rows[0][0].ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable MOByPhoneNumber_Data(int BeginRow, int PageSize, DateTime FromDate, DateTime ToDate,  string OrderBy)
        {
            try
            {
                if (BeginRow > 0)
                    BeginRow = BeginRow - 1;

                if (string.IsNullOrEmpty(OrderBy))
                    OrderBy = "ReceiveDate DESC ";
                DataTable mTable = new DataTable();

                string Format = "   SELECT 	@TableName_MO.User_ID AS MSISDN,@TableName_MO.Service_ID AS ShortCode, " +
                                    "       @TableName_MO.Command_Code AS Keyword,@TableName_MO.Info AS MO,@TableName_MO.Receive_Date AS ReceiveDate " +
                                    "FROM 	@TableName_MO " +
                                    "WHERE @TableName_MO.command_code IN ('GAME','CLIP','IMG','NH')  AND @TableName_MO.Receive_Date BETWEEN @FromDate AND @ToDate " +
                                    
                                    "ORDER BY  " + OrderBy +" LIMIT " + BeginRow.ToString() + "," + PageSize .ToString()+ " ";;

                string TableName_MO = "sms_receive_log" + FromDate.ToString("yyyyMM");

                Format = Format.Replace("@TableName_MO", TableName_MO);

                string[] arr_para = {  "@FromDate", "@ToDate" };
                string[] arr_value = { FromDate.ToString(MyConfig.DateFormat_InsertToDB), ToDate.ToString(MyConfig.DateFormat_InsertToDB) };
                
                mTable = mGet.GetDataTableByQuery(Format, arr_para, arr_value);

                return mTable;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable MOByday_Data(int BeginRow, int PageSize, DateTime FromDate, DateTime ToDate)
        {
            try
            {
                if (BeginRow > 0)
                    BeginRow = BeginRow - 1;

                string TableName_MO = "sms_receive_log" + FromDate.ToString("yyyyMM");
                string Query = "SELECT Service_ID,Command_code, DATE_FORMAT(Receive_Date,'%d-%m-%Y') AS ReportDay, COUNT(1) AS MOCount " +
                                "FROM @TableName_MO " +
                                "WHERE command_code IN ('GAME','CLIP','IMG','NH') AND " +
                                "Receive_Date BETWEEN @FromDate AND @ToDate "+
                                "GROUP BY Service_ID,Command_code, DATE_FORMAT(Receive_Date,'%d-%m-%Y') " +
                                "ORDER BY Receive_Date DESC LIMIT " + BeginRow.ToString() + "," + PageSize .ToString()+ " ";
                Query = Query.Replace("@TableName_MO", TableName_MO);
                string[] arr_para = { "@FromDate", "@ToDate" };
                string[] arr_value = { FromDate.ToString(MyConfig.DateFormat_InsertToDB), ToDate.ToString(MyConfig.DateFormat_InsertToDB)};
                DataTable mTable = mGet.GetDataTableByQuery(Query, arr_para, arr_value);
                return mTable;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public int MOByday_RowCount(DateTime FromDate, DateTime ToDate)
        {
            try
            {
                string TableName_MO = "sms_receive_log" + FromDate.ToString("yyyyMM");
                string Query = "SELECT COUNT(1) FROM ("+
                                "SELECT Service_ID,Command_code, DATE_FORMAT(Receive_Date,'%d-%m-%Y') AS ReportDay, COUNT(1) " +
                                "FROM @TableName_MO " +
                                "WHERE command_code IN ('GAME','CLIP','IMG','NH') AND " +
                                "Receive_Date BETWEEN @FromDate AND @ToDate " +
                                "GROUP BY Service_ID,Command_code, DATE_FORMAT(Receive_Date,'%d-%m-%Y') " +
                                "ORDER BY Receive_Date DESC ) AS TEMP";
                Query = Query.Replace("@TableName_MO", TableName_MO);
                string[] arr_para = {  "@FromDate", "@ToDate" };
                string[] arr_value = { FromDate.ToString(MyConfig.DateFormat_InsertToDB), ToDate.ToString(MyConfig.DateFormat_InsertToDB) };
                DataTable mTable = mGet.GetDataTableByQuery(Query, arr_para, arr_value);
                return int.Parse(mTable.Rows[0][0].ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


    }
}
