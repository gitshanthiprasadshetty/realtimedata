using System;
using System.Collections.Generic;
using CMDataCollector.Models;

namespace CMDataCollector.DataParser
{
    class HuntGroupTrafficParser
    {
        static Logger.Logger log = new Logger.Logger(typeof(HuntGroupTrafficParser));

        public static void HuntTrafficParser(string result)
        {
            log.Debug("HuntTrafficParser()");
            try
            {
                Models.CMData data = Utils.ExtractCMData(result);
                if (data == null)
                {
                    log.Warn("Data is null");
                }
                else
                {
                    if (!data.Success)
                    {
                        log.Warn("data not success");
                    }
                }
                //List<HuntGroupTraffic> huntTraffic = null;
                // check if the response is successful
                if (data != null && data.Success)
                {
                    int cnt = 0;
                    int innerCnt = 0;
                    if (data.Fields != null && data.Fields.Count > 0)
                    {
                        //huntTraffic = new List<HuntGroupTraffic>();
                        string date = string.Empty;
                        if (data.Fields[0].StartsWith("0001ff"))
                            date = data.Values[cnt];

                        string callsWaiting = string.Empty;
                        string huntGroup = string.Empty;
                        string lCIQ = string.Empty;

                        data.Fields.RemoveAt(cnt);
                        data.Values.RemoveAt(cnt);
                        cnt = 0;
                        log.Debug("Total fields :" + data.Fields.Count);
                        log.Debug("Total values :" + data.Values.Count);
                        foreach (var item in data.Fields)
                        {
                            innerCnt++;
                            //log.Debug("count :" + cnt);
                            if (cnt >= data.Fields.Count)
                                break;

                            var val = data.Values[cnt];
                            //log.Debug("value = " + val);
                            if (item.StartsWith("0002ff"))
                                huntGroup = val;
                            if (item.StartsWith("0006ff"))                            
                                callsWaiting = val;
                            if (item.StartsWith("0007ff"))
                                lCIQ = val;

                            //log.Debug("huntGroup = " + huntGroup);

                            if (innerCnt == 6)
                            {
                                innerCnt = 0;

                                if (!string.IsNullOrEmpty(huntGroup))
                                {
                                    Utilities.CacheMemory.AddOrUpdateCacheMemory(new HuntGroupTraffic
                                    {
                                        CallsWaiting = callsWaiting,
                                        HuntGroup = huntGroup,
                                        LongestCIQ = lCIQ 
                                    });
                                }
                                huntGroup = string.Empty;
                                callsWaiting = string.Empty;
                                lCIQ = string.Empty;
                            }
                            cnt++;
                        }
                    }
                    log.Info("completed parsing");
                }
            }
            catch (Exception ex)
            {
                log.Error("Error in HuntTrafficParser : ", ex);
            }
        }
    }
}
