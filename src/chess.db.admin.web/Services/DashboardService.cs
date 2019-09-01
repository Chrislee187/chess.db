using System.Collections.Generic;
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

        public async Task<IEnumerable<Game>> GetGamesAsync()
        {
            return await Task.Run(() => _context.GamesWithIncludes());
        }
    }

    public interface IDashboardService
    {
        Task<IEnumerable<Game>> GetGamesAsync();
    }
}