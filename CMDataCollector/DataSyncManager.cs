//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading;
//using System.Security.Cryptography.X509Certificates;
//using System.Net.Security;
//using System.Net;

//namespace CMDataCollector
//{
//    public class DataSyncManager
//    {
//         static Logger.Logger Log = new Logger.Logger(typeof(DataSyncManager));

//        /// <summary>
//        /// 
//        /// </summary>
//        public static void Start()
//        {
//            try
//            {
//                started = true;
//                IsAlternateServiceRunning = false;
//                Thread ValidateHB = new Thread(new ThreadStart(HeartBeatStatus));
//                ValidateHB.Start();
//            }
//            catch (Exception)
//            {
//                Log.Error("");
//            }
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        static void HeartBeatStatus()
//        {
//            Log.Debug("HeartBeatStatus()");

//            HB_BcmsService.BcmsDashboardService service = new HB_BcmsService.BcmsDashboardService();

//            try
//            {
//                // do a certificate validation
//                string certificationFile = System.Configuration.ConfigurationSettings.AppSettings["ServiceCertificate"];
//                if (certificationFile != null)
//                {
//                    System.Security.Cryptography.X509Certificates.X509Certificate cert = new System.Security.Cryptography.X509Certificates.X509Certificate(certificationFile);
//                    cert.Import(certificationFile);
//                    service.ClientCertificates.Add(cert);
//                    ServicePointManager.ServerCertificateValidationCallback =
//                        delegate(Object obj, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors) { return (true); };

//                }
//            }
//            catch (Exception ex)
//            {
//                Log.Error("Exception in HeartBeatStatus while validating service certificate :", ex);
//            }

//            bool isServiceRunning = false;
//            bool isRunningResultSpecified = false;

//            // keep checking the status of alternate server.
//            while (started)
//            {
//                try
//                {
//                    // gets the boolean output of alternate service status.
//                    service.IsRunning(out isServiceRunning, out isRunningResultSpecified);

//                    // if alternate service is up and running, do data sync
//                    if (isServiceRunning)
//                    {                      
//                        IsAlternateServiceRunning = true;
//                    }

//                    // if alternate service is not running
//                    if (!isServiceRunning)
//                    {
//                        //if (!CMConnectionManager.connectToCM)
//                        //    CMConnectionManager.GetInstance().Start();
//                    }
//                }
//                catch (Exception ex)
//                {
//                    IsAlternateServiceRunning = false;
//                    Log.Error("Exception in HeartBeatStatus while checking alternate service status :", ex);
//                }
//                Thread.Sleep(1000);
//            }
//        }
//    }
//}
