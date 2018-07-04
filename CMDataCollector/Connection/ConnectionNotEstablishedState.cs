using CMDataCollector.Utilities;
using System;
using System.Threading;

namespace CMDataCollector.Connection
{
    class ConnectionNotEstablishedState : ConnectionState
    {
        /// <summary>
        /// Logger
        /// </summary>
        private static readonly Logger.Logger Log = new Logger.Logger(typeof(ConnectionNotEstablishedState));

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
        public ConnectionNotEstablishedState(string connectionKey)
        {
            Log.Debug("ConnectionNotEstablishedState:" + connectionKey);
            _connectionKey = connectionKey;
        }

        /// <summary>
        /// Tries to connect to CM
        /// </summary>
        internal override void Connect()
        {
            Log.Debug("Connect:" + _connectionKey);
            try
            {
                int _counter = 0;

                // check if login is success
                _connValue = CMConnectionManager.GetInstance().GetConnectionValue(_connectionKey);
                while (!_connValue.Connect())
                {
                    // connection error try again
                    if (_counter >= ConfigurationData.MaxTriesOnCMConFailure)
                    {
                        Log.Debug("Max tries to CM done. Please check the Network connection.");

                        // When max tries execedz check for action to be taken
                        switch (ConfigurationData.ActionOnCMConFailure)
                        {
                            case "kill":
                                Log.Debug("Killing the application");
                                Environment.Exit(-1);
                                break;
                            case "stop":
                                Log.Debug("Stopping the application");
                                Environment.Exit(0);
                                break;
                        }
                    }

                    Log.Debug("Connect : Connect to CM failed. Retrying again:" + _connectionKey);
                    _connValue = CMConnectionManager.GetInstance().GetConnectionValue(_connectionKey);
                    _counter++;
                    Thread.Sleep(15000);
                }

                Log.Debug("Connect : Connect state is success:" + _connectionKey);
                //change the state to Established and delegate control to new state
                _connValue.State = new ConnectionEstablishedState(_connectionKey);
                _connValue.ConnectionStateStatus = "ConnectionEstablishedState";
                Log.Debug("Connect : Set to next connection state to ConnectionEstablishedState :" + _connectionKey);
                _connValue.State.Sat();
            }
            catch (Exception ex)
            {
                Log.Error("Exception in Connect : " + _connectionKey, ex);
            }
        }
    }
}
