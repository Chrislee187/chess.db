using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using chess.db.admin.web.Services;
using chess.games.db.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace chess.db.admin.web.Pages
{
    public class DashboardModel : PageModel
    {
        private readonly ILogger<DashboardModel> _logger;
        private IDashboardService _dashboardService;

        public IEnumerable<Game> Games { get; set; }

        [BindProperty]
        public int GameCount { get; set; }
        public DashboardModel(
            ILogger<DashboardModel> logger,
            IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
            _logger = logger;


        }

        public async Task OnGetAsync()
        {
            var games = await _dashboardService.GetGamesAsync();
            GameCount = games.Count();
        }

    }
}