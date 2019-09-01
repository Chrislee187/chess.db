using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using chess.db.admin.angular.Services;
using chess.games.db.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace chess.db.admin.angular.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DashboardController : ControllerBase
    {

        private readonly ILogger<DashboardController> _logger;
        private readonly IDashboardService _dashboardService;

        public DashboardController(IDashboardService dashboardService, ILogger<DashboardController> logger)
        {
            _dashboardService = dashboardService;
            _logger = logger;
        }

        [HttpGet("playersdata")]
        public async Task<DashletData> GetPlayersCountAsync() =>
            new DashletData
            {
                TotalCount = await _dashboardService.GetPlayersCountAsync()
            };

        [HttpGet("eventsdata")]
        public async Task<DashletData> GetEventsCountAsync() =>
            new DashletData
            {
                TotalCount = await _dashboardService.GetEventsCountAsync()
            };

        [HttpGet("sitesdata")]
        public async Task<DashletData> GetSitesCountAsync() =>
            new DashletData
            {
                TotalCount = await _dashboardService.GetSitesCountAsync()
            };

        [HttpGet("gamesdata")]
        public async Task<DashletData> GetGamesCountAsync() =>
            new DashletData
            {
                TotalCount = await _dashboardService.GetGamesCountAsync()
            };

    }

    public class DashletData
    {
        public int TotalCount { get; set; }
    }
}
