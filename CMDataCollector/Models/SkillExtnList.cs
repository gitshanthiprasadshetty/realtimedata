using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CMDataCollector.Models
{
    [DataContract]
    public class SkillExtnList
    {
        [DataMember]
        public string Extension { get; set; }
        [DataMember]
        public string Skill { get; set; }
    }
}
