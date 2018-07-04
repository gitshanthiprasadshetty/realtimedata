using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace CMDataCollector.Models
{
    [DataContract]
    public class TrunkGroupTraffic
    {
        /// <summary>
        /// Trunk group id
        /// </summary>
        [DataMember]
        private int trunkGroup;

        [DataMember]
        public int TrunkGroup
        {
            get { return trunkGroup; }
            set { trunkGroup = value; }
        }

        /// <summary>
        /// Size of trunk
        /// </summary>
        [DataMember]
        public int trunkGroupSize;

        [DataMember]
        public int TrunkGroupSize
        {
            get { return trunkGroupSize; }
            set { trunkGroupSize = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public int activeMembers;


        [DataMember]
        public int ActiveMembers
        {
            get { return activeMembers; }
            set { activeMembers = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public int queueLength;

        [DataMember]
        public int QueueLength
        {
            get { return queueLength; }
            set { queueLength = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public int callsWaiting;

        [DataMember]
        public int CallsWaiting
        {
            get { return callsWaiting; }
            set { callsWaiting = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string date { get; set; }

        [DataMember]
        public string Date
        {
            get { return date; }
            set { date = value; }
        }
    }
}
