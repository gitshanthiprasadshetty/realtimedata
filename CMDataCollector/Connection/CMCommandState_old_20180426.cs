using System;
using System.Collections.Generic;
using CMDataCollector.Models;
using CMDataCollector.DataParser;
using System.Threading;
using CMDataCollector.Utilities;
using System.Configuration;
using BcmsExternalInterfaceLayer.Proxy;

namespace CMDataCollector.Connection
{
    class CMCommandState:ConnectionState
    {
        /// <summary>
        /// Logger
        /// </summary>
        private static readonly Logger.Logger Log = new Logger.Logger(typeof(CMCommandState));


        /// <summary>
        /// Unique CM-Connection Value  obtained for _connectionKey 
        /// </summary>
        CMConnection _connValue;

        /// <summary>
        ///  Unique CM-Connection key 
        /// </summary>
        private readonly string _connectionKey;

        /// <summary>
        /// Constructor to set unique con-key 
        /// </summary>
        /// <param name="connectionKey"></param>
        public CMCommandState(string connectionKey)
        {
            Log.Debug("CMCommandState: " + connectionKey);
           _connectionKey = connectionKey;
        }

        /// <summary>
        /// Run the command required to monitor bcms skill
        /// </summary>
        public override void ExecuteCommand()
        {
            Log.Debug("ExecuteCommand:" + _connectionKey);
            try
            {
                _connValue = CMConnectionManager.GetInstance().GetConnectionValue(_connectionKey);

                _connValue.ExecuteCommand(_connValue.SkillRange);

                // check which command to run
                //if (_connValue.SkillRange.Contains("trunk") || _connValue.SkillRange.Contains("system"))
                //{
                // this will be called only once and runs in sep thread if trunk is enabled, 
                //_connValue.ExecuteCommand(_connValue.SkillRange);
                //}
                //else
                //{
                //foreach (var skills in _connValue.SkillRange) {
                //    Log.Debug("ExecuteCommand : skill" + skills + " to monitor for connection key : "+ _connectionKey);
                //}            
                //}
            }
            catch (Exception ex)
            {
                Log.Error("Exceptioin in ExecuteCommand:" + _connectionKey, ex);
            }
        }

        /// <summary>
        /// On response for monitor bcms command data.
        /// Process the recevied data.
        /// </summary>
        /// <param name="data">CM raw data</param>
        public override void DataReceived(string data)
        {
            try
            {
                Log.Debug("DataReceived : " + _connectionKey + Environment.NewLine + "[" + data + "]");
                _connValue = CMConnectionManager.GetInstance().GetConnectionValue(_connectionKey);
                string data1 = data;
                data1 = data1.Replace("\0", "");
                if (data1.Trim().ToLower().StartsWith("c mon bcms skill") &&  data1.Trim().EndsWith("\nt"))
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
                    //else
                    //{
                    //    Log.Debug("DataReceived : inside else");
                    //    if (_connValue.DataReceived != null)
                    //    {
                    //        _connValue.DataReceived += data;
                    //        Log.Debug("DataReceived : " + _connectionKey + " Combined Data is : " + Environment.NewLine + "[" + _connValue.DataReceived + "]");
                    //    }
                    //    else
                    //    {
                    //        _connValue.DataReceived = "";
                    //        _connValue.DataReceived = data;
                    //        Log.Debug("DataReceived : Not used loop");
                    //    }

                    //    if (_connValue.DataReceived.Trim().EndsWith("t"))
                    //    {
                    //        if (_connValue.DataReceived.StartsWith("c mon bcms skill"))
                    //        {
                    //            Log.Debug("Chunk of data combined and passed to process");
                    //            data = _connValue.DataReceived;
                    //            _connValue.DataReceived = "";
                    //            ProcessData(data);
                    //        }
                    //    }
                    //}
                }
            }
            catch (Exception ex)
            {
                Log.Error("Exception in DataReceived:" + _connectionKey, ex);                
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

                if (ts[0].ToLower().Contains("c mon traffic trunk-groups"))
                {
                    Log.Debug("ProcessData : Data received for monitor traffic trunk | connection key : " + _connectionKey);
                    foreach (var s0 in ts)
                    {
                        var trunk = new TrunkMonitor();
                        trunk.CMTrunkData(_connValue, s0);
                    }
                }

                if (ts[0].ToLower().Contains("c mon bcms system"))
                {
                    Log.Debug("ProcessData : Data received for monitor bcms system | connection key : " + _connectionKey);
                    foreach (var s0 in ts)
                    {
                        var system = new SystemMonitor();
                        system.CMSystemData(_connValue, s0);
                    }
                }

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
                            //_connValue.BcmsDashboard = new BcmsDashboard();
                            //_connValue.BcmsDashboard.Channel = channel;
                            //_connValue.BcmsDashboard.Skill = b.Skill;
                            //_connValue.BcmsDashboard.Date = b.Date;
                            //_connValue.BcmsDashboard.SkillName = b.SkillName;
                            //_connValue.BcmsDashboard.CallsWaiting = (channel.ToLower() == "email")? WorkQueueProxy.GetQueueCount(extenstion) : b.CallsWaiting;
                            //_connValue.BcmsDashboard.AccptedSL = b.AccptedSL;
                            //_connValue.BcmsDashboard.OldestCall = (channel.ToLower() == "email") ? WorkQueueProxy.GetOldestWaitTime(extenstion) : b.OldestCall;
                            //_connValue.BcmsDashboard.SL = b.SL;
                            //_connValue.BcmsDashboard.Staff = b.Staff;
                            //_connValue.BcmsDashboard.Avail = b.Avail;
                            //_connValue.BcmsDashboard.ACD = b.ACD;
                            //_connValue.BcmsDashboard.ACW = b.ACW;
                            //_connValue.BcmsDashboard.AUX = b.AUX;
                            //_connValue.BcmsDashboard.Extn = b.Extn;
                            //_connValue.BcmsDashboard.Other = b.Other;
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

        /// <summary>
        /// Handle Connection related error.
        /// </summary>
        /// <param name="data">Exception Message</param>
        public override void ErrorOccurred(string data)
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
    }
}
