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

  
        /*

        public static SkillData GetHistoricalData(List<string> skillExtn, string skillId)
        {
            log.Info($"GetHistoricalData() : skillExtn = {skillExtn} , skillid = {skillId}");
            try
            {
                //string sql = @"select Count(1) as COUNT from dbo.TMAC_WorkQueueHistory 
                //               where channel='voice' and Reason='ABN' and skill='" + Extn + "' and CreateDate= '" + DateTime.Now.Date.ToString("yyyyMMdd") + "'";
                SkillData skillInfo= new SkillData();
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
                string skillExtension = string.Join(",", skillExtn);
                string sql = @"EXEC [GET_HistoricalData] '" + DateTime.Now.Date.ToString("yyyyMMdd") + "'" + ',' + "'" + startTime + "'" + ',' + "'" + endTime + "'" + ',' + "'" + skillExtension + "'" + ',' + "'" + type + "'" + ',' + "'" + accSlLevl + "'";
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
                            TotalCallsHandled = Convert.ToInt32(item["TotalInteraction"]),
                            Date=item["Date"].ToString(),
                            skillId= Convert.ToInt32(item["SkillId"]),
                            AvgSpeedAnswer = Convert.ToInt32(item["AvgSpeedAnswer"]),
                            AvgTalkTime= Convert.ToInt32(item["AvgTalkTime"]),
                            TotalAuxTime = Convert.ToInt32(item["TotalAuxTime"]),
                            FlowIn = Convert.ToInt32(item["FlowIn"]),
                            FlowOut = Convert.ToInt32(item["FlowOut"]),
                            AvgStaffedTime = Convert.ToInt32(item["AvgStaffedTime"]),
                            TotalStaffedTime = Convert.ToInt32(item["TotalStaffedTime"]),
                            CallsHandledWithinThreshold = Convert.ToInt32(item["CallsHandledWithinSLAThreshold"]),
                            CallsAbandAfterThreshold = Convert.ToInt32(item["CallsAbandonedAfterSLAThreshold"]),
                            PassedCalls = Convert.ToInt32(item["PassedCalls"]),
                            TransferCalls = Convert.ToInt32(item["TransferCall"]),
                            TotalAbandTime = Convert.ToInt32(item["TotalAbandTime"]),
                            SpeedOfAnswer = Convert.ToInt32(item["SpeedOfAnswer"]),
                            TotalTalkTime = Convert.ToInt32(item["TotalTalkTime"]),
                            TotalStaffedAgents = Convert.ToInt32(item["TotalStaffedAgents"])
                        };
                       
                    }
                    return skillInfo;
                }
            }
            catch (Exception ex)
            {
                log.Error("Exception in GetHistoricalData():" , ex);
            }
            return new SkillData();
        }

        */

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
                        int accSlLevl = string.IsNullOrEmpty(item["AcceptableServiceLevel"].ToString()) ? ConfigurationData.acceptableSL : Convert.ToInt32(item["AcceptableServiceLevel"]);
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
    }
}
