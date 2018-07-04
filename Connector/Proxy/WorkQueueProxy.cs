using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Connector.Models;

namespace Connector.Proxy
{
    public class WorkQueueProxy
    {
        /// <summary>
        /// Tmac WorkQ proxy
        /// </summary>
        static TMACWorkQ.ServiceClient workqProxy = new TMACWorkQ.ServiceClient();

        /// <summary>
        /// Logger
        /// </summary>
        private static readonly Logger.Logger Log = new Logger.Logger(typeof(WorkQueueProxy));

        /// <summary>
        /// Gets the total email queue count
        /// </summary>
        /// <param name="extensionId">email extensionId</param>
        /// <returns>returns total emails in queue for given skill</returns>
        public static string GetQueueCount(string extensionId)
        {
            try
            {
                Log.Debug("GetQueueCount()");
                // get total number of interactions waiting in queue for requested 'email' extension
                var result = workqProxy.GetQueueCount(extensionId);
                var jData = JsonConvert.DeserializeObject(result);
                // return ((Newtonsoft.Json.Linq.JObject)jData).Count.ToString();
                if (jData != null && ((JObject)jData).Count > 0)
                    return ((JValue)((JProperty)(((JContainer)jData).First)).Value).Value.ToString();
            }
            catch (Exception ex)
            {
                Log.Error("GetQueueCount : " + ex);
            }
            return "0";
        }

        /// <summary>
        /// Gets the oldest interaction time for a skill
        /// </summary>
        /// <param name="extensionId">email extensionId</param>
        /// <returns>Gets the oldest interaction time[in seconds] for a skill</returns>
        public static string GetOldestWaitTime(string extensionId)
        {
            try
            {
                Log.Debug("GetOldestWaitTime()");
                DateTime currentTime = DateTime.Now;

                // returns all the data in queue.
                var qDetails = workqProxy.GetQueueDetails(extensionId);

                var jData = JsonConvert.DeserializeObject<List<WorkQueueDetails>>(qDetails);

                //  var result = jData.FirstOrDefault(x => x.Skill == extenId).Items.Min(x => x.AddedTime);
                // string.Format("{0:s}.{0:fff}", dt)
                // var lastTime = string.Format("{0:s}", result);

                // find for the requested extension in result obtained.
                var result = jData.FirstOrDefault(x => x.Skill == extensionId);
                if (result != null)
                {
                    // if there were more than 1 mails waiting in queue
                    // for requested extensionid then get the minimum queue addedtime from the result list.
                    var value = result.Items.Min(x => x.AddedTime);

                    // parse it to proper datetime format.
                    DateTimeOffset minDatetime = DateTimeOffset.Parse(value);
                    DateTime dtObject = minDatetime.DateTime;

                    // calculate the timedifference between currentdatetime and mindatetime
                    TimeSpan timeDiff = currentTime.Subtract(dtObject);

                    // convert the timdiff into seconds and return.
                    return timeDiff.TotalSeconds.ToString();
                }
            }
            catch (Exception ex)
            {
                Log.Error("GetOldestWaitTime : " + ex);
            }
            return "0";
        }
    }
}
