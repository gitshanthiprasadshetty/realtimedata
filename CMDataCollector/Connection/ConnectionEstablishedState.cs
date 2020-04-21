using System;

namespace CMDataCollector.Connection
{
    class ConnectionEstablishedState : ConnectionState
    {
        /// <summary>
        /// Logger
        /// </summary>
        private static readonly Logger.Logger Log = new Logger.Logger(typeof(ConnectionEstablishedState));

        /// <summary>
        /// Unique CM-Connection Value  obtained for _connectionKey 
        /// </summary>
        CMConnection _connValue;

        /// <summary>
        /// Unique CM-Connection key 
        /// </summary>
        private readonly string _connectionKey;

        /// <summary>
        /// Constructor to set unique con-key 
        /// </summary>
        /// <param name="connectionKey"></param>
        public ConnectionEstablishedState(string connectionKey)
        {
            Log.Debug("ConnectionEstablishedState:" + connectionKey);
            _connectionKey = connectionKey;
        }

        /// <summary>
        /// Performs sat command operation
        /// </summary>
        internal override void Sat()
        {
            Log.Debug("Sat:" + _connectionKey);
            try
            {
                _connValue = CMConnectionManager.GetInstance().GetConnectionValue(_connectionKey);
                // check for successful execution of sat commands              
                if (_connValue.Sat())
                {
                    Log.Debug("Sat : Sat command executed successfully");

                    //now wait for data to be received
                }
                else
                {
                    Log.Debug("Sat command executed failed. Retrying again");
                    _connValue.State = new ConnectionNotEstablishedState(_connectionKey);
                    _connValue.ConnectionStateStatus = "ConnectionNotEstablishedState";
                    _connValue.State.Connect();
                }
            }
            catch (Exception ex)
            {
                Log.Error("Error in ConnectionEstablishedState[Sat] : " + ex);
            }
        }

        /// <summary>
        /// Data Received on sat command is executed.
        /// </summary>
        /// <param name="data">CM data response for Sat command.</param>
        internal override void DataReceived(string data)
        {
            Log.Debug("DataReceived:" + _connectionKey + Environment.NewLine + "[" + data + "]");

            //data received after issusing the sat command
            //check if this data is the terminal type
            const string ttype = "Terminal Type (513, 715, 4410, 4425, VT220, NTT, W2KTT, SUNT): [513]";

            try
            {
                if (data.Contains(ttype))
                {
                    Log.Debug("DataReceived contains terminal type:" + _connectionKey);
                    if (_connValue != null)
                    {
                        _connValue.State = new ConnectionTerminalType(_connectionKey);
                        _connValue.ConnectionStateStatus = "ConnectionTerminalTypeState";
                        Log.Debug("DataReceived: change state to ConnectionTerminalType:" + _connectionKey);
                        _connValue.State.TerminalType();
                    }
                }
                else
                {
                    Log.Debug("DataReceived NOT contains terminal type:" + _connectionKey);
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
