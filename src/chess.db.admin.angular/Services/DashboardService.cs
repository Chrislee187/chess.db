using System.Threading.Tasks;
using chess.games.db.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace chess.db.admin.angular.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly ChessGamesDbContext _chessDb;
        private readonly ILogger<DashboardService> _logger;

        public DashboardService(ChessGamesDbContext chessDb, ILogger<DashboardService> logger)
        {
            _chessDb = chessDb;
            _logger = logger;
        }

        public async Task<int> GetPlayersCountAsync()
            => await _chessDb.PgnPlayers.CountAsync();

        public async Task<int> GetGamesCountAsync()
            => await _chessDb.Games.CountAsync();

        public async Task<int> GetEventsCountAsync()
            => await _chessDb.Events.CountAsync();

        public async Task<int> GetSitesCountAsync()
            => await _chessDb.Sites.CountAsync();

    }
}