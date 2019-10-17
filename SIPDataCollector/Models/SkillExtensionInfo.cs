using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIPDataCollector.Models
{
    /// <summary>
    /// Holds skill-extension information[one-one mapping]
    /// </summary>
    public class SkillExtensionInfo
    {
        /// <summary>
        /// Group ExtensionId
        /// </summary>
        public int ExtensionId { get; set; }
        
        /// <summary>
        /// SkillId for ExtensionID
        /// </summary>
        public int SkillId { get; set; }

        /// <summary>
        /// SkillName for SkillId
        /// </summary>
        public string SkillName { get; set; }
        public string Channel { get; set; }
    }
}
