using CMDataCollector;
using System;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.ServiceProcess;

namespace TRealtimeData
{
    static class Program
    {

        /// <summary>
        /// Logger
        /// </summary>
        static Logger.Logger log = new Logger.Logger(typeof(Program));
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            TConfigLoader.TConfig.UpdateConfigFromTmc();
            try
            {
                ServicePointManager.ServerCertificateValidationCallback += new RemoteCertificateValidationCallback((object sender, X509Certificate certificate,
                    X509Chain chain, SslPolicyErrors sslPolicyErrors) => { return true; });

                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls | SecurityProtocolType.Ssl3;

                string securityProtocolType = System.Configuration.ConfigurationManager.AppSettings["SecurityProtocolType"].ToString();
                if (!string.IsNullOrWhiteSpace(securityProtocolType))
                {
                    foreach (var r in securityProtocolType.Split(new char[] { ',' }))
                    {
                        log.Debug($"SecurityProtocolType to apply from config {r.Trim()}");
                        SecurityProtocolType tp = SecurityProtocolType.Ssl3;
                        if (SecurityProtocolType.TryParse(r.Trim(), out tp))
                        {
                            ServicePointManager.SecurityProtocol |= tp;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                log.Debug("Error while setting security protocol : " + ex);
            }
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new TRealtimeService()
            };
            ServiceBase.Run(ServicesToRun);
            //TRealtimeService t = new TRealtimeService();
            //t.OnStart(null);
            //BCMSDashboardManager.Start();
            //Console.ReadLine();
        }
    }
}
