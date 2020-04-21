using System;

namespace CMDataCollector.Connection
{
    internal class ConnectionTerminalType : ConnectionState
    {
        /// <summary>
        /// Logger
        /// </summary>
        private static readonly Logger.Logger Log = new Logger.Logger(typeof(ConnectionTerminalType));

        /// <summary>
        /// Unique CM-Connection key 
        /// </summary>
        private readonly string _connectionKey;

        /// <summary>
        /// Unique CM-Connection Value  obtained for _connectionKey 
        /// </summary>
        private CMConnection _connValue;


        /// <summary>
        /// Constructor to set unique con-key 
        /// </summary>
        /// <param name="connectionKey"></param>
        public ConnectionTerminalType(string connectionKey)
        {
            Log.Debug("ConnectionTerminalType:" + connectionKey);
            _connectionKey = connectionKey;
        }


        /// <summary>
        /// Terminal type that is used to connect to CM
        /// </summary>
        internal override void TerminalType()
        {
            Log.Debug("TerminalType:" + _connectionKey);
            try
            {
                _connValue = CMConnectionManager.GetInstance().GetConnectionValue(_connectionKey);
                if (_connValue.TerminalType())
                {
                    Log.Debug("TerminalType : TerminalType command executed successfully");
                }
                //send ossi command
                else
                {
                    Log.Debug("TerminalType command executed failed. Retrying again");
                    _connValue.State = new ConnectionNotEstablishedState(_connectionKey);
                    _connValue.ConnectionStateStatus = "ConnectionNotEstablishedState";
                    _connValue.State.Connect();
                }
                //wait for "ossit" response 
            }
            catch (Exception ex)
            {
                Log.Error("Exception in TerminalType:" + _connectionKey, ex);
            }
        }


        /// <summary>
        /// Data Received for termial-type command[ossi] passed in previous state.
        /// </summary>
        /// <param name="data">Received data from CM</param>
        internal override void DataReceived(string data)
        {
            Log.Debug("DataReceived:" + _connectionKey + Environment.NewLine + "[" + data + "]");

            try
            {
                //data received after issuing ossi command
                _connValue = CMConnectionManager.GetInstance().GetConnectionValue(_connectionKey);
                //check if "ossit" response has received. then we can start commands
                string lin = data.Replace("\0", "");

                // 24/11/2017
                // ---------------
                Log.Debug("DataReceived : previous chuck of data in connection " + _connValue.dataReceived);
                string data1 = data;
                if (data1.Trim().ToLower().StartsWith("o") && data1.Trim().EndsWith("\nt"))
                {
                    _connValue.DataReceived = data1;
                    Log.Debug("Received Terminal datatype as :" + data1);
                }
                else
                {
                    if (_connValue.DataReceived == "" && data1.Trim().ToLower().StartsWith("o"))
                    {
                        Log.Debug("DataReceived : first chunk of data is received " + data1);
                        _connValue.DataReceived = data1;
                        Log.Debug("DataReceived : data after binding = " + _connValue.dataReceived);
                    }
                    else if (_connValue.DataReceived != "" && (_connValue.DataReceived.Trim().ToLower().StartsWith("o")))
                    {
                        Log.Debug("DataReceived: piece of data obtained is : " + data1);
                        _connValue.DataReceived += data1;
                        Log.Debug("DataReceived : combined data is " + _connValue.DataReceived);
                    }
                    else if (_connValue.DataReceived != "" && !(_connValue.DataReceived.Trim().ToLower().StartsWith("o")))
                    {
                        Log.Info("DataReceived: previous connection terminal data is empty, and not starting with letter o : ");
                        _connValue.DataReceived = "";
                    }
                }

                //-------------------------
                lin = _connValue.DataReceived;
                _connValue.DataReceived = "";
                Log.Debug("Print final terminal type obtained : " + lin);
                if (lin.Trim().EndsWith("t"))
                {
                    Log.Debug("Line ends with 't':" + _connectionKey);
                    if (lin.Contains("ossi"))
                    {
                        // if data contains ossi in it, set state to next state[Command]
                        Log.Debug("Line contains 'ossi': " + _connectionKey);
                        _connValue.ConnStateStatus = true;
                        _connValue.State = new CMCommandState(_connectionKey);
                        _connValue.ConnectionStateStatus = "CMCommandState";
                        Log.Debug("Change state to CMCommandState:" + _connectionKey);
                        _connValue.State.ExecuteCommand();
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error("Exception in DataReceived:" + _connectionKey, ex);
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