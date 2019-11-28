using System;
using System.Collections.Generic;
using CMDataCollector.BcmsCommandType;
using CMDataCollector.DataParser;
using CMDataCollector.Models;
using CMDataCollector.Utilities;

namespace CMDataCollector.Connection
{
    class CMCommandState : ConnectionState
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
        internal override void ExecuteCommand()
        {
            Log.Debug("ExecuteCommand:" + _connectionKey);
            try
            {
                //_connValue = CMConnectionManager.GetInstance().GetConnectionValue(_connectionKey);

                //if (_connValue.SkillRange.Contains("system"))
                //    _connValue.State = new BcmsCommandType.System(_connectionKey);
                //else if (_connValue.SkillRange.Contains("trunk"))
                //    _connValue.State = new Trunk(_connectionKey);
                //else if (_connValue.SkillRange.Contains("list"))
                //    _connValue.State = new Agent(_connectionKey);
                //else
                //    _connValue.State = new Skill(_connectionKey);

                //_connValue.State.RunCommand();

                Log.Debug("ExecuteCommand:" + _connectionKey);
                try
                {
                    _connValue = CMConnectionManager.GetInstance().GetConnectionValue(_connectionKey);

                    if(ConfigurationData.CommandType.ToLower() == "traffic")
                    {
                        // check which command to run
                        if (_connValue.SkillRange.Contains("trunk"))
                        {
                            // this will be called only once and runs in sep thread if trunk is enabled, 
                            _connValue.ExecuteTrunkCommand();
                        }
                        else
                        {
                            _connValue.ExecuteHuntCommand(_connValue.SkillRange);
                        }
                    }
                    else
                    {
                        Log.Debug("executing for skill connection key is : " + _connectionKey);
                        _connValue.ExecuteSkillCommand(_connValue.SkillRange);
                    }
                }
                catch (Exception ex)
                {
                    Log.Error("Exceptioin in ExecuteCommand:" + _connectionKey, ex);
                }
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
        internal override void DataReceived(string data)
        {
            try
            {
                Log.Debug("DataReceived : " + _connectionKey + Environment.NewLine + "[" + data + "]");

                if (ConfigurationData.CommandType.ToLower() == "traffic")
                {
                    ProcessHuntTrafficData(data);
                    return;
                }

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


                    //  _connValue.DataReceived += data;
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

        void ProcessData(string data)
        {
            try
            {
                Log.Debug("ProcessData :" + _connectionKey + Environment.NewLine + "[" + data + "]");

                _connValue.Responsereceived = true;
                var ts = data.Split(new[] { "\nt" }, StringSplitOptions.RemoveEmptyEntries);

                //if (ts[0].ToLower().Contains("c mon traffic trunk-groups"))
                //{
                //    Log.Debug("ProcessData : Data received for monitor traffic trunk for connection key : " + _connectionKey);
                //    foreach (var s0 in ts)
                //    {
                //        var trunk = new TrunkMonitor();
                //        trunk.CMTrunkData(_connValue, s0);
                //    }
                //}

                if (ts[0].ToLower().Contains("c mon bcms skill"))
                {
                    foreach (var s1 in ts)
                    {
                        Log.Debug("CMCommandState[ProcessData] to monitor : ");
                        var bcms = new BcmsMonitor();
                        _connValue.AgentDataSet = new List<AgentData>();
                        var b = bcms.Monitor(s1);
                        Log.Debug("returned data from bcms monitor");
                        if (b != null)
                        {
                            Log.Debug("start mapping data to model obj");
                            string extenstion = string.Empty;
                            var channel = ConfigurationData.GetChannel(b.Skill) ?? "";
                            if (channel.ToLower() == "email")
                                extenstion = ConfigurationData.GetExtensionId(b.Skill) ?? "";

                            var dateTime = DateTime.ParseExact(b.OldestCall, "H:mm", null, System.Globalization.DateTimeStyles.None);
                            long seconds = dateTime.Ticks / 10000000;

                            _connValue.BcmsDashboard = new RealtimeData();
                            _connValue.BcmsDashboard.Channel = channel;
                            _connValue.BcmsDashboard.SkillId = Convert.ToInt32(b.Skill);
                            // _connValue.BcmsDashboard.Date = b.Date;
                            _connValue.BcmsDashboard.SkillName = ConfigurationData.GetSkillName(b.Skill);//b.SkillName;
                            _connValue.BcmsDashboard.InteractionsInQueue = Convert.ToInt32(b.CallsWaiting); // (channel.ToLower() == "email") ? Convert.ToInt32(Connector.Proxy.WorkQueueProxy.GetQueueCount(extenstion)) : Convert.ToInt32(b.CallsWaiting);
                            _connValue.BcmsDashboard.AcceptedSL = Convert.ToInt32(b.AccptedSL);
                            _connValue.BcmsDashboard.OldestInteractionWaitTime = (int)seconds; //(channel.ToLower() == "email") ? Convert.ToInt32(Connector.Proxy.WorkQueueProxy.GetOldestWaitTime(extenstion)) : Convert.ToInt32(b.OldestCall);
                           // _connValue.BcmsDashboard.SLPercentage = Convert.ToDecimal(b.SL);
                            _connValue.BcmsDashboard.TotalAgentsStaffed = Convert.ToInt32(b.Staff);
                            _connValue.BcmsDashboard.TotalAgentsAvailable = Convert.ToInt32(b.Avail);
                            _connValue.BcmsDashboard.ActiveInteractions = Convert.ToInt32(b.ACD);
                            _connValue.BcmsDashboard.TotalAgentsInACW = Convert.ToInt32(b.ACW);
                            _connValue.BcmsDashboard.TotalAgentsInAUX = Convert.ToInt32(b.AUX);
                            // _connValue.BcmsDashboard.Extn = b.Extn;
                            // _connValue.BcmsDashboard.Other = b.Other;
                            if (b.AgentData != null)
                            {
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
                            _connValue.BcmsDashboard.AgentStats = _connValue.AgentDataSet;

                            Log.Debug("CMCommandState[ProcessData] Add/update to cachememory");
                            CacheMemory.UpdateCacheMemory(_connValue.BcmsDashboard);
                        }
                        else
                            Log.Debug("data returned is null");
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error("Error in ProcessData : " + ex);
            }
        }

        void ProcessHuntTrafficData(string data)
        {
            Log.Debug("ProcessHuntTrafficData");
            try
            {
                _connValue.Responsereceived = true;
                HuntGroupTrafficParser.HuntTrafficParser(data);
            }
            catch (Exception ex)
            {
                Log.Error("Error in ProcessHuntTrafficData : ", ex);
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
    }
}
