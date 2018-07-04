using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CMDataCollector.Models;
using CMDataCollector.Utilities;

namespace CMDataCollector.DataParser
{
    public class TrunkMonitor
    {
        /// <summary>
        /// Logger
        /// </summary>
        private static readonly Logger.Logger Log = new Logger.Logger(typeof(TrunkMonitor));

        /// <summary>
        /// Sends trunk-data recevied from CM to format
        /// </summary>
        public void CMTrunkData(Connection.CMConnection conn, string trunkData)
        {
            Log.Debug("TrunkMonitor[CMTrunkData]");
            try
            {
                FormatTrunkData(conn, trunkData);
            }
            catch (Exception ex)
            {
                Log.Error("Error in CMTrunkData() : " + ex);
            }
        }

        /// <summary>
        /// Formats trunk-data recevied to Model data
        /// </summary>
        /// <param name="conn">CM Connection</param>
        /// <param name="trunkData">Raw data received from CM for mon-trunk command</param>
        static void FormatTrunkData(Connection.CMConnection conn, string trunkData)
        {
            Log.Debug("TrunkMonitor[FormatTrunkData]");
            try
            {
                //extract receveid data into fields and values arrays
                CMData data = Utils.ExtractCMData(trunkData);
                //check if the response is successful
                if (data != null && data.Success)
                {
                    //check if we have atleast one field
                    if (data.Fields != null && data.Fields.Count > 0)
                    {                                             
                        //List<string> result;
                        var result = new Dictionary<string, List<string>>();
                        var dateValue = "";
                        if (data.Fields[0] == "0001ff00")
                              dateValue = data.Values[0];

                        for(int i = 1; i < data.Values.Count(); i++)
                        {
                            if (data.Values[i] != null && data.Values[i] != "")
                            {
                                string[] g = data.Values[i].Split(new[] {" "}, StringSplitOptions.RemoveEmptyEntries);
                                result.Add(g[0], g.ToList());
                            }
                        }

                        foreach(var entry in result)
                        {
                            if (entry.Value != null)
                            {
                                conn.trunkTraffic = new TrunkGroupTraffic();
                                conn.trunkTraffic.TrunkGroup = Convert.ToInt32(entry.Key);
                                conn.trunkTraffic.TrunkGroupSize = Convert.ToInt32(entry.Value[1]);
                                conn.trunkTraffic.ActiveMembers = Convert.ToInt32(entry.Value[2]);
                                conn.trunkTraffic.QueueLength = Convert.ToInt32(entry.Value[3]);
                                conn.trunkTraffic.CallsWaiting = Convert.ToInt32(entry.Value[4]);
                                conn.trunkTraffic.Date = dateValue;
                                // call for update to cache.
                                CacheMemory.UpdateCacheMemory(conn.trunkTraffic);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error("Error in FormatTrunkData() : " + ex);
            }
        }
    }
}
