using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMDataCollector.Utilities
{
    public class SqlDataAccess
    {
        /// <summary>
        /// Log Defination
        /// </summary>
        private static Logger.Logger Log = new Logger.Logger(typeof(SqlDataAccess));

        public static DataTable ExecuteDataTable(string sql)
        {
            using (SqlConnection connection = new SqlConnection(ConfigurationData.CMConntnString))
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
                   // Log.SQLLog("Exception : " + ex);
                    Log.Error("Exception in SqlDataAccess - ExecuteDataTable : " + ex);
                }
                return null;
            }
        }
    }
}
