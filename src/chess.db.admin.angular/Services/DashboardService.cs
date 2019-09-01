using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

        public async Task<DashboardModel> GetAsync()
        {
            var db = new DashboardModel
            {
                TotalGames = await _chessDb.Games.CountAsync(),
                TotalSites = await _chessDb.Sites.CountAsync(),
                TotalEvents = await _chessDb.Events.CountAsync(),
                TotalPlayers = await _chessDb.Players.CountAsync(),
                TopPlayers = MostFrequentPlayers()
            };

            return db;
        }

        public async Task<int> GetPlayersCountAsync()
            => await _chessDb.Players.CountAsync();

        public async Task<int> GetGamesCountAsync()
            => await _chessDb.Games.CountAsync();

        public async Task<int> GetEventsCountAsync()
            => await _chessDb.Events.CountAsync();

        public async Task<int> GetSitesCountAsync()
            => await _chessDb.Sites.CountAsync();

        public IEnumerable<TopXItem> MostFrequent(IEnumerable<string> dupeList)
        {
            return dupeList
                .GroupBy(s => s)
                .Where(g => g.Count() > 1)
                .OrderByDescending(g => g.Count()).ToList()
                .Select(i => new TopXItem {Name = i.Key, Count = i.Count()});


        }
        public IEnumerable<TopXItem> MostFrequentPlayers()
        {
            return MostFrequent(
                    _chessDb.Games
                        .Include(g => g.White)
                        .Select(g => g.White.Name).ToList()
                    .Concat(
                            _chessDb.Games
                                .Include(g => g.Black)
                                .Select(g => g.Black.Name).ToList())
                );
        }
    }
}