using CMDataCollector;
using SIPDataCollector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TRealtimeData
{
    public interface ITRealtimeInterface
    {
        void GetInvokeAndHostType(string type);
    }
}
