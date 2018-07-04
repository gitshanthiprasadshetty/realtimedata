using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMDataCollector.Models
{
    public class WorkQueueDetails
    {
        public string Skill { get; set; }

        public List<Items> Items { get; set; }
    }
}
