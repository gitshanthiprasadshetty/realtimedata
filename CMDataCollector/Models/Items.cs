using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMDataCollector.Models
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class Items
    {
        [JsonProperty(PropertyName = "ItemID")]
        public string ItemId { get; set; }

        [JsonProperty(PropertyName = "Key")]
        public string Key { get; set; }

        [JsonProperty(PropertyName = "OrderIndex")]
        public string OrderIndex { get; set; }

        [JsonProperty(PropertyName = "Channel")]
        public string Channel { get; set; }

        [JsonProperty(PropertyName = "Skill")]
        public string Skill { get; set; }

        [JsonProperty(PropertyName = "Data")]
        public string Data { get; set; }

        [JsonProperty(PropertyName = "AddedTime")]
        public string AddedTime { get; set; }

        [JsonProperty(PropertyName = "CreatedBy")]
        public string CreatedBy { get; set; }

        [JsonProperty(PropertyName = "SubChannel")]
        public string SubChannel { get; set; }

        [JsonProperty(PropertyName = "Status")]
        public string Status { get; set; }

        [JsonProperty(PropertyName = "RouteDate")]
        public string RouteDate { get; set; }

        [JsonProperty(PropertyName = "RouteTime")]
        public string RouteTime { get; set; }

        [JsonProperty(PropertyName = "AgentID")]
        public string AgentID { get; set; }

        [JsonProperty(PropertyName = "Reason")]
        public string Reason { get; set; }

        [JsonProperty(PropertyName = "CustomerIdentifier")]
        public string CustomerIdentifier { get; set; }
    }
}
