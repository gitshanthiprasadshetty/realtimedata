using CMDataCollector.Connection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMDataCollector.BcmsCommandType
{
    class Agent : CMCommandState
    {
        private static readonly Logger.Logger Log = new Logger.Logger(typeof(Agent));
        private CMConnection _connValue;
        private readonly string _connectionKey;

        public Agent(string connectionKey) : base(connectionKey)
        {
            _connectionKey = connectionKey;
        }

        internal override void RunCommand()
        {
            Log.Debug("Run command for connection : " + _connectionKey);
            _connValue = CMConnectionManager.GetInstance().GetConnectionValue(_connectionKey);
            _connValue.ExecuteCommand();
        }

        internal override void DataReceived(string data)
        {
            try
            {
                Log.Debug("DataReceived for skill : ");
                var res = data;
            }
            catch (Exception ex)
            {
                Log.Error("Error in DataReceived : " + ex);
            }
        }

    }
}
