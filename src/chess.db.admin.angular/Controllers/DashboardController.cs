using System.Threading.Tasks;
using chess.db.admin.angular.Services;
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
        public async Task<DashletData> GetPlayersDataAsync() =>
            new DashletData
            {
                TotalCount = await _dashboardService.GetPlayersCountAsync()
            };

        [HttpGet("eventsdata")]
        public async Task<DashletData> GetEventsDataAsync() =>
            new DashletData
            {
                TotalCount = await _dashboardService.GetEventsCountAsync()
            };

        [HttpGet("sitesdata")]
        public async Task<DashletData> GetSitesDataAsync() =>
            new DashletData
            {
                TotalCount = await _dashboardService.GetSitesCountAsync()
            };

        [HttpGet("gamesdata")]
        public async Task<DashletData> GetGamesDataAsync() =>
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
