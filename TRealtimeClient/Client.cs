using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace TRealtimeClient
{
    public static class Client
    {
        /// <summary>
        /// logger instance
        /// </summary>
        private static readonly Logger.Logger _log = new Logger.Logger(typeof(Client));
        private static int _serverCount = 0;
        static List<string> _tRealtimeRestUrls;
        private static string _certProtocol;
        private static string _certPath;

        static Client()
        {
            try
            {
                _tRealtimeRestUrls = new List<string>();
                _tRealtimeRestUrls = ConfigurationManager.AppSettings["TRealtimeRestUrls"].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                _serverCount = _tRealtimeRestUrls.Count;
                _log.Info("TRealtimeServersCount= " + _serverCount);
            }
            catch (Exception ex)
            {
                _log.Error("Exception in Constructor.TRealtimeRestUrls", ex);
            }

            try
            {
                _certProtocol = System.Configuration.ConfigurationManager.AppSettings["RestCertificateProtocol"];
            }
            catch (Exception ex)
            {
                _log.Error("Exception in Constructor.CertProtocol", ex);
            }

            try
            {
                _certPath = System.Configuration.ConfigurationManager.AppSettings["RestCertificateFile"];
            }
            catch (Exception ex)
            {
                _log.Error("Exception in Constructor.CertPath", ex);
            }

            if (_serverCount == 0)
            {
                _log.Warn("Please add TRealtimeServerCount key and its value in configuration.appSettings");
            }
            else
            {
                _log.Info("TRealtimeServer count: " + _serverCount);
            }
        }

        /// <summary>
        /// To get the realtime data for all the skills
        /// </summary>
        /// <returns></returns>
        public static object GetRealtimeDataForAllSkills()
        {
            _log.Debug("GetRealtimeDataForAllSkills");
            try
            {
                return RestConnector(_tRealtimeRestUrls, "GetRealtimeDataForAllSkills");
            }
            catch (Exception ex)
            {
                _log.Error("Exception in GetRealtimeDataForAllSkills", ex);
            }
            return null;
        }
        public static object GetRealtimeDataForSkill(string skillId)
        {
            _log.Debug("GetRealtimeDataForAllSkills");
            try
            {
                return RestConnector(_tRealtimeRestUrls, $"GetRealtimeDataForSkill/{skillId}");
            }
            catch (Exception ex)
            {
                _log.Error("Exception in GetRealtimeDataForSkill", ex);
            }
            return null;
        }

        public static object GetSkillAndExtensions()
        {
            _log.Debug("GetSkillAndExtensions");
            try
            {
                return RestConnector(_tRealtimeRestUrls, "GetSkillAndExtensions");
            }
            catch (Exception ex)
            {
                _log.Error("Exception in GetSkillAndExtensions", ex);
            }
            return null;
        }

        /// <summary>
        /// To connect to rest api
        /// </summary>
        /// <returns></returns>
        public static object RestConnector(List<string> restUrls, string action)
        {
            _log.Debug("RestConnector");
            try
            {
                // Lets iterates with all the servers.
                foreach (string url in restUrls)
                {
                    var tryUrl = (url.EndsWith(@"/") ? url : url + @"/") + action;

                    _log.Info("[RestConnector]: Connecting to : " + tryUrl);
                    try
                    {
                        //create an instance of rest client
                        RestClient client = new RestClient(tryUrl);
                        // Bind Certitificate for https
                        if (tryUrl.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
                        {
                            CertificateBinding.BindCertificate(client, _certProtocol, _certPath);
                        }
                        //create an instance of rest request
                        var request = new RestRequest(Method.GET);
                        request.AddHeader("postman-token", "7f56a635-1347-2385-3b87-2af172957a19");
                        request.AddHeader("cache-control", "no-cache");
                        request.AddHeader("content-type", "application/json");
                        request.Timeout = 15000;
                        //execute the request
                        IRestResponse response = client.Execute(request);
                        //check if response is null
                        if (response == null)
                        {
                            _log.Warn("[RestConnector]: [" + tryUrl + " ]: null response from server");
                            return null;
                        }
                        //check if the response is ErrorException
                        if (response.ErrorException != null)
                        {
                            _log.Error("[RestConnector]: [" + tryUrl + " ]: " + response.ErrorException.Message);
                        }
                        else
                        {
                            var jsonObject = JToken.Parse(response.Content).ToString();
                            return jsonObject;
                        }
                    }
                    catch (Exception ex)
                    {
                        // track the server failure
                        _log.Error("[RestConnector]: " + ex);
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                _log.Error("[RestConnector]: ", ex);
            }
            return null;
        }
    }

    internal class CertificateBinding
    {
        /// <summary>
        /// Initialize the Logger for log4Net.
        /// </summary>
        private static readonly Logger.Logger Log = new Logger.Logger(typeof(CertificateBinding));

        /// <summary>
        /// Interpretoer Security Protocol
        /// </summary>
        /// <summary>
        /// Web sync Security Protocol
        /// </summary>
        /// <summary>
        /// Certificate Binding
        /// </summary>
        /// <param name="clientProxy"><see cref="SoapHttpClientProtocol"/></param>
        /// <param name="link">Certificate Path.</param>
        public static RestClient BindCertificate(RestClient clientProxy, string protocol, string certificatePath)
        {
            // If protool is not configured, just reuturn the 'clientProxy'
            if (protocol == "")
            {
                Log.Warn("[BindCertificate]: security protocol Not configured");
                return clientProxy;
            }

            Log.Debug("[BindCertificate]: certificate binding starts");

            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = InterpreterGetSecurityProtocolType(protocol);

            //search cerficates in folder
            string[] certificateFiles = Directory.GetFiles(certificatePath, "*.cer");
            if (certificateFiles.Length > 0)
            {
                foreach (string certFile in certificateFiles)
                {
                    Log.Info("[BindCertificate]: security certificate [binding] : " + certFile.ToString());
                    X509Certificate cert = new X509Certificate(certFile);

                    // lets print few certificate details for verifications.
                    Log.Info("Certificate Subject  : " + cert.Subject +
                        " | Issuer : " + cert.Issuer +
                        " | Format  :" + cert.GetFormat() +
                        " | Efffective Date : " + cert.GetEffectiveDateString() +
                        " | Expiry Date : " + cert.GetExpirationDateString()
                        );
                    cert.Import(certFile);
                    clientProxy.ClientCertificates = new X509CertificateCollection();
                    clientProxy.ClientCertificates.Add(cert);
                    //clientProxy.ClientCredentials.ClientCertificate.Certificate = new X509Certificate2(cert);
                    ServicePointManager.ServerCertificateValidationCallback =
                        delegate (Object obj, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
                        {
                            return (true);
                        };
                }
            }
            Log.Debug("[BindCertificate]: certificate binding ENDS");

            return clientProxy;
        }

        /// <summary>
        /// Get Interpreter<see cref="SecurityProtocolType"/>
        /// </summary>
        /// <returns><see cref="SecurityProtocolType"/></returns>
        private static SecurityProtocolType InterpreterGetSecurityProtocolType(string interpreterSecurityProtocolType)
        {
            // set default SecurityProtocolType to Tls12
            var securityType = SecurityProtocolType.Tls12;
            try
            {
                switch (interpreterSecurityProtocolType.ToLower())
                {
                    case "tls12":
                        securityType = SecurityProtocolType.Tls12;
                        break;

                    case "ssl3":
                        securityType = SecurityProtocolType.Ssl3;
                        break;

                    case "tls11":
                        securityType = SecurityProtocolType.Tls11;
                        break;

                    case "tls":
                        securityType = SecurityProtocolType.Tls;
                        break;

                    default:
                        securityType = SecurityProtocolType.Tls12;
                        break;
                }
                Log.Debug(" SecurityProtocol supports    :: " + ServicePointManager.SecurityProtocol);
                Log.Debug("Current SecurityProtocolType :: " + interpreterSecurityProtocolType.ToUpper());
                return securityType;
            }
            catch (Exception exception)
            {
                Log.Error("[GetSecurityProtocolType] : " + exception);
                return securityType;
            }
        }

        /// <summary>
        /// Get Interpreter<see cref="SecurityProtocolType"/>
        /// </summary>
        /// <returns><see cref="SecurityProtocolType"/></returns>
        private static SecurityProtocolType WebsyncGetSecurityProtocolType(string webSyncSecurityProtocolType)
        {
            // set default SecurityProtocolType to Tls12
            var securityType = SecurityProtocolType.Tls12;
            try
            {
                switch (webSyncSecurityProtocolType.ToLower())
                {
                    case "tls12":
                        securityType = SecurityProtocolType.Tls12;
                        break;

                    case "ssl3":
                        securityType = SecurityProtocolType.Ssl3;
                        break;

                    case "tls11":
                        securityType = SecurityProtocolType.Tls11;
                        break;

                    case "tls":
                        securityType = SecurityProtocolType.Tls;
                        break;

                    default:
                        securityType = SecurityProtocolType.Tls12;
                        break;
                }
                Log.Debug("SecurityProtocol supports    :: " + ServicePointManager.SecurityProtocol);
                Log.Debug("Current  SecurityProtocolType :: " + webSyncSecurityProtocolType.ToUpper());
                return securityType;
            }
            catch (Exception exception)
            {
                Log.Error("[GetSecurityProtocolType] : " + exception);
                return securityType;
            }
        }
    }
}
