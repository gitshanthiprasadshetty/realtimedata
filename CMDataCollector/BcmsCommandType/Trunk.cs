using CMDataCollector.Connection;
using CMDataCollector.DataParser;
using CMDataCollector.Models;
using CMDataCollector.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CMDataCollector.BcmsCommandType
{
    class Trunk : CMCommandState
    {
        private static readonly Logger.Logger Log = new Logger.Logger(typeof(CMCommandState));
        private CMConnection _connValue;
        private readonly string _connectionKey;

        public Trunk(string connectionKey) : base(connectionKey)
        {
            Log.Debug("Run command for connection : " + _connectionKey);
            _connectionKey = connectionKey;
            _connValue = CMConnectionManager.GetInstance().GetConnectionValue(_connectionKey);
        }

        internal override void RunCommand()
        {
            _connValue.ExecuteTrunkCommand();
        }

        internal override void DataReceived(string data)
        {
            try
            {
                Log.Debug("DataReceived for Trunk : ");
                string[] ts = data.Split(new[] { "\nt" }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var s0 in ts)
                {
                    FrameTrunkData(s0);
                }
            }
            catch (Exception ex)
            {
                Log.Error("Exception in DataReceived : ", ex);
            }
        }

        /// <summary>
        /// Handle Connection related error.
        /// </summary>
        /// <param name="data">Exception Message</param>
        internal override void ErrorOccurred(string data)
        {
            Log.Debug("ErrorOccurred:" + _connectionKey + Environment.NewLine + "[" + data + "]");
            try
            {
                _connValue.HandleConnectionError(_connectionKey);
            }
            catch (Exception ex)
            {
                Log.Error("Exception in ErrorOccurred:" + _connectionKey, ex);
            }
        }
        /// <summary>
        /// Formats trunk-data recevied to Model data
        /// </summary>
        /// <param name="conn">CM Connection</param>
        /// <param name="trunkData">Raw data received from CM for mon-trunk command</param>
        void FrameTrunkData(string trunkData)
        {
            Log.Debug("FrameTrunkData() :");
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

                        for (int i = 1; i < data.Values.Count(); i++)
                        {
                            if (data.Values[i] != null && data.Values[i] != "")
                            {
                                string[] g = data.Values[i].Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                                result.Add(g[0], g.ToList());
                            }
                        }

                        foreach (var entry in result)
                        {
                            if (entry.Value != null)
                            {
                                _connValue.trunkTraffic = new TrunkGroupTraffic();
                                _connValue.trunkTraffic.TrunkGroup = Convert.ToInt32(entry.Key);
                                _connValue.trunkTraffic.TrunkGroupSize = Convert.ToInt32(entry.Value[1]);
                                _connValue.trunkTraffic.ActiveMembers = Convert.ToInt32(entry.Value[2]);
                                _connValue.trunkTraffic.QueueLength = Convert.ToInt32(entry.Value[3]);
                                _connValue.trunkTraffic.CallsWaiting = Convert.ToInt32(entry.Value[4]);
                                _connValue.trunkTraffic.Date = dateValue;
                                // call for update to cache.
                                CacheMemory.UpdateCacheMemory(_connValue.trunkTraffic);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error("Error in FrameTrunkData() : " + ex);
            }
        }      
    }
}
