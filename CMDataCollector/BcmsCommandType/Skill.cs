using Connector.Proxy;
using CMDataCollector.Connection;
using CMDataCollector.DataParser;
using CMDataCollector.Models;
using CMDataCollector.Utilities;
using System;
using System.Collections.Generic;

namespace CMDataCollector.BcmsCommandType
{
    class Skill : CMCommandState
    {
        private static readonly Logger.Logger Log = new Logger.Logger(typeof(Skill));
        private CMConnection _connValue;
        private readonly string _connectionKey;

        public Skill(string connectionKey) : base(connectionKey)
        {
            _connectionKey = connectionKey;
        }

        internal override void RunCommand()
        {
            Log.Debug("Run command for connection : " + _connectionKey);
            _connValue = CMConnectionManager.GetInstance().GetConnectionValue(_connectionKey);
            _connValue.ExecuteBcmsCommand(_connValue.SkillRange);
        }

        internal override void DataReceived(string data)
        {
            try
            {
                Log.Debug("DataReceived for skill : ");
                EvaluateData(data);
            }
            catch (Exception ex)
            {
                Log.Error("Error in DataReceived : " + ex);
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

        void EvaluateData(string data)
        {
            // Log.Debug("EvaluateData : " + _connectionKey + Environment.NewLine + "[" + data + "]");
            Log.Debug("Evaluate skill Data : " + _connectionKey);
            _connValue = CMConnectionManager.GetInstance().GetConnectionValue(_connectionKey);
            string data1 = data;
            data1 = data1.Replace("\0", "");
            if (data1.Trim().ToLower().StartsWith("c mon bcms skill") && data1.Trim().EndsWith("\nt"))
            {
                Log.Debug("DataReceived : Full data is received at once.");
                _connValue.DataReceived = data1;
                Log.Debug("DataReceived : " + _connectionKey + " Data to process is : " + Environment.NewLine + "[" + _connValue.DataReceived + "]");
                ProcessData(_connValue.DataReceived);
                _connValue.DataReceived = "";
                //  Log.Debug("DataReceived : clear connection data  : [" + _connectionKey + "]" + Environment.NewLine + "[" + _connValue.DataReceived + "]");
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

                    if (_connValue.DataReceived == "" && data.Trim().ToLower().StartsWith("c mon bcms skill"))
                    {
                        Log.Debug("DataReceived : Assigning first part of data to conn.obj : " + _connectionKey);
                        _connValue.DataReceived = data;
                        //  Log.Debug("DataReceived0 : " + _connectionKey + " Combined Data is : " + Environment.NewLine + "[" + _connValue.DataReceived + "]");
                    }
                    else if (_connValue.DataReceived != "" && _connValue.DataReceived.Trim().ToLower().StartsWith("c mon bcms skill"))
                    {
                        Log.Debug("DataReceived : appending data to inital data : ");
                        _connValue.DataReceived += data;
                        // Log.Debug("DataReceived1 : " + _connectionKey + " Combined Data is : " + Environment.NewLine + "[" + _connValue.DataReceived + "]");
                    }
                    else if (_connValue.DataReceived != "" && !(_connValue.DataReceived.Trim().ToLower().StartsWith("c mon bcms skill")))
                    {
                        //  Log.Debug("DataReceived2 : No first part data is found for connection : [" + _connectionKey +"]" + Environment.NewLine + "[" + _connValue.DataReceived + "]");
                        _connValue.DataReceived = "";
                        //  Log.Debug("DataReceived2 : cleared the data for connection : [" + _connectionKey + "]" + Environment.NewLine + "[" + _connValue.DataReceived + "]");
                    }
                }


                // when data received in chunks, wait till all chunks are obtained and created as a single piece of data before processing.
                // lin0 will have complete data starting with c mon bcms skill to \nt

                string lin0 = _connValue.DataReceived;//.Replace("\0", "");
                if (lin0.Trim().EndsWith("\nt"))
                {
                    Log.Debug("DataReceived : inside lin0.Trim()... for conn key : " + _connectionKey + Environment.NewLine + " Data : " + _connValue.DataReceived);
                    if (lin0.Trim().ToLower().StartsWith("c mon bcms skill"))
                    {
                        Log.Debug("chunks of data received combined and passed to process");
                        data = _connValue.DataReceived;
                        _connValue.DataReceived = "";
                        ProcessData(data);
                    }
                }
            }
        }

        /// <summary>
        /// Process the obtained data.
        /// </summary>
        /// <param name="data"></param>
        void ProcessData(string data)
        {
            try
            {
                Log.Debug("ProcessData :" + _connectionKey + Environment.NewLine + "[" + data + "]");

                _connValue.Responsereceived = true;
                var ts = data.Split(new[] { "\nt" }, StringSplitOptions.RemoveEmptyEntries);

                if (ts[0].ToLower().Contains("c mon bcms skill"))
                {
                    foreach (var s1 in ts)
                    {
                        Log.Debug("CMCommandState[ProcessData] to monitor : ");
                        var bcms = new BcmsMonitor();
                        _connValue.AgentDataSet = new List<AgentData>();
                        var b = bcms.Monitor(s1);
                        if (b != null)
                        {
                            string extenstion = string.Empty;
                            var channel = ConfigurationData.GetChannel(b.Skill) ?? "";
                            if (channel.ToLower() == "email")
                                extenstion = ConfigurationData.GetExtensionId(b.Skill) ?? "";

                            _connValue.BcmsDashboard = new BcmsDashboard
                            {
                                Channel = channel,
                                Skill = b.Skill,
                                Date = b.Date,
                                SkillName = b.SkillName,
                                CallsWaiting = (channel.ToLower() == "email") ? WorkQueueProxy.GetQueueCount(extenstion) : b.CallsWaiting,
                                AccptedSL = b.AccptedSL,
                                OldestCall = (channel.ToLower() == "email") ? WorkQueueProxy.GetOldestWaitTime(extenstion) : b.OldestCall,
                                SL = b.SL,
                                Staff = b.Staff,
                                Avail = b.Avail,
                                ACD = b.ACD,
                                ACW = b.ACW,
                                AUX = b.AUX,
                                Extn = b.Extn,
                                Other = b.Other
                            };


                            if (b.AgentData != null)
                            {
                                Log.Debug("Attach agent details to connection");
                                for (int i = 0; i < b.AgentData.Count; i++)
                                {
                                    _connValue.AgentDataSet.Add(new AgentData
                                    {
                                        LoginId = b.AgentData[i].LoginId,
                                        State = b.AgentData[i].State
                                    });
                                }
                            }
                            // total agentlist currently loggedin having this skill
                            _connValue.BcmsDashboard.AgentData = _connValue.AgentDataSet;

                            Log.Debug("CMCommandState[ProcessData] Add/update to cachememory");
                            CacheMemory.UpdateCacheMemory(_connValue.BcmsDashboard);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error("Error in ProcessData : " + ex);
            }
        }
    }
}
