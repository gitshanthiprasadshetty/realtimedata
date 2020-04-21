using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMDataCollector.Models
{
    class CMData
    {
        /// <summary>
        /// Field Names
        /// </summary>
        public List<string> Fields { get; set; }

        /// <summary>
        /// Field Values/Data
        /// </summary>
        public List<string> Values { get; set; }

        /// <summary>
        /// Error message to show
        /// </summary>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// boolean to refer success/failure.
        /// </summary>
        public bool Success { get; set; }
    }
}
