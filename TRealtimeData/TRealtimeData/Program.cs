using CMDataCollector;
using System;
using System.ServiceProcess;

namespace TRealtimeData
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            TConfigLoader.TConfig.UpdateConfigFromTmc();
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
