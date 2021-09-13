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
        /// log Defination
        /// </summary>
        private static Logger.Logger log = new Logger.Logger(typeof(DataAccess));

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sql">SQL query to run</param>
        /// <returns></returns>
        public static int ExecuteQuery(string sql)
        {
            log.Debug("ExecuteQuery()");
            try
            {
                log.Debug("SQL" + sql);
                using (SqlConnection conn = new SqlConnection(ConfigurationData.ConntnString))
                {
                    conn.Open();
                    SqlCommand comm = new SqlCommand(sql, conn);
                    log.Debug("Return value " + comm.ExecuteReader());
                    Convert.ToInt32(comm.ExecuteReader().GetValue(0));
                }
            }
            catch (Exception ex)
            {
                log.Error("Exception in ExecuteQuery():" + sql, ex);
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
            log.Debug("DataAccess[GetAbnData]");
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
                log.Error("Exception in GetAbnData():" + ex);
            }
            return 0;
        }

        public static SkillData GetHistoricalData(string skillExtn, int skillId)
        {
            log.Info($"GetHistoricalData() : skillExtn = {skillExtn} , skillid = {skillId}");
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
                    accSlLevl = ConfigurationData.acceptableSlObj.FirstOrDefault(x => x.Key == skillId.ToString()).Value;
                }
                catch (Exception ex)
                {
                    log.Error("Error while reading acceptable sl level : ", ex);
                    accSlLevl = ConfigurationData.acceptableSL;
                }
                string sql = @"EXEC [GET_HistoricalData] '" + DateTime.Now.Date.ToString("yyyyMMdd") + "'" + ',' + "'" + startTime + "'" + ',' + "'" + endTime + "'" + ',' + "'" + skillExtn + "'" + ',' + "'" + type + "'" + ',' + "'" + accSlLevl + "'";
                log.Debug("SQL Query : " + sql);
                DataTable dsusers = SqlDataAccess.ExecuteDataTable(sql, ConfigurationData.ConntnString);
                if (dsusers != null)
                {
                    foreach(DataRow item in dsusers.Rows)
                    {
                        skillInfo = new SkillData
                        {
                            ACWTime = Convert.ToInt32(item["TotalAfterCallTime"]),
                            SLPercentage = Convert.ToDecimal(item["SLPercentage"]),
                            ACDTime = Convert.ToInt32(item["TotalACDTime"]),
                            AbandCalls = Convert.ToInt32(item["AbandCalls"]),
                            TotalACDInteractions = Convert.ToInt32(item["TotalInteraction"]),
                            AvgAbandTime = Convert.ToInt32(item["AvgAbandTime"]),
                            AHTTime = Convert.ToInt32(item["HoldTime"]),
                            TotalCallsHandled = Convert.ToInt32(item["TotalInteraction"])
                        };
                        return skillInfo;
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error("Exception in GetHistoricalData():" , ex);
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
            log.Debug("DataAccess[GetACDData]");
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
                log.Error("Exception in GetACDData():" + ex);
            }
            return 0;
        }

        /// <summary>
        /// Gets Skill-Extension related information.
        /// </summary>
        /// <returns>returns DataTable</returns>
        public static DataTable GetSkillExtnInfo(string skillsToMonitor)
        {
            log.Debug("GetSkillExtnInfo()");
            try
            {
                string sql = "";
                sql = string.IsNullOrEmpty(skillsToMonitor) ? @"select SkillID,SkillExtension,SkillName from TMAC_Skills with (nolock)" :
                    @"select SkillID,SkillExtension,SkillName from TMAC_Skills with (nolock) Where SkillID in (" + skillsToMonitor + ")";
                DataTable skillExtnObj = SqlDataAccess.ExecuteDataTable(sql, ConfigurationData.ConntnString);
                return skillExtnObj;
            }
            catch (Exception ex)
            {
                log.Error("Exception in ExtnToSkillMap():" + ex);
                return null;
            }
        }

        /// <summary>
        /// Gets Agent-{List of Skills} data
        /// </summary>
        /// <returns>returns dataTable having agentid and total skills he has.</returns>
        public static DataTable GetAgentSkillInfo()
        {
            log.Debug("GetAgentSkillInfo()");
            try
            {
                string sql = @"SELECT AgentID , STUFF(( SELECT  ','+ SkillID FROM TMAC_Agent_Skills a
                                WHERE b.AgentID = a.AgentID FOR XML PATH('')),1 ,1, '')  Skills
                                FROM TMAC_Agent_Skills b with(nolock)
                                GROUP BY AgentID;";
                DataTable agentSkillObj = SqlDataAccess.ExecuteDataTable(sql, ConfigurationData.ConntnString);
                return agentSkillObj;
            }
            catch (Exception ex)
            {
                log.Error("Exception in ExtnToSkillMap():" ,ex);
                return null;
            }
        }

        public static List<string> GetAuxCodes()
        {
            try
            {
                string[] auxCodes = new string[] { };
                string sql = @"select Name from [dbo].[AGT_AUX_Codes] with (nolock)";
                DataTable dataTable = SqlDataAccess.ExecuteDataTable(sql, ConfigurationData.ConntnString);
                if(dataTable != null)
                {
                        auxCodes = dataTable
                        .AsEnumerable()
                        .Select(row => row.Field<string>("Name"))
                        .ToArray();
                }
                return auxCodes.ToList();
            }
            catch (Exception ex)
            {
                log.Error("Exception in GetAuxCodes():", ex);
            }
            return null;
        }

        public static Dictionary<string, int> GetAcceptableLevels()
        {
            log.Debug("GetAcceptableLevels()");
            try
            {
                string[] auxCodes = new string[] { };
                string sql = @"select distinct SkillID, AcceptableServiceLevel from TMAC_Skills with (nolock)";
                DataTable dataTable = SqlDataAccess.ExecuteDataTable(sql, ConfigurationData.ConntnString);
                if (dataTable != null)
                {
                    Dictionary<string, int> result = new Dictionary<string, int>();
                    foreach (DataRow item in dataTable.Rows)
                    {
                        string skillId = Convert.ToString(item["SkillID"]);
                        int accSlLevl = string.IsNullOrEmpty(item["AcceptableServiceLevel"].ToString()) ? ConfigurationData.acceptableSL : Convert.ToInt32(item["AcceptableServiceLevel"]) == 0 ? ConfigurationData.acceptableSL : Convert.ToInt32(item["AcceptableServiceLevel"]);
                        result.Add(skillId, accSlLevl);
                    }
                    return result;
                }
            }
            catch (Exception ex)
            {
                log.Error("Error in GetAcceptableLevels : " + ex);
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
        //        log.Error("Exception in ExtnToSkillMap():");
        //        return null;
        //    }
        //}
        #endregion
    }
}
