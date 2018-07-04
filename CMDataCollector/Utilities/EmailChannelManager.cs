using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Runtime.Serialization.Json;
using CMDataCollector.Models;

namespace CMDataCollector.Utilities
{
    public class EmailChannelManager
    {
        /// <summary>
        /// Tmac WorkQ proxy
        /// </summary>
        static TmacWorkQ.ServiceClient tWorkQ = new TmacWorkQ.ServiceClient();

        /// <summary>
        /// Logger
        /// </summary>
        private static readonly Logger.Logger Log = new Logger.Logger(typeof(EmailChannelManager));

        /// <summary>
        /// Gets the total email queue count
        /// </summary>
        /// <param name="skillId">skillId</param>
        /// <returns>returns total emails in queue for given skill</returns>
        public static string GetQueueCount(string skillId)
        {
            try
            {

                var result = tWorkQ.GetQueueCount(ConfigurationData.GetExtensionId(skillId));
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
        /// <param name="skillId">skillid</param>
        /// <returns>Gets the oldest interaction time for a skill</returns>
        public static string GetOldestWaitTime(string skillId)
        {
            try
            {
                DateTime currentTime = DateTime.Now;
                var extenId = ConfigurationData.GetExtensionId(skillId);
                var qDetails = tWorkQ.GetQueueDetails(extenId);

                var jData = JsonConvert.DeserializeObject<List<WorkQueueDetails>>(qDetails);

                //  var result = jData.FirstOrDefault(x => x.Skill == extenId).Items.Min(x => x.AddedTime);
                // string.Format("{0:s}.{0:fff}", dt)
                // var lastTime = string.Format("{0:s}", result);

                var result = jData.FirstOrDefault(x => x.Skill == extenId);
                if (result != null)
                {
                    var value = result.Items.Min(x => x.AddedTime);
                    DateTimeOffset dto = DateTimeOffset.Parse(value);
                    DateTime dtObject = dto.DateTime;
                    TimeSpan timeDiff = currentTime.Subtract(dtObject);
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
