using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using SIPDataCollector.Utilites;
using Connector.DbLayer;
using SIPDataCollector.Models;

namespace SIPDataCollector.Utilities
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
        /// <param name="sql">SQL query to run</param>
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
        /// Gets Total abundon calls for given date.
        /// </summary>
        /// <param name="Extn">Group Extension Id</param>
        /// <returns>Total abundon calls for given extensionid and date</returns>
        public static int GetAbnData(string Extn, string channel)
        {
            Log.Debug("DataAccess[GetAbnData]");
            try
            {
                //string sql = @"select Count(1) as COUNT from dbo.TMAC_WorkQueueHistory 
                //               where channel='voice' and Reason='ABN' and skill='" + Extn + "' and CreateDate= '" + DateTime.Now.Date.ToString("yyyyMMdd") + "'";

                string sql = @"EXEC [GET_TotalAbandonedInteractions] '" + Extn + "'" + ',' + "'" + channel + "'" + ',' + "'" + DateTime.Now.Date.ToString("yyyyMMdd") + "'";
                DataTable dsusers = SqlDataAccess.ExecuteDataTable(sql, ConfigurationData.ConntnString);
                if (dsusers != null)
                {
                    if (dsusers.Rows.Count > 0)
                    {
                        return Convert.ToInt32(dsusers.Rows[0][0]);
                    }
                    else return 0;
                }
            }
            catch (Exception ex)
            {
                Log.Error("Exception in GetAbnData():" + ex);
            }
            return 0;
        }

        public static SkillData GetHistoricalData(string skillExtn, string skillId)
        {
            Log.Debug("GetHistoricalData() : " + skillExtn);
            try
            {
                //string sql = @"select Count(1) as COUNT from dbo.TMAC_WorkQueueHistory 
                //               where channel='voice' and Reason='ABN' and skill='" + Extn + "' and CreateDate= '" + DateTime.Now.Date.ToString("yyyyMMdd") + "'";
                SkillData skillInfo;
                string startTime = "000000";
                string endTime = DateTime.Now.TimeOfDay.ToString("hhmmss");
                string type = "skill";
                int accSlLevl;
                try
                {
                    accSlLevl = ConfigurationData.acceptableSlObj.FirstOrDefault(x => x.Key == skillId).Value;
                }
                catch (Exception ex)
                {
                    Log.Error("Error while reading acceptable sl level : ", ex);
                    accSlLevl = ConfigurationData.acceptableSL;
                }
                string sql = @"EXEC [GET_HistoricalData] '" + DateTime.Now.Date.ToString("yyyyMMdd") + "'" + ',' + "'" + startTime + "'" + ',' + "'" + endTime + "'" + ',' + "'" + skillExtn + "'" + ',' + "'" + type + "'" + ',' + "'" + accSlLevl + "'";
                Log.Debug("SQL Query : " + sql);
                DataTable dsusers = SqlDataAccess.ExecuteDataTable(sql, ConfigurationData.ConntnString);
                if (dsusers != null)
                {
                    foreach(DataRow item in dsusers.Rows)
                    {
                        skillInfo = new SkillData();
                        skillInfo.ACWTime = Convert.ToInt32(item["TotalACWTime"]);
                        skillInfo.SLPercentage = Convert.ToDecimal(item["SLPercentage"]);
                        skillInfo.ACDTime = Convert.ToInt32(item["TotalACDTime"]);
                        skillInfo.AbandCalls = Convert.ToInt32(item["AbandCalls"]);
                        skillInfo.TotalACDInteractions = Convert.ToInt32(item["TotalInteraction"]);
                        return skillInfo;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error("Exception in GetAbnData():" + ex);
            }
            return new SkillData();
        }

        /// <summary>
        /// Gets total summary of acd calls for given extensionid
        /// </summary>
        /// <param name="Extn">Group Extension Id</param>
        /// <returns>Summary of ACD calls</returns>
        public static int GetACDData(string Extn, string channel)
        {
            Log.Debug("DataAccess[GetACDData]");
            try
            {
                //string sql = @"select Count(1) as COUNT from dbo.TMAC_WorkQueueHistory 
                //               where Channel='voice' AND SubChannel='in' AND Reason NOT IN ('ABN','abandon') 
                //               and skill='" + Extn + "' and CreateDate= '" + DateTime.Now.Date.ToString("yyyyMMdd") + "'";
                string sql = @"EXEC [GET_TotalAcdInteractions] '" + Extn + "'" + ',' + "'" + channel + "'" + ',' + "'" + DateTime.Now.Date.ToString("yyyyMMdd") + "'";
                DataTable dsusers = SqlDataAccess.ExecuteDataTable(sql, ConfigurationData.ConntnString);
                if (dsusers != null)
                {
                    if (dsusers.Rows.Count > 0)
                    {
                        return Convert.ToInt32(dsusers.Rows[0][0]);
                    }
                    else return 0;           
                }
            }
            catch (Exception ex)
            {
                Log.Error("Exception in GetACDData():" + ex);
            }
            return 0;
        }

        /// <summary>
        /// Gets Skill-Extension related information.
        /// </summary>
        /// <returns>returns DataTable</returns>
        public static DataTable GetSkillExtnInfo(string skillsToMonitor)
        {
            Log.Debug("GetSkillExtnInfo()");
            try
            {
                string sql = @"select SkillID,SkillExtension,SkillName from TMAC_Skills Where SkillID in (" + skillsToMonitor + ")";
                DataTable skillExtnObj = SqlDataAccess.ExecuteDataTable(sql, ConfigurationData.ConntnString);
                return skillExtnObj;
            }
            catch (Exception ex)
            {
                Log.Error("Exception in ExtnToSkillMap():");
                return null;
            }
        }

        /// <summary>
        /// Gets Agent-{List of Skills} data
        /// </summary>
        /// <returns>returns dataTable having agentid and total skills he has.</returns>
        public static DataTable GetAgentSkillInfo()
        {
            Log.Debug("GetAgentSkillInfo()");
            try
            {
                string sql = @"SELECT AgentID , STUFF(( SELECT  ','+ SkillID FROM TMAC_Agent_Skills a
                                WHERE b.AgentID = a.AgentID FOR XML PATH('')),1 ,1, '')  Skills
                                FROM TMAC_Agent_Skills b
                                GROUP BY AgentID;";
                DataTable agentSkillObj = SqlDataAccess.ExecuteDataTable(sql, ConfigurationData.ConntnString);
                return agentSkillObj;
            }
            catch (Exception ex)
            {
                Log.Error("Exception in ExtnToSkillMap():");
                return null;
            }
        }

        public static List<string> GetAuxCodes()
        {
            try
            {
                string[] auxCodes = new string[] { };
                string sql = @"select code from [dbo].[AGT_AUX_Codes]";
                DataTable dataTable = SqlDataAccess.ExecuteDataTable(sql, ConfigurationData.ConntnString);
                if(dataTable != null)
                {
                        auxCodes = dataTable
                        .AsEnumerable()
                        .Select(row => row.Field<string>("code"))
                        .ToArray();
                }
                return auxCodes.ToList();
            }
            catch (Exception ex)
            {
            }
            return null;
        }

        public static Dictionary<string, int> GetAcceptableLevels()
        {
            try
            {
                string[] auxCodes = new string[] { };
                string sql = @"select distinct SkillID, AcceptableServiceLevel from TMAC_Skills";
                DataTable dataTable = SqlDataAccess.ExecuteDataTable(sql, ConfigurationData.ConntnString);
                if (dataTable != null)
                {
                    Dictionary<string, int> result = new Dictionary<string, int>();
                    foreach (DataRow item in dataTable.Rows)
                    {
                        string skillId = Convert.ToString(item["SkillID"]);
                        int accSlLevl = string.IsNullOrEmpty(item["AcceptableServiceLevel"].ToString()) ? ConfigurationData.acceptableSL : Convert.ToInt32(item["AcceptableServiceLevel"]);
                        result.Add(skillId, accSlLevl);
                    }
                    return result;
                }
            }
            catch (Exception ex)
            {
                Log.Error("Error in GetAcceptableLevels : " + ex);
            }
            return null;
        }

        #region for bcms summary data

        public void SummaryData(BcmsDataForSIP summarydata)
        {
            try
            {

            }
            catch (Exception ex)
            {
            }
        }

        #endregion

        #region Not-Used
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
        #endregion
    }
}
