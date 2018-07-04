﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CMDataCollector.Models
{
    /// <summary>
    /// used only for load testing
    /// </summary>
    [Serializable]
    public class LoadTest
    {
        public string Skill { get; set; }
        public string Date { get; set; }
        public string SkillName { get; set; }
        public string CallsWaiting { get; set; }
        public string AccptedSL { get; set; }
        public string OldestCall { get; set; }
        public string SL { get; set; }

        public string Staff { get; set; }
        public string Avail { get; set; }
        public string ACD { get; set; }
        public string ACW { get; set; }
        public string AUX { get; set; }
        public string Extn { get; set; }
        public string Other { get; set; }
    }
}
