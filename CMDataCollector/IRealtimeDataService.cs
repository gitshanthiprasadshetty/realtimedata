using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using CMDataCollector.Models;
using System.Runtime.Serialization;

namespace CMDataCollector
{
    [ServiceContract]
    public interface IRealtimeDataService
    {
        [OperationContract]
        List<BcmsDashboard> MonitorBcms();

        [OperationContract]
        BcmsDashboard MonitorBcmsForSkill(string skillToMonitor);

        [OperationContract]
        List<TrunkGroupTraffic> MonitorTrunkTraffic();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="skillName"></param>
        /// <returns></returns>
        [OperationContract]
        List<BcmsSystem> MonitorBcmsSystem(string skillName);

        [OperationContract]
        bool IsRunning();

        [OperationContract]
        bool IsConnectedToCM();
    }
}
