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
    }
}
