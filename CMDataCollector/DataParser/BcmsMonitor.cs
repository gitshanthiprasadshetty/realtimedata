using System;
using System.Collections.Generic;
using System.Reflection;
using CMDataCollector.Models;

namespace CMDataCollector.DataParser
{
    public class BcmsMonitor 
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

        public List<AgentData> AgentData { get; set; }

        /// <summary>
        /// Logger 
        /// </summary>
        static Logger.Logger Log = new Logger.Logger(typeof(BcmsMonitor));

        /// <summary>
        /// Formats the result CM data obtained
        /// </summary>
        /// <param name="result">CM data</param>
        /// <returns>returns value for each properites defined in this class</returns>
        public  BcmsMonitor Monitor(string result)
        {
            try
            {
                Log.Debug("Monitor()");

                // result = System.IO.File.ReadAllText(@"D:\Tetherfi_Projects\SVN\bcms.txt");
                // extract receveid data into fields and values arrays
                Models.CMData data = Utils.ExtractCMData(result);

                Log.Debug("Completed Extracting Data");
                //System.IO.File.WriteAllLines("fs.txt",data.Fields);
                //System.IO.File.WriteAllLines("d1.txt", data.Values);

                //check if the response is successful
                if (data != null && data.Success)
                {
                    Log.Debug("got data");
                    //check if we have atleast one field
                    if (data.Fields != null && data.Fields.Count > 0)
                    {
                        int fieldPosition = 0;

                        //find the positions of each field. This will help to get the exact locaitons for values
                        //Map the model names with UID 
                        var fieldPositionMap = new Dictionary<string, int>();
                        foreach (string field in data.Fields)
                        {
                            string typeField = "";
                            switch (field)
                            {
                                case "0002ff00"://Skill
                                    typeField = "Skill";
                                    break;

                                case "0004ff00"://Date
                                    typeField = "Date";
                                    break;

                                case "0003ff00"://SkillName
                                    typeField = "SkillName";
                                    break;

                                case "0005ff00"://CallsWaiting
                                    typeField = "CallsWaiting";
                                    break;

                                case "07d1ff00"://AccptedSL
                                    typeField = "AccptedSL";
                                    break;

                                case "0006ff00"://OldestCall
                                    typeField = "OldestCall";
                                    break;

                                case "07d2ff00"://SL
                                    typeField = "SL";
                                    break;

                                case "0007ff00"://Staff
                                    typeField = "Staff";
                                    break;

                                case "0008ff00"://Avail
                                    typeField = "Avail";
                                    break;

                                case "0009ff00"://ACD
                                    typeField = "ACD";
                                    break;

                                case "000aff00"://ACW
                                    typeField = "ACW";
                                    break;

                                case "3e81ff00"://AUX
                                    typeField = "AUX";
                                    break;

                                case "000bff00"://Extn
                                    typeField = "Extn";
                                    break;

                                case "000cff00"://Other
                                    typeField = "Other";
                                    break;

                                default:
                                    fieldPosition--;
                                    break;
                            }
                            if (!string.IsNullOrWhiteSpace(typeField))
                                fieldPositionMap.Add(typeField, fieldPosition);
                            fieldPosition++;
                        }

                        //list object to hold the final set of staitons
                        //List<BcmsMonitor> monList = new List<BcmsMonitor>();

                        Log.Debug("field position done");
                        //station type object
                        var monBcms = new BcmsMonitor();
                        monBcms.AgentData = new List<AgentData>(); 
                        //get all the properties for this object
                        PropertyInfo[] props = typeof(BcmsMonitor).GetProperties();

                        //travers the value array and assign the values to station object
                        for (int x = 0; x < data.Values.Count; x = x + data.Fields.Count)
                        {
                            //assign all parameters one by one
                            foreach (PropertyInfo prop in props)
                            {
                                Log.Debug("assigning all parameters one by one");
                                //check if our field position map contains this property
                                //then set the value
                                if (fieldPositionMap.ContainsKey(prop.Name))
                                {
                                    try
                                    {
                                        //read the value from corrosponding position
                                        prop.SetValue(monBcms, data.Values[x + fieldPositionMap[prop.Name]], null);
                                    }
                                    catch (Exception ex)
                                    {
                                        Log.Error("BcmsMonitor[fieldPositionMap] : " + ex);
                                    }
                                }
                                if (prop.Name == "AgentData")
                                {
                                    // int acounter = 1;
                                    Log.Debug("AgentData data count = : " + data.Fields.Count);
                                    for (int z = 15; z < data.Fields.Count; z = z + 8)
                                    {
                                        Log.Debug("AgentData current index value = " + z);
                                        // add agentdata to agentdata list
                                        if (!string.IsNullOrWhiteSpace(data.Values[z]))
                                        {
                                            string agentLoginId = data.Values[z];
                                            if (!string.IsNullOrWhiteSpace(data.Values[z]))
                                            {
                                                string status = data.Values[z + 2];
                                                monBcms.AgentData.Add(new AgentData { LoginId = agentLoginId, State = status });
                                                prop.SetValue(monBcms, monBcms.AgentData, null);
                                            }
                                        }
                                    }
                                    Log.Debug("Total Agent Count returning :" + monBcms.AgentData.Count);
                                }
                            }

                            return monBcms;
                        }
                        return null;
                    }
                }              
            }
            catch (Exception ex)
            {
                Log.Error("Exception in Monitor", ex);
            }
            return null;
        }
    }
}
