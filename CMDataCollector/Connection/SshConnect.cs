using System;
using Renci.SshNet;
using Renci.SshNet.Common;

namespace CMDataCollector.Connection
{
    public class SshConnect
    {
        /// <summary>
        /// Logger
        /// </summary>
        //private static readonly ILog Log = LogManager.GetLogger(typeof(SshConnect).Name);
        static Logger.Logger Log = new Logger.Logger(typeof(SshConnect));

        /// <summary>
        /// 
        /// </summary>
        private SshClient _sshClient;

        /// <summary>
        /// 
        /// </summary>
        private ShellStream _shellStream;

        /// <summary>
        /// Delegate declaration for an event OnDataReceived.
        /// </summary>
        /// <param name="data">Data received</param>
        public delegate void OnDataReceived(string data);

        /// <summary>
        /// Event declaration, calls delegate when data is received for a command execution.
        /// </summary>
        public event OnDataReceived CmData;

        /// <summary>
        /// Delegate declaration for an event OnErrorOccured.
        /// </summary>
        /// <param name="data">Data received</param>
        public delegate void OnErrorOccured(string data);

        /// <summary>
        /// Event declaration, calls delegate when error occured while retriving data.
        /// </summary>
        public event OnErrorOccured CmError;

        // adding for load test
        public int tempCounter = 0;
        public int tempCounterSuccess = 0;

        // conction failure
        private string _password;

        /// <summary>
        /// Establish a Connection to Remote server
        /// </summary>
        /// <param name="server">Remote server[CM] ip-address</param>
        /// <param name="port">Remote server Port to connect</param>
        /// <param name="userName">Login-Username</param>
        /// <param name="password">Login-Password</param>
        /// <returns>success : ture, failure : false</returns>
        public bool Connect(string server, int port, string userName, string password)
        {
            Log.Debug("SshConnect[Connect]");
            try
            {
                this._password = password;
                KeyboardInteractiveAuthenticationMethod kauth = new KeyboardInteractiveAuthenticationMethod(userName);
                PasswordAuthenticationMethod pauth = new PasswordAuthenticationMethod(userName, this._password);

                kauth.AuthenticationPrompt += new EventHandler<AuthenticationPromptEventArgs>(HandleKeyEvent);

                _sshClient = new SshClient(new ConnectionInfo(server, port, userName, pauth, kauth));
                _sshClient.Connect();                
                _shellStream = _sshClient.CreateShellStream("FSCLI", 0, 0, 0, 0, 2048);

                // DataReceived event to read data for the executed cm command.
                _shellStream.DataReceived += (sender, dataEvent) =>
                {
                    if (CmData != null)
                        CmData(_shellStream.Read());
                };

                // ErrorOccured event used to handle error during data reception.
                _shellStream.ErrorOccurred += (sender, dataEvent) =>
                {
                    Log.Error("SshConnect.ErrorOccurred : " + dataEvent);
                    var exMessage = dataEvent.Exception.Message + Environment.NewLine + dataEvent.Exception.InnerException;
                    CmError(exMessage);
                };

                return true;
            }
            catch (Exception ex)
            {
                Log.Error("Exception : " + ex);
                return false;
            }
        }
       
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HandleKeyEvent(Object sender, AuthenticationPromptEventArgs e)
        {
            Log.Debug("SshConnect[HandleKeyEvent]");
            try
            {
                foreach (AuthenticationPrompt prompt in e.Prompts)
                {
                    if (prompt.Request.IndexOf("Password:", StringComparison.InvariantCultureIgnoreCase) != -1)
                    {
                        prompt.Response = _password;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error("HandleKeyEvent : " + ex);
            }
        }

        /// <summary>
        /// Sends command to remote server using ssh.
        /// </summary>
        /// <param name="command">command to be executed.</param>
        /// <returns>returns int(1 for success, 0 for failure, -1 for connection failure)</returns>
        public int SendCommand(string command)
        {
            Log.Debug("SshConnect[SendCommand]");
            try
            {
                _shellStream.WriteLine(command);
                return 1;
            }
            catch (Exception ex)
            {
                if (ex is SshConnectionException || ex.ToString().Contains("connection") || ex.ToString().Contains("Client not connected"))
                {
                    return -1;
                }
                else
                {
                    Log.Error("SshConnect[SendCommand] : Error : " + ex);
                }
            }
            tempCounter = ++tempCounter;
           Log.Debug("Failed Counts : " + tempCounter);
           return 0;
        }
    }
}
