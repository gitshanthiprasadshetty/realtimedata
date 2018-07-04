
using CMDataCollector.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CMDataCollector.Connection;

namespace CMDataCollector.DataParser
{
    class SystemMonitor
    {

        /// <summary>
        /// Logger
        /// </summary>
        private static readonly Logger.Logger Log = new Logger.Logger(typeof(SystemMonitor));



        /// <summary>
        /// Sends trunk-data recevied from CM to format
        /// </summary>
        public void CMSystemData(Connection.CMConnection conn, string systemData)
        {
            Log.Debug("SystemMonitor[CMSystemData]");
            try
            {
                FrameSystemData(conn, systemData);
            }
            catch (Exception ex)
            {
                Log.Error("Error in CMSystemData() : " + ex);
            }
        }

        /// <summary>
        /// Formats system-data recevied to Model data
        /// </summary>
        /// <param name="conn">CM Connection</param>
        /// <param name="systemData">Raw data received from CM for mon-trunk command</param>
        static void FrameSystemData(Connection.CMConnection conn, string systemData)
        {
            try
            {
                CMData data = Utils.ExtractCMData(systemData);
                //check if the response is successful
                if (data != null && data.Success)
                {
                    //check if we have atleast one field
                    if (data.Fields != null && data.Fields.Count > 0)
                    {
                        var result = new ConcurrentDictionary<string, List<string>>();
                        var dateValue = "";
                        if (data.Fields[0] == "000eff00")
                            dateValue = data.Values[0];
                        
                        for (int i = 1; i < data.Values.Count(); i = i + 12)
                        {
                            if(data.Values[i] != null)
                            {
                                string key = data.Values[i];
                                List<string> values = new List<string>(data.Values.GetRange(i, 12));
                                result.TryAdd(key, values);                               
                            }
                        }
                        List<BcmsSystem> bcmsSystem;
                        if (result != null)
                        {
                            bcmsSystem = new List<BcmsSystem>();
                            foreach (var entry in result)
                            {
                                if (entry.Value != null)
                                {
                                    conn.BcmsSystemData = new BcmsSystem
                                    {
                                        Date = dateValue,
                                        SkillName = entry.Value[0],
                                        CallsWaiting = entry.Value[1],
                                        OldestCall = entry.Value[2],
                                        AvgSpeedAnswer = entry.Value[3],
                                        AvailableAgents = entry.Value[4],
                                        AbandandCalls = entry.Value[5],
                                        AvgAbandTime = entry.Value[6],
                                        AcdCalls = entry.Value[7],
                                        AvgTalkTime = entry.Value[8],
                                        AvgAfterCall = entry.Value[9],
                                        PercentageSL = entry.Value[10]
                                    };
                                    bcmsSystem.Add(conn.BcmsSystemData);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error("Exception in FrameSystemData : ", ex);
            }
        }
    }
}
