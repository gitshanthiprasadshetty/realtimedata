using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TRealtimeData
{
    public class RealtimeFactory 
    {
        private static Logger.Logger log = new Logger.Logger(typeof(RealtimeFactory));
        public static ITRealtimeInterface GetDataAccessObj()
        {
            log.Info("Interface GetDataAccessObj");
            return new RealtimeAccess();
        }
    }
}