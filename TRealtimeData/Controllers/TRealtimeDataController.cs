using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SIPDataCollector.Models;
using TRealtimeData.Model;

namespace TRealtimeData.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TRealtimeDataController : ControllerBase
    {
        private static Logger.Logger log = new Logger.Logger(typeof(TRealtimeDataController));
        private readonly IOptions<MySettingsModel> appSettings;
        private readonly IOptions<TRealTimeDataServiceSettings> appServiceSettings;
        public TRealtimeDataController(IOptions<MySettingsModel> app,IOptions<TRealTimeDataServiceSettings> appSet)
        {
            appSettings = app;
            appServiceSettings = appSet;
            ApplicationSettings.CMDbConn = appSettings.Value.CMDbConn;
            ApplicationSettings.acceptableSL = appSettings.Value.acceptableSL;
            ApplicationSettings.DashboardRefreshTime = appSettings.Value.DashboardRefreshTime;
            ApplicationSettings.DBRefreshTime = appSettings.Value.DBRefreshTime;
            ApplicationSettings.ExtraAUXCodes = appSettings.Value.ExtraAUXCodes;
            ApplicationSettings.GetAgentSkillInfo = appSettings.Value.GetAgentSkillInfo;
            ApplicationSettings.ReloadConfigTime = appSettings.Value.ReloadConfigTime;
            ApplicationSettings.TmacServers = appSettings.Value.TmacServers;
            ApplicationSettings.isVDNMonitor = appSettings.Value.isVDNMonitor;
            ApplicationSettings.channelName = appServiceSettings.Value.channelName;

        }
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        //private readonly ILogger<TRealtimeDataController> _logger;

        //public TRealtimeDataController(ILogger<TRealtimeDataController> logger)
        //{
        //    _logger = logger;
        //}

        [HttpGet]
        public IEnumerable<TRealtimeData> Get()
        {
            SIPDataCollector.SIPManager.GetInstance().Start();
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new TRealtimeData
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
        }
        [HttpGet("{id}")]
        public List<RealtimeData> GetBcmsData()
        {
            return SIPDataCollector.SIPManager.GetInstance().GetBcmsData();
        }
    }
}
