using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using CMDataCollector.Models;

namespace CMDataCollector.DataParser
{
    class Utils
    {
        /// <summary>
        /// Logger
        /// </summary>
        static Logger.Logger Log = new Logger.Logger(typeof(Utils));

        /// <summary>
        /// Formats raw CM data to readable format by seperating fields and data.
        /// </summary>
        /// <param name="result">raw CM data received</param>
        /// <returns>returns foramted CM data</returns>
        public static CMData ExtractCMData(string result)
        {
            try
            {
                Log.Debug("Utils[ExtractCMData]");
                List<string> fileds = new List<string>();
                List<string> values = new List<string>();

                if (!string.IsNullOrWhiteSpace(result))
                {
                    //split the result string into string array by line feed char
                    string[] resArray = result.Split(new char[] { '\n' });
                    if (resArray != null && resArray.Length > 0)
                    {
                        foreach (string line in resArray)
                        {
                            if (line.StartsWith("t"))
                            {
                                //end of record set
                                break;
                            }
                            else if (line.StartsWith("c"))
                            {
                                //command name
                            }
                            else if (line.StartsWith("e"))
                            {
                                //error
                                Log.Warn("CM Error:" + line);
                                return new Models.CMData()
                                {
                                    ErrorMessage = line,
                                    Success = false
                                };
                            }
                            else if (line.StartsWith("f"))
                            {
                                //field names
                                string[] fieldArray = line.Remove(0, 1).Split(new char[] { '\t' });
                                fileds.AddRange(fieldArray);
                            }
                            else if (line.StartsWith("d"))
                            {
                                //data
                                string[] valueArray = line.Remove(0, 1).Split(new char[] { '\t' });
                                values.AddRange(valueArray);
                            }

                        }
                    }
                }

                if (fileds.Count > 0 && values.Count > 0)
                {
                    return new Models.CMData()
                    {
                        ErrorMessage = "",
                        Fields = fileds,
                        Success = true,
                        Values = values
                    };
                }

                //Log.Warn("No data");
                //Log.Warn("RESULT:" + result);
                return new Models.CMData()
                {
                    ErrorMessage = "ERROR",
                    Success = false
                };
            }
            catch (Exception ex)
            {
                Log.Error("Exception in Utils[ExtractCMData]" + ex);
                return null;
            }
        }

        #region Not used

        /// <summary>
        /// Converts date to YMD format.
        /// </summary>
        /// <param name="strDate">date</param>
        /// <returns></returns>
        public static DateTime ConvertToDate(string strDate)
        {
            Log.Debug("Utils[ConvertToDate]");
            try
            {
                DateTime dateTime;
                if (DateTime.TryParseExact(strDate, "yyyyMMddHHmmss", CultureInfo.InvariantCulture,
                    DateTimeStyles.None, out dateTime))
                {
                    return dateTime;
                }
            }
            catch (Exception ex)
            {
                Log.Error("Exception in Utils[ConvertToDate]" + ex);
            }
            return DateTime.Now;
        }

        #endregion
    }    
}

