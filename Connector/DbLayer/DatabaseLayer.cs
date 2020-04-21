using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Connector.DbLayer
{
    public class DatabaseLayer
    {
        private static Logger.Logger log = new Logger.Logger(typeof(SqlDataAccess));

        public static SkillData GetHistoricalData(string skillExtn, int skillId, int accptableSlLevel, string conn)
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

                accSlLevl = accptableSlLevel;
                string sql = @"EXEC [GET_HistoricalData] '" + DateTime.Now.Date.ToString("yyyyMMdd") + "'" + ',' + "'" + startTime + "'" + ',' + "'" + endTime + "'" + ',' + "'" + skillExtn + "'" + ',' + "'" + type + "'" + ',' + "'" + accSlLevl + "'";
                log.Debug("SQL Query : " + sql);
                DataTable dsusers = SqlDataAccess.ExecuteDataTable(sql, conn);
                if (dsusers != null)
                {
                    foreach (DataRow item in dsusers.Rows)
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
                log.Error("Exception in GetHistoricalData():", ex);
            }
            return new SkillData();
        }
    }

    public class SkillData
    {
        public int skillId { get; set; }
        public int ACDTime { get; set; }
        public int ACWTime { get; set; }
        public int AHTTime { get; set; }
        public int AbandCalls { get; set; }
        public decimal SLPercentage { get; set; }
        public int TotalACDInteractions { get; set; }
        public int TotalCallsHandled { get; set; }
        public int AvgHandlingTime { get; set; }
        public int AvgAbandTime { get; set; }
        public decimal AbandonPercentage { get; set; }
    }
}
