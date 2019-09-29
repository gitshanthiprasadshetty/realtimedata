using BcmsSIPManager.Utilites;
using SIPDataCollector.Utilites;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BcmsSIPManager.Utilities
{
    public class SqlDataAccess
    {
        /// <summary>
        /// log Defination
        /// </summary>
        private static Logger.Logger log = new Logger.Logger(typeof(SqlDataAccess));

        /// <summary>
        /// Standard sql method to execute datatable.
        /// </summary>
        /// <param name="sql">SQL Query</param>
        /// <returns>Returns dataTable</returns>
        public static DataTable ExecuteDataTable(string sql)
        {
            log.Debug("ExecuteDataTable()");
            using (SqlConnection connection = new SqlConnection(ConfigurationData.ConntnString))
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
                    dap.Fill(dsOutput);

                    if (dsOutput.Tables.Count > 0)
                        return dsOutput.Tables[0];
                    else
                        return null;
                }
                catch (Exception ex)
                {
                    log.Error("Exception in ExecuteDataTable : " , ex);
                }
                return null;
            }
        }
    }
}
