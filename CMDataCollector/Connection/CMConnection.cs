using System;
using System.Collections.Generic;
using CMDataCollector.Models;
using System.Threading;
using CMDataCollector.Utilities;

namespace CMDataCollector.Connection
{
    public class CMConnection
    {

        #region Global variable declarations

        /// <summary>
        /// Logger
        /// </summary>
        private static readonly Logger.Logger Log = new Logger.Logger(typeof(CMConnection));

        /// <summary>
        /// total skill Range for each Connection established to CM
        /// </summary>
        public List<string> SkillRange;

        /// <summary>
        /// Unique CM connectionKey
        /// </summary>
        public string ConnectionKey { get; set; }   
        
        /// <summary>
        /// Hold the current state with respect to CM 
        /// </summary>
        public ConnectionState State { get; set; }

        /// <summary>
        /// About this connection which state is in with respect to connection state.
        /// </summary>
        public string ConnectionStateStatus;

        /// <summary>
        /// Holds the connection value
        /// </summary>
        private  SshConnect _conn;

        /// <summary>
        /// hold terminal state connection status
        /// </summary>
        public bool ConnStateStatus = false;

        /// <summary>
        /// Final model object to store values
        /// </summary>
        public  BcmsDashboard BcmsDashboard;

        /// <summary>
        /// Final model object of trunkTraffic to store values
        /// </summary>
        public TrunkGroupTraffic trunkTraffic;

        /// <summary>
        /// Final model object of trunkTraffic to store values
        /// </summary>
        public BcmsSystem BcmsSystemData;

        /// <summary>
        /// Holds Agent Lists for given skill in this connection.
        /// </summary>
        public List<AgentData> AgentDataSet;

        /// <summary>
        /// ssh commandresponse
        /// </summary>
        int _commandReponse = 0;

        /// <summary>
        /// becomes true once data is received for sendcommand
        /// </summary>
        public bool Responsereceived = false;


        public string DataReceived;

        public Dictionary<string, string> dataReceived = new Dictionary<string, string>();
        #endregion


        #region Constructor
        /// <summary>
        /// Constructor to set CM connectionId[key] and Range of values[skills] to be monitored
        /// under this CM connection.
        /// </summary>
        /// <param name="connId">CM ConnectionID</param>
        /// <param name="skillValue">Range of skills</param>
        public CMConnection(string connId, List<string> skillValue)
        {
            Log.Debug("CMConnection:" + connId);
            ConnectionKey = connId;
            SkillRange = skillValue;
        }

        #endregion

        #region CM connection states

        /// <summary>
        /// First Connection state to CM, On connect success to cm return true
        /// </summary>
        /// <returns>boolean value : true=success</returns>
        public bool Connect()
        {
            Log.Debug("Connect:" + ConnectionKey);
            try
            {
                // creat a instance of ssh to connect to remote CM machine
                _conn = new SshConnect();
                // initialize CM DataReceived event.
                GetEventData();

                bool isConnected = _conn.Connect(ConfigurationData.ServerAddress, ConfigurationData.Port,
                        ConfigurationData.UserName, ConfigurationData.Password);
                var val = isConnected ? true : false;
                return val;
            }
            catch (Exception ex)
            {
                Log.Error("Exception in Connect:" + ConnectionKey, ex);
                return false;
            }
        }

        /// <summary>
        /// Second connection state to CM, Execute only after success of first state i.e login[connect].
        /// </summary>
        /// <returns>returns boolean :true = success</returns>
        public bool Sat()
        {
            Log.Debug("Sat:" + ConnectionKey);
            try
            {
                _conn.SendCommand("\r\n");
                _conn.SendCommand("\r\n");
                _conn.SendCommand("\r\n");
                int result = _conn.SendCommand("sat");
                bool retunValue = result == 1 ? true : false;
                return retunValue;
            }
            catch (Exception ex)
            {
                Log.Error("Exception in Sat:" + ConnectionKey, ex);
                return false;
            }
        }

        /// <summary>
        /// Third connection state to CM, Execute only after success of second state.
        /// Terminal type used to connect to cm is OSSI
        /// </summary>
        public bool TerminalType()
        {
            Log.Debug("TerminalType:" + ConnectionKey);
            try
            {
                //issue ossi command
                int result = _conn.SendCommand("ossi");
                bool retunValue = result == 1 ? true : false;
                return retunValue;
            }
            catch (Exception ex)
            {
                Log.Error("Exception in TerminalType:" + ConnectionKey, ex);
                return false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void ExecuteCommand()
        {
            try
            {
                var t1 = new Thread(new ThreadStart(delegate
                   {
                       string c = "c lis agent \r\nt";

                       _commandReponse = _conn.SendCommand(c);
                       Log.Debug("ExecuteCommand commandReponse :[" + _commandReponse + "]:" + ConnectionKey);

                       if (_commandReponse == -1)
                       {
                           Log.Debug("ExecuteCommand failed: Reconnect:" + ConnectionKey);
                           Responsereceived = true;
                    //reset the connection
                    //set the state to not connected and connect again
                    State = new ConnectionNotEstablishedState(ConnectionKey);
                           ConnectionStateStatus = "ConnectionNotEstablishedState";
                           State.Connect();
                       }
                   }));
                t1.Start();
            }
            catch (Exception ex)
            {
            }
        }

        /// <summary>
        /// Execute BCMS command once all connection state is success.
        /// </summary>
        /// <param name="skillRange"></param>
        public void ExecuteSkillCommand(List<string> skillRange)
        { 
            Log.Debug("ExecuteSkillCommand:" + ConnectionKey);
            try
            {
                var t = new Thread(new ThreadStart(delegate
                {                    
                    // execute monitor command for each skills.
                    foreach (var skill in skillRange)
                    {
                        Log.Debug("ExecuteCommand: Skill to monitor:[" + skill + "]:" + ConnectionKey);
                        Responsereceived = false;

                        string skill1 = skill;
                        var t1 = new Thread(new ThreadStart(delegate
                        {
                            string c = "c mon bcms skill " + skill1 + "\r\nt";
                            Log.Debug("ExecuteCommand command [" + c + "]:" + ConnectionKey);
                            _commandReponse = _conn.SendCommand(c);
                            Log.Debug("ExecuteCommand commandReponse :[" + _commandReponse + "]:" + ConnectionKey);

                            if (_commandReponse == -1)
                            {
                                Log.Debug("ExecuteCommand failed: Reconnect:" + ConnectionKey);
                                Responsereceived = true;
                                //reset the connection
                                //set the state to not connected and connect again
                                State = new ConnectionNotEstablishedState(ConnectionKey);
                                ConnectionStateStatus = "ConnectionNotEstablishedState";
                                State.Connect();
                            }
                        }));
                        t1.Start();

                        // if no data/resposnce is received keep trying..

                        // this keep trying is required to handle data-mismatch between diff skills in same connection obj.
                        // doing this, control won't be passed to next sequence to execute untill we get whole data
                        // for executing skillid.
                        Log.Debug("Waiting for Response");
                        while (!Responsereceived)
                        {
                            //Log.Debug("No Response Received...");
                            Thread.Sleep(1000);
                        }
                        Thread.Sleep(100);
                        Log.Debug("Response received. Execute next skill in loop");
                    }
                }));
                t.Start();
                Thread.Sleep(100);
            }
            catch (Exception ex)
            {
                Log.Error("Exception in bcms ExecuteCommand :" + ConnectionKey, ex);
                //  return false;
            }
        }


        /// <summary>
        /// Execute BCMS command once all connection state is success.
        /// </summary>
        /// <param name="skillRange"></param>
        public void ExecuteHuntCommand(List<string> huntStartValues)
        {
            Log.Debug("ExecuteHuntCommand:" + ConnectionKey);
            try
            {               
                    var t = new Thread(new ThreadStart(delegate
                    {
                        foreach (var huntStartValue in huntStartValues)
                        {
                            // execute monitor command for each skills.
                            Log.Debug("ExecuteCommand: Hunt to monitor:[" + huntStartValue + "]:" + ConnectionKey);
                            Responsereceived = false;

                            string huntStartValue1 = huntStartValue;
                            var t1 = new Thread(new ThreadStart(delegate
                            {
                                string c = "c mon traffic hunt-groups " + huntStartValue1 + "\r\nt";
                                Log.Debug("ExecuteCommand command [" + c + "]:" + ConnectionKey);
                                _commandReponse = _conn.SendCommand(c);
                                Log.Debug("ExecuteCommand commandReponse :[" + _commandReponse + "]:" + ConnectionKey);

                                if (_commandReponse == -1)
                                {
                                    Log.Debug("ExecuteCommand failed: Reconnect:" + ConnectionKey);
                                    Responsereceived = true;
                                    //reset the connection
                                    //set the state to not connected and connect again
                                    State = new ConnectionNotEstablishedState(ConnectionKey);
                                    ConnectionStateStatus = "ConnectionNotEstablishedState";
                                    State.Connect();
                                }
                            }));
                            t1.Start();

                            // if no data/resposnce is received keep trying..

                            // this keep trying is required to handle data-mismatch between diff skills in same connection obj.
                            // doing this, control won't be passed to next sequence to execute untill we get whole data
                            // for executing skillid.
                            Log.Debug("Waiting for Response");
                            while (!Responsereceived)
                            {
                                //Log.Debug("No Response Received...");
                                Thread.Sleep(1000);
                            }
                            Thread.Sleep(100);
                            Log.Debug("Response received. Execute next skill in loop");
                        }
                    }));
                    t.Start();                
                Thread.Sleep(1000);
            }
            catch (Exception ex)
            {
                Log.Error("Exception in bcms ExecuteCommand :" + ConnectionKey, ex);
                //  return false;
            }
        }

        /// <summary>
        /// Execute TRUNK command once all connection state is success.
        /// </summary>
        public void ExecuteTrunkCommand()
        {
            var conn = CMConnectionManager.GetInstance().GetConnectionValue(ConnectionKey);
            Log.Debug("ExecuteCommand:" + conn.ConnectionKey);
            try
            {
                var t2 = new Thread(new ThreadStart(delegate
                {
                    while (true)
                    {
                        ManageExecution(conn);                     
                        Thread.Sleep(ConfigurationData.DashboardRefreshTime);
                    }
                }));
                t2.Start();
            }
            catch (Exception ex)
            {
                Log.Error("Exception in trunk ExecuteCommand :" + conn.ConnectionKey, ex);
            }
        }

        /// <summary>
        ///  Execute SYSTEM command once all connection state is success.
        /// </summary>
        public void ExecuteSystemCommand()
        {
            try
            {
                var t = new Thread(new ThreadStart(delegate
                {
                    // execute monitor command for each skills.
                  
                    Log.Debug("ExecuteCommand: monitor system : " + ConnectionKey);
                    Responsereceived = false;

                    var t1 = new Thread(new ThreadStart(delegate
                    {
                        string c = "c mon bcms system \r\nt";
                        Log.Debug("ExecuteCommand command [" + c + "]:" + ConnectionKey);
                        _commandReponse = _conn.SendCommand(c);
                        Log.Debug("ExecuteCommand commandReponse :[" + _commandReponse + "]:" + ConnectionKey);

                        if (_commandReponse == -1)
                        {
                            Log.Debug("ExecuteCommand failed: Reconnect:" + ConnectionKey);
                            Responsereceived = true;
                            //reset the connection
                            //set the state to not connected and connect again
                            State = new ConnectionNotEstablishedState(ConnectionKey);
                            ConnectionStateStatus = "ConnectionNotEstablishedState";
                            State.Connect();
                        }
                    }));
                    t1.Start();

                    // if no data/resposnce is received keep trying..
                    while (!Responsereceived)
                    {
                        Thread.Sleep(1000);
                    }
                    Thread.Sleep(100);

                }));
                t.Start();
                Thread.Sleep(100);
            }
            catch (Exception ex)
            {
                Log.Error("Error in ExecuteSystemCommand : " + ex);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Manages Execution while monitoring trunk-traffic.
        /// </summary>
        /// <param name="connValue">Trunk Connection Value</param>
        void ManageExecution(CMConnection connValue)
        {
            Log.Debug("CMConnection[ManageExecution]" + connValue.ConnectionKey);
            try
            {
                switch (connValue.ConnectionStateStatus)
                {
                    case "CMCommandState":
                        RunTrunkCommand();
                        break;
                    case "ConnectionNotEstablishedState":
                        State = new ConnectionNotEstablishedState(connValue.ConnectionKey);
                        State.Connect();
                        break;
                    case "ConnectionEstablishedState":
                        State = new ConnectionEstablishedState(connValue.ConnectionKey);
                        State.Sat();
                        break;
                    case "ConnectionTerminalTypeState":
                        State = new ConnectionTerminalType(connValue.ConnectionKey);
                        State.TerminalType();
                        break;
                }
            }
            catch (Exception ex)
            {
                Log.Error("Error in CMConnection[ManageExecution] conn.key" + connValue.ConnectionKey + " " + ex);
            }
        }

        /// <summary>
        /// ExecuteTrunkCommand to get all traffic details for each trunk-group.
        /// </summary>
        void RunTrunkCommand()
        {
            Log.Debug("CMConnection[ExecuteTrunkCommand]");
            try
            {
                Responsereceived = false;

                // pass the command to get trunk-traffic data.
                string c = "c mon traffic trunk-groups \r\nt";

                Log.Debug("ExecuteCommand command [" + c + "]:" + ConnectionKey);
                _commandReponse = _conn.SendCommand(c);
                Log.Debug("ExecuteCommand commandReponse :[" + _commandReponse + "]:" + ConnectionKey);
                if (_commandReponse == -1)
                {
                    Log.Debug("ExecuteCommand failed: Reconnect:" + ConnectionKey);
                    Responsereceived = true;
                    //reset the connection
                    //set the state to not connected and connect again
                    State = new ConnectionNotEstablishedState(ConnectionKey);
                    ConnectionStateStatus = "ConnectionNotEstablishedState";
                    State.Connect();
                }

                // if no data/resposnce is received keep trying..
                while (!Responsereceived)
                {
                    Thread.Sleep(100);
                }
            }
            catch (Exception ex)
            {
                Log.Error("Error in CMConnection[ExecuteTrunkCommand] conn.key" + ex);
            }
        }


        /// <summary>
        /// Re-Connect on failure.
        /// </summary>
        /// <param name="_connectionKey">Unique connection key id</param>
        public void HandleConnectionError(string _connectionKey)
        {
            try
            {
                State = new ConnectionNotEstablishedState(_connectionKey);
                ConnectionStateStatus = "ConnectionNotEstablishedState";
                State.Connect();
            }
            catch (Exception ex)
            {
                Log.Error("Exception in HandleConnectionError:" + ConnectionKey, ex);
            }
        }

        #endregion

        #region Events

        /// <summary>
        /// Local variable to bypass GetEventData method if it is called
        /// more than once in this application.
        /// </summary>
        private bool _getEventStarted;

        /// <summary>
        ///  Get data for each bcms skill and write to a cache memory.
        /// </summary>
        private void GetEventData()
        {
            // if true then bypass this method execution.
            if (_getEventStarted)
                return;

            Log.Debug("GetEventData:" + ConnectionKey);

            // this try block will be called only once while establishing connection to CM
            // at connect stage. CMDaa event is iniitalized at this time and whenever event is triggered on response 
            // for each command that is passed, this is called.
            try
            {
                _getEventStarted = true;
                _conn.CmData += s => 
                {
                    //2017-01-17 Sam
                    //send the data to current state for processing
                    if (State != null)
                        State.DataReceived(s);
                };

                _conn.CmError += er =>
                {
                    // 2017-06-08 Ashwath
                    // handle error on data receive failure, try reconnecting...
                    if (State != null)
                        State.ErrorOccurred(er);
                };
            }
            catch (Exception ex)
            {
                Log.Error("Exception in GetEventData:" + ConnectionKey, ex);
            }
        }

        #endregion
    }
}
