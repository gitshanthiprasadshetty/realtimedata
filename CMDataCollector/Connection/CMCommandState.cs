using System;
using CMDataCollector.BcmsCommandType;

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
                _connValue = CMConnectionManager.GetInstance().GetConnectionValue(_connectionKey);

                if (_connValue.SkillRange.Contains("system"))
                    _connValue.State = new BcmsCommandType.System(_connectionKey);
                else if (_connValue.SkillRange.Contains("trunk"))
                    _connValue.State = new Trunk(_connectionKey);
                else if (_connValue.SkillRange.Contains("list"))
                    _connValue.State = new Agent(_connectionKey);
                else
                    _connValue.State = new Skill(_connectionKey);

                _connValue.State.RunCommand();
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
            Log.Debug("DataReceived in CM Commandstate");
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
