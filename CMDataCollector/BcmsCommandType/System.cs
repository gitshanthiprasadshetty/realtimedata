using CMDataCollector.Connection;
using CMDataCollector.DataParser;
using CMDataCollector.Models;
using CMDataCollector.Utilities;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace CMDataCollector.BcmsCommandType
{
    class System : CMCommandState
    {
        private static readonly Logger.Logger Log = new Logger.Logger(typeof(System));
        private CMConnection _connValue;
        private readonly string _connectionKey;

        public System(string connectionKey)  : base(connectionKey)
        {
            _connectionKey = connectionKey;
        }

        internal override void RunCommand()
        {
            Log.Debug("Run command for connection : " + _connectionKey);
            _connValue = CMConnectionManager.GetInstance().GetConnectionValue(_connectionKey);
            _connValue.ExecuteSystemCommand();
        }

        internal override void DataReceived(string data)
        {
            try
            {
                Log.Debug("DataReceived for System : ");
                EvaluateData(data);
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
        /// 
        /// </summary>
        /// <param name="data"></param>
        void EvaluateData(string data)
        {
            try
            {
                //Log.Debug("DataReceived : " + _connectionKey + Environment.NewLine + "[" + data + "]");
                //_connValue = CMConnectionManager.GetInstance().GetConnectionValue(_connectionKey);
                Log.Debug("Evaluate system Data : " + _connectionKey);
                string data1 = data;
                data1 = data1.Replace("\0", "");
                if (data1.Trim().ToLower().StartsWith("c mon bcms system") && data1.Trim().EndsWith("\nt"))
                {
                    Log.Debug("DataReceived : Full data is received at once.");
                    _connValue.DataReceived = data1;
                    Log.Debug("DataReceived : " + _connectionKey + " Data to process is : " + Environment.NewLine + "[" + _connValue.DataReceived + "]");
                    ProcessData(_connValue.DataReceived);
                    _connValue.DataReceived = "";
                }
                else
                {
                    data = data.Replace("\0", "");
                    //  _connValue = CMConnectionManager.GetInstance().GetConnectionValue(_connectionKey);

                    if (_connValue.DataReceived == null)
                    {
                        Log.Debug("DataReceived : is null!!!..");
                        _connValue.DataReceived = "";
                    }

                    if (_connValue.DataReceived != null)
                    {
                        Log.Debug("Append obtained data connectiondata");

                        if (_connValue.DataReceived == "" && data.Trim().ToLower().StartsWith("c mon bcms system"))
                        {
                            Log.Debug("DataReceived : Assigning first part of data to conn.obj : " + _connectionKey);
                            _connValue.DataReceived = data;
                        }
                        else if (_connValue.DataReceived != "" && _connValue.DataReceived.Trim().ToLower().StartsWith("c mon bcms system"))
                        {
                            Log.Debug("DataReceived : appending data to inital data : ");
                            _connValue.DataReceived += data;
                        }
                        else if (_connValue.DataReceived != "" && !(_connValue.DataReceived.Trim().ToLower().StartsWith("c mon bcms system")))
                        {
                            _connValue.DataReceived = "";
                        }
                    }


                    // when data received in chunks, wait till all chunks are obtained and created as a single piece of data before processing.
                    // lin0 will have complete data starting with c mon bcms system to \nt

                    string lin0 = _connValue.DataReceived;//.Replace("\0", "");
                    if (lin0.Trim().EndsWith("\nt"))
                    {
                        Log.Debug("DataReceived : inside lin0.Trim()... for conn key : " + _connectionKey + Environment.NewLine + " Data : " + _connValue.DataReceived);
                        if (lin0.Trim().ToLower().StartsWith("c mon bcms system"))
                        {
                            Log.Debug("chunks of data received combined and passed to process");
                            data = _connValue.DataReceived;
                            _connValue.DataReceived = "";
                            ProcessData(data);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error("Exception in System : " , ex);
            }
        }

        /// <summary>
        /// Formats system-data recevied to Model data
        /// </summary>
        /// <param name="conn">CM Connection</param>
        /// <param name="systemData">Raw data received from CM for mon-trunk command</param>
        void ProcessData(string systemData)
        {
            Log.Debug("ProcessData()");
            try
            {
                Log.Debug("ProcessData system Data : " + _connectionKey + Environment.NewLine + "[" + systemData + "]");
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
                            if (data.Values[i] != null)
                            {
                                string key = data.Values[i];
                                List<string> values = new List<string>(data.Values.GetRange(i, 12));
                                result.TryAdd(key, values);
                            }
                        }
                        List<BcmsSystem> sysListObj;
                        if (result != null)
                        {
                            sysListObj = new List<BcmsSystem>();
                            foreach (var entry in result)
                            {
                                if (entry.Value != null)
                                {
                                    _connValue.BcmsSystemData = new BcmsSystem
                                    {
                                        Date = dateValue,
                                        SkillName = entry.Value[0],
                                        CallsWaiting = entry.Value[2],
                                        OldestCall = entry.Value[3],
                                        AvgSpeedAnswer = entry.Value[4],
                                        AvailableAgents = entry.Value[5],
                                        AbandandCalls = entry.Value[6],
                                        AvgAbandTime = entry.Value[7],
                                        AcdCalls = entry.Value[8],
                                        AvgTalkTime = entry.Value[9],
                                        AvgAfterCall = entry.Value[10],
                                        PercentageSL = entry.Value[11]
                                    };
                                    sysListObj.Add(_connValue.BcmsSystemData);
                                }
                            }
                            // update to cache mem
                            int ret = CacheMemory.UpdateCacheMemory(sysListObj);
                            if (ret == 0)
                                Log.Debug("update to system cache mem is success");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error("Exception in ProcessData : ", ex);
            }
        }
    }
}
