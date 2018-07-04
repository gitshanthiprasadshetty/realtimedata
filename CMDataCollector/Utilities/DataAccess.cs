using CMDataCollector.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMDataCollector.Utilities
{
    class DataAccess
    {
        /// <summary>
        /// Log Defination
        /// </summary>
        private static Logger.Logger Log = new Logger.Logger(typeof(DataAccess));

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public static int ExecuteQuery(string sql)
        {
            Log.Debug("DataAccess[ExecuteQuery]");
            try
            {
                Log.Debug("SQL" + sql);
                using (SqlConnection conn = new SqlConnection(ConfigurationData.ConntnString))
                {
                    conn.Open();
                    SqlCommand comm = new SqlCommand(sql, conn);
                    Log.Debug("Return value " + comm.ExecuteReader());
                    Convert.ToInt32(comm.ExecuteReader().GetValue(0));
                }
            }
            catch (Exception ex)
            {
                Log.Error("Exception in ExecuteQuery():" + sql, ex);
            }
            return 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Extn"></param>
        /// <returns></returns>
        public static int GetAbnData(string Extn)
        {
            Log.Debug("DataAccess[GetAbnData]");
            try
            {
                string sql = @"select Count(1) as COUNT from dbo.TMAC_WorkQueueHistory 
                               where channel='voice' and Reason='ABN' and skill='" + Extn + "' and CreateDate= '" + DateTime.Now.Date.ToString("yyyyMMdd") + "'";
                DataTable dsusers = SqlDataAccess.ExecuteDataTable(sql);
                if (dsusers != null)
                {
                    if (dsusers.Rows.Count > 0)
                    {
                        return Convert.ToInt32(dsusers.Rows[0][0]);
                    }
                    else return 0;
                        //foreach (DataRow item in dsusers.Rows)
                        //{
                        //    return Convert.ToInt32(item.ItemArray[0]);
                        //}
                    }
            }
            catch (Exception ex)
            {
                Log.Error("Exception in GetAbnData():" + ex);
            }
            return 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Extn"></param>
        /// <returns></returns>
        public static int GetACDData(string Extn)
        {
            Log.Debug("DataAccess[GetAbnData]");
            try
            {
                string sql = @"select Count(1) as COUNT from dbo.TMAC_WorkQueueHistory 
                               where Channel='voice' AND SubChannel='in' AND Reason NOT IN ('ABN','abandon') 
                               and skill='" + Extn + "' and CreateDate= '" + DateTime.Now.Date.ToString("yyyyMMdd") + "'";
                DataTable dsusers = SqlDataAccess.ExecuteDataTable(sql);
                if (dsusers != null)
                {
                    if (dsusers.Rows.Count > 0)
                    {
                        return Convert.ToInt32(dsusers.Rows[0][0]);
                    }
                    else return 0;
                    //foreach (DataRow item in dsusers.Rows)
                    //{
                    //    return Convert.ToInt32(item.ItemArray[0]);
                    //}
                }
            }
            catch (Exception ex)
            {
                Log.Error("Exception in GetAbnData():" + ex);
            }
            return 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static DataTable GetSkillExtnInfo(string skillsToMonitor)
        {
            try
            {
                string sql = @"select SkillID,SkillExtension,SkillName from TMAC_Skills Where SkillID in (" + skillsToMonitor + ")";
                DataTable skillExtnObj = SqlDataAccess.ExecuteDataTable(sql);
                return skillExtnObj;
            }
            catch (Exception ex)
            {
                Log.Error("Exception in ExtnToSkillMap():");
                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static DataTable GetAgentSkillInfo()
        {
            try
            {
                string sql = @"SELECT AgentID , STUFF(( SELECT  ','+ SkillID FROM TMAC_Agent_Skills a
                                WHERE b.AgentID = a.AgentID FOR XML PATH('')),1 ,1, '')  Skills
                                FROM TMAC_Agent_Skills b
                                GROUP BY AgentID;";
                DataTable agentSkillObj = SqlDataAccess.ExecuteDataTable(sql);
                return agentSkillObj;
            }
            catch (Exception ex)
            {
                Log.Error("Exception in ExtnToSkillMap():");
                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        //public static ExtnSkillMap ExtnToSkillMap()
        //{
        //    try
        //    {
        //        string sql = @"select Count(1) as COUNT from dbo.TMAC_WorkQueueHistory 
        //                       where Channel='voice' AND SubChannel='in' AND Reason NOT IN ('ABN','abandon') and skill='" + Extn + "' and CreateDate= '" + DateTime.Now.Date.ToString("yyyyMMdd") + "'";
        //        DataTable dsusers = SqlDataAccess.ExecuteDataTable(sql);
        //    }
        //    catch (Exception ex)
        //    {
        //        Log.Error("Exception in ExtnToSkillMap():");
        //        return null;
        //    }
        //}
    }
}
