using System;
using System.Collections.Generic;
using System.Linq;
using CMDataCollector.Connection;
using System.Threading;
using CMDataCollector.Utilities;

namespace CMDataCollector
{
    public class CMConnectionManager
    {
        #region Global Declarations

        /// <summary>
        /// Holds value based on current service connectivity to CM
        /// </summary>
        public bool connectToCM;

        /// <summary>
        /// log Defination
        /// </summary>
        private static readonly Logger.Logger log = new Logger.Logger(typeof(CMConnectionManager));

        /// <summary>
        /// Instance of an Cache mem class
        /// </summary>
        readonly CacheMemory _listBcmsObj = new CacheMemory();

        /// <summary>
        /// Dictionary object to store total number of connections established to CM
        /// </summary>
        static readonly Dictionary<string, CMConnection> CM = new Dictionary<string, CMConnection>();

        /// <summary>
        /// made true when first request is sent from client side, 
        /// based on this variable value looping/refresh of cache is done.
        /// </summary>
        public  bool IsUpdateStarted;

        /// <summary>
        /// 
        /// </summary>
        public bool isBcmsEnabled;

        /// <summary>
        /// 
        /// </summary>
        public bool isTrafficEnabled;

        /// <summary>
        /// 
        /// </summary>
        public bool isTrunkEnabled;

        /// <summary>
        /// 
        /// </summary>
        public bool isHuntEnabled;

        /// <summary>
        /// 
        /// </summary>
        public bool isSystemEnabled;

        #endregion

        #region Singleton

        /// <summary>
        /// singleton instance of class
        /// </summary>
        private static CMConnectionManager _instance;

        /// <summary>
        /// 
        /// </summary>
        /// <returns>Returns existing instance or new instance of this class</returns>
        public static CMConnectionManager GetInstance()
        {
            if (_instance == null)
                _instance = new CMConnectionManager();

            return _instance;
        }

        #endregion

        #region Methods

        #region Private Methods

        /// <summary>
        /// Constructor
        /// </summary>
        private CMConnectionManager()
        {
        }

        /// <summary>
        /// Create an instance of connection
        /// </summary>
        static void CreateConnectionToCm()
        {
            log.Debug("[CreateConnectionToCm]");
            try
            {
                if (CM != null && CM.Count > 0)
                {
                    foreach (var entry in CM)
                    {
                        //var con = CMConnection.CreateInstance(entry.Key, entry.Value._skillRange);
                        var entry1 = entry;
                        var s = new Thread(new ThreadStart(delegate
                        {
                        log.Debug("Open Connection : " + entry1.Key);
                        var con = entry1.Value;
                        con.State = new ConnectionNotEstablishedState(entry1.Key);
                        con.ConnectionStateStatus = "ConnectionNotEstablishedState";
                        con.State.Connect();
                        con.DataReceived = "";
                        }));
                        s.Start();
                        Thread.Sleep(10000);
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error("Error in [CreateConnectionToCm] : " + ex);
            }
        }

        /// <summary>
        /// Find and get the connection key for given skillid
        /// </summary>
        /// <param name="skillId">skillid</param>
        /// <returns>returns cm conection key value for given skillid</returns>
        private static string GetConnectionKeyForSkill(string skillId)
        {
            log.Debug("[GetConnectionKeyForSkill] " + skillId);
            try
            {
                var connectionKey = CM.FirstOrDefault(x => x.Value.SkillRange.Contains(skillId)).Key;
                log.Debug("[GetConnectionKeyForSkill] connectionKey" + connectionKey);
                return connectionKey;
            }
            catch (Exception ex)
            {
                log.Error("Error in [GetConnectionKeyForSkill] : " + ex);
                return null;
            }
        }

        List<int> processedHuntGroups = null;
        int? GenerateNextHuntStartValue()
        {
            log.Debug("GenerateNextHuntStartValue()");
            try
            {
                // huntgroup numbers to be monitored
                List<int> values = ConfigurationData.HuntGroups.ToList();

                // last processedvalue 
                int lowerBoundaryValue = processedHuntGroups.Last() + 32;

                // get next min huntgroup from huntgroupnumbers
                if (lowerBoundaryValue <= values.Max())
                    return values?.Where(x => x > lowerBoundaryValue).Min();
            }
            catch (Exception ex)
            {
                log.Error("Error in GenerateNextHuntStartValue : ", ex);
            }
            return -1;
        }

        /// <summary>
        /// Returns list of data to be put under each connection 
        /// </summary>
        /// <param name="totalConn">max connection that can be established to cm</param>
        /// <param name="huntStartValue">start value of hunt traffic</param>
        /// <returns>returns bundled list to caller</returns>
        List<List<string>> GetHuntBatchs(int totalConn, int huntStartValue)
        {
            try
            {
                log.Debug("GetHuntBatchs()");
                int cnt = 1;
                processedHuntGroups = new List<int>();
                processedHuntGroups.Add(huntStartValue);

                List<string> batchData = new List<string>();
                batchData.Add(huntStartValue.ToString());
                // total list of skills startvalues; since one page contains data of 32 skills we 
                // need to form logic to have all initial skillvalues in the list. i.e. hunt startvalues.
                List<List<string>> listobj = new List<List<string>>();

                // map skills to each connection keeping total connections to establish in mind.
                for (int i = 0; i < totalConn; i++)
                {                    
                    do
                    {
                        huntStartValue = GenerateNextHuntStartValue() ?? -1;
                        if (huntStartValue == -1)
                            break;

                        processedHuntGroups.Add(huntStartValue);
                        batchData.Add(huntStartValue.ToString());
                        cnt++;
                    } while (cnt < ConfigurationData.HuntFrequency);

                    if (batchData.Count > 0)
                        listobj.Add(batchData);

                    batchData = new List<string>();
                    cnt = 0;
                }
                return listobj;
            }
            catch (Exception ex)
            {
                log.Error("Error in GetHuntBatchs : ", ex);
                return null;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Start method to start connection to CM
        /// </summary>
        public void Start()
        {
            log.Debug("[start]");
            try
            {
                // change flag status to true.
                connectToCM = true;

                //
                IsUpdateStarted = false;

                // initialize the starting connection key
                int noOfConn = 1;

                // test 
                //ConfigurationData.LoadConfig();
                // get total number of connections to be established to cm based on skills.
                var totalConnCount = ConfigurationData.GetNumberOfConnectionsToEstablish();
                if (totalConnCount <= 0)
                {
                    log.Debug("[start] : NumberOfConnectionsToEstablish" + totalConnCount);
                    return;
                }
                // check if monitor bcms command to be executed.
                if (ConfigurationData.CommandType.ToLower() == "bcms")
                {
                    log.Info("Monitor bcms is enabled");
                    IsUpdateStarted = false;
                  
                    log.Debug("[start] : NumberOfConnectionsToEstablish" + totalConnCount);

                    // start of skill index assigned as zero
                    int skillStartIndex = 0;

                    // get from config , total no of skills that can be taken in each connection.
                    int tempMaxSkillsPerConn = ConfigurationData.skillsPerConnection;
                    int maxSkillsPerConn = tempMaxSkillsPerConn;
                    // get all skills tobe monitored from config.
                    var skillList = ConfigurationData.skillList;

                    log.Debug("[start] Total skillList Count:" + skillList.Count());
                    for (noOfConn = 1; noOfConn <= totalConnCount; noOfConn++)
                    {
                        var skillRange = new List<string>();
                        for (int skillIndex = skillStartIndex; skillIndex < maxSkillsPerConn; skillIndex++)
                        {
                            if (skillIndex < skillList.Count())
                            {
                                skillRange.Add(skillList[skillIndex]);
                                // keep incrementing start index value
                                skillStartIndex++;
                            }
                        }

                        // update max-skillsPerconnection value to next level
                        maxSkillsPerConn += tempMaxSkillsPerConn;

                        CM.Add(noOfConn.ToString(), new CMConnection(noOfConn.ToString(), skillRange));
                        log.Debug("[start] _cm dictionary key : " + noOfConn);
                        foreach (var entry in skillRange)
                        {
                            log.Debug("[start] _cm dictionary values[Skills] : " + entry);
                        }
                    }


                    // check if we need to monitor 'system data' as well
                    isSystemEnabled = ConfigurationData.CommandsToRun.Contains("system");
                    if (isSystemEnabled)
                    {
                        log.Debug("System Enabled");
                        int key = noOfConn + 1;
                        CM.Add(key.ToString(), new CMConnection(key.ToString(), new List<string> { "system" }));
                    }

                    // start monitoring bcms historical report
                    var bcmsReport = new Thread(new ThreadStart(delegate
                    {
                        log.Debug("[start] ListBcmsSkill");
                        _listBcmsObj.ListBcmsSkill();
                    }));
                    bcmsReport.Start();
                }

                if (ConfigurationData.CommandType.ToLower() == "traffic")
                {
                    log.Info("Monitor traffic is enabled.");
                    // check if we need to monitor 'trunk traffic' as well
                    isHuntEnabled = ConfigurationData.CommandsToRun.Contains("hunt");
                    if (isHuntEnabled)
                    {
                        log.Debug("hunt enabled");

                        // there two below methods are used when skillsperconnection frequency is not set. else use frequ
                        int huntStartValue = ConfigurationData.HuntGroups.Min();
                        List<List<string>> obj = GetHuntBatchs(totalConnCount, huntStartValue);

                        if (obj != null)
                        {
                            log.Debug("Total Connections established : " + obj.Count);
                            for (int i = 0; i < obj.Count; i++)
                            {
                                CM.Add((i + 1).ToString(), new CMConnection((i + 1).ToString(), obj[i]));
                            }
                        }
                    }
                    
                    // check if we need to monitor 'trunk traffic' as well
                    isTrunkEnabled = ConfigurationData.CommandsToRun.Contains("trunk");
                    if (isTrunkEnabled)
                    {
                        log.Debug("trunk enabled");
                        // since we are establishing only one connection to CM if trunk is to be monitored, so
                        // take next followed connectionkey as a connection key to trunk.
                        int key = noOfConn;
                        // instead of skillRange adding "trunk" as a string just to filter/get proper connectionkey
                        // in further states.
                        CM.Add(key.ToString(), new CMConnection(key.ToString(), new List<string> { "trunk" }));
                    }
                }

                CreateConnectionToCm();
            }
            catch (Exception ex)
            {
                log.Error("Error in [start] : " + ex);
            }
        }

        /// <summary>
        /// Get the connecction key value, for given connectionid
        /// </summary>
        /// <param name="key">connectionid of connection class</param>
        /// <returns>returns cmconnection class object for given connectionid</returns>
        public CMConnection GetConnectionValue(string key)
        {
            log.Debug("[GetConnectionValue]");
            try
            {
                if (CM.ContainsKey(key))
                {
                    var value = CM.FirstOrDefault(x => x.Key == key).Value;
                    return value;
                }
            }
            catch (Exception ex)
            {
                log.Error("Error in [GetConnectionValue] : " + ex);
            }
            return null;
        }

        /// <summary>
        /// Called first iff user/client had requested for bcms data atleast once. Used to update the bcms data by polling from CM.
        /// 
        /// 10-03-2017 : If we call this method for trunk and if user has not enabled to monitor bcms, then this method will try to connect to cm
        /// for bcms skills too which is wrong, so will keep this method specific to bcms update only by skipping connection related to monitor trunk.
        /// </summary>
        public void UpdateDashboard()
        {
            log.Debug("[UpdateDashboard]");
            try
            {
                // bool isUpdateStarted = true;
                // 10-03-2017 : since this cache returns only count for bcms.               
                log.Debug("UpdateDashboard : All connections are created and updated to cache, now keep updating.");
                while (true)
                {
                    log.Debug("UpdateDashboard");

                    // commenting below getcachecount method. Assuming CM connection will be establsihed for all
                    // requested skills. If some wrong skill-id is passed then cachecount will be always false and 
                    // Update never happens.

                    //bool result = CacheMemory.GetCacheCount();
                    //log.Debug("CMConnectionManager[UpdateDashboard] Is all given skills Monitored once" + result);
                    //if (result)
                    //{
                        // implies all the skills are monitored once, now try updating
                        // 10-03-2017 : since we have trunk related key also in this 'CM' object , 
                        // either we need to skip this connection from executing to maintain bcms execution seperatly or 
                        // need to handle.
                        log.Debug("Total count in CM : " + CM.Count);
                        foreach (var entry in CM)
                        {
                            //// skip if current CMconnection is to monitor trunk/system.
                            //// this will be handled seperately keeping this method specific only to BCMS part.
                            //if (entry.Value.SkillRange.Contains("trunk") || entry.Value.SkillRange.Contains("system"))
                            //{
                            //    continue;
                            //}
                            var con = entry.Value;
                            // if current connection state status is not commandstate 
                            // then terminal connection state has not happend. So cant execute commands,
                            // wait for some time.
                            log.Debug("[UpdateDashboard] ConnectionStateStatus: " + con.ConnectionStateStatus);
                            if (con.ConnectionStateStatus == "CMCommandState")
                            {
                                log.Debug("[UpdateDashboard] Run Execute command for cm connection key = " + entry.Key);
                                con.State = new CMCommandState(entry.Key);
                                con.State.ExecuteCommand();
                            }
                            // if(counter>10){ // try next state}
                        }
                        // increment counter here and check for this to take next state step.
                        // counter++
                //    }
                    Thread.Sleep(ConfigurationData.DashboardRefreshTime);
                }
            }
            catch (Exception ex)
            {
                log.Error("Error in [UpdateDashboard] : " + ex);
            }    
        }

        /// <summary>
        /// Method is called only when we need to get bcms data for a skillid[mostly in failure cases.]
        /// </summary>
        /// <param name="skillId"></param>
        public void ExecuteCommandForSkill(string skillId)
        {
            log.Debug("[ExecuteCommandForSkill] " + skillId);
            try
            {
                // get the CM key for skillid given
                var key = GetConnectionKeyForSkill(skillId);
                if (key != null)
                {
                    // get the CMconnection value using key from dictionary.
                    var connectionValue = CM.FirstOrDefault(x => x.Key == key).Value;
                    var con = connectionValue;
                    log.Debug("[ExecuteCommandForSkill] ConnectionStateStatus: " + con.ConnectionStateStatus);
                    if (con.ConnectionStateStatus == "CMCommandState")
                    {
                        // this will again do execute for all skillrange present in that CMConnection object instead for a single skillid.
                        con.State = new CMCommandState(key);
                        con.State.ExecuteCommand();
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error("Error in [ExecuteCommandForSkill] : " + ex);
            }
        }

        #endregion

        #endregion

        #region not used

        public void GetOrCreateConnection(string key)
        {
            //if (!_cm.ContainsKey(key))
            //{
            //   // _cm.Add(key, new CMConnection.CMConnection());
            //}
        }

        public static string GenerateUniqueKey()
        {
            return Guid.NewGuid().ToString();
        }

        /// <summary>
        /// service debug mode
        /// </summary>
        public void OnDebug()
        {
            Start();
        }

        #endregion
    }
}
