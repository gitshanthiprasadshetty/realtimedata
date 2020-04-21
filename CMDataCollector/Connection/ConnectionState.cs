namespace CMDataCollector.Connection
{
    public class ConnectionState
    {
        /// <summary>
        /// Logger
        /// </summary>
        private static readonly Logger.Logger Log = new Logger.Logger(typeof(ConnectionState));

        /// <summary>
        /// First conn-state to connect to CM
        /// Overriden in ConnecitonNotEstablishedState child class.
        /// </summary>
        internal virtual void Connect() { }

        /// <summary>
        /// Second conn-state while establishing connection to CM.
        /// after Connection is established, sat command is passed in this state.
        /// Overriden in ConnecitonEstablishedState child class.
        /// </summary>
        internal virtual void Sat() { }

        /// <summary>
        /// Third conn-state while establishing connection to CM.
        /// 'ossit' is used here as a terminal-type.
        /// Overriden in ConnecitonTerminalType child class.
        /// </summary>
        internal virtual void TerminalType() { }

        /// <summary>
        /// Last/fourth conn-state [Command execution state.]
        /// Overriden in CMCommandState child class.
        /// </summary>
        internal virtual void ExecuteCommand() { }

        /// <summary>
        /// 
        /// </summary>
        internal virtual void RunCommand() { }

        /// <summary>
        /// Method will be called when data is received for each-of these states and 
        /// this method is overriden in all childclasses to handle CM response/data respectively.
        /// </summary>
        /// <param name="data">Data Recevied from CM for commands passed in eachstate</param>
        internal virtual void DataReceived(string data)
        {
        }

        /// <summary>
        /// Error Occurred event triggers if error occurs during data recevice.
        /// </summary>
        /// <param name="data"></param>
        internal virtual void ErrorOccurred(string data)
        {
        }
    }
}
