using System;
using System.Collections.Generic;

namespace Connector.Models
{
    public class WorkQueueDetails
    {
        public string Skill { get; set; }

        public List<Items> Items { get; set; }
    }
}
