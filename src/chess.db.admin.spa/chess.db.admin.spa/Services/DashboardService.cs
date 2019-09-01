using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using chess.games.db.Entities;

namespace chess.db.admin.web.Services
{
    public class DashboardService : IDashboardService
    {
        private ChessGamesDbContext _context;

        public DashboardService(ChessGamesDbContext context)
        {
            _context = context;
        }

        private IEnumerable<Game> _gameCache = null;
        public async Task<IEnumerable<Game>> GetGamesAsync() 
            => _gameCache 
               ?? (_gameCache = await Task.Run(() => _context.GamesWithIncludes())).ToList();

        public async Task<IEnumerable<Site>> GetSitesAsync() 
            => await Task.Run(() => _context.Sites);

        public async Task<int> GamesCount()
        {
            return _context.Games.Count();
        }
    }

    public interface IDashboardService
    {
        Task<IEnumerable<Game>> GetGamesAsync();
        Task<IEnumerable<Site>> GetSitesAsync();
        Task<int> GamesCount();
    }
}