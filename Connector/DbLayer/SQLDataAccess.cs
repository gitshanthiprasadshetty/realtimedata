using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Connector.DbLayer
{
    public class SqlDataAccess
    {
        /// <summary>
        /// Log Defination
        /// </summary>
        private static Logger.Logger Log = new Logger.Logger(typeof(SqlDataAccess));

        public static DataTable ExecuteDataTable(string sql, string conn)
        {
            Log.Info("ExecuteDataTable()");
            using (SqlConnection connection = new SqlConnection(conn))
            {
                try
                {
                    int commandTimeOut = 10000;
                    connection.Open();

                    var cmd = new SqlCommand(sql, connection);
                    cmd.CommandType = CommandType.Text;
                    if (commandTimeOut == 0)
                        cmd.CommandTimeout = 120;
                    else
                        cmd.CommandTimeout = commandTimeOut;
                    var dap = new SqlDataAdapter(cmd);
                    var dsOutput = new DataSet();
                    //Log.SQLLog(sql);
                    dap.Fill(dsOutput);

                    if (dsOutput.Tables.Count > 0)
                        return dsOutput.Tables[0];
                    else
                        return null;
                }
                catch (Exception ex)
                {
                    Log.Info("Exception , sql : " + sql);
                    Log.Error("Exception in SqlDataAccess - ExecuteDataTable : " , ex);
                }
                return null;
            }
        }

        public static void ExecuteReader(string sql, string conn)
        {
            try
            {
                List<string> values = new List<string>();
                using (SqlConnection connection = new SqlConnection(conn))
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }

        public static DataTable GetWorkQueueData(string date,string startTime,string endTime,string connectionString,string skills)
        {
            DataTable dataTable = new DataTable();
            try
            {
                Log.Info($"GetWorkQueueData()- Date: {date}, StartTime: {startTime}, EndTime: {endTime}, Skills: {skills}");
                SqlConnection conn = new SqlConnection(connectionString);
                SqlCommand cmd = new SqlCommand("SELECT AbandCalls,PassedCalls,Skill FROM [dbo].[WorkQueueData](@Date,@StartTime,@EndTime,@Id,@acceptableSL)", conn);
                cmd.Parameters.AddWithValue("@Date", date);
                cmd.Parameters.AddWithValue("@StartTime", startTime);
                cmd.Parameters.AddWithValue("@EndTime", endTime);
                cmd.Parameters.AddWithValue("@Id", skills);
                cmd.Parameters.AddWithValue("@acceptableSL", 1);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dataTable);
                return dataTable;
            }
            catch (Exception e)
            {
                Log.Error($"Exception in GetWorkQueueData(): {e}");
                return dataTable;
            }
        }
    }
}
