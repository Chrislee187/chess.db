using System;
using System.Linq;
using chess.games.db.Entities;

namespace chess.games.db.api.Players
{
    public class PlayersRepository : IPlayersRepository
    {
        private readonly ChessGamesDbContext _chessGamesDbContext;

        public PlayersRepository(ChessGamesDbContext chessGamesDbContext)
        {
            _chessGamesDbContext = chessGamesDbContext;
        }

        public IQueryable<PgnPlayer> GetPgnPlayers() 
            => _chessGamesDbContext.PgnPlayers;

        public IQueryable<PgnPlayer> GetPgnPlayers(
            PgnPlayersFilterParams filters, 
            PgnPlayersSearchQuery query)
        {
            if (string.IsNullOrEmpty(filters.Name) && string.IsNullOrEmpty(query.QueryText))
            {
                return GetPgnPlayers();
            }

            var set = _chessGamesDbContext.PgnPlayers as IQueryable<PgnPlayer>;

            if (!string.IsNullOrEmpty(filters.Name))
            {
                set = set.Where(p => p.Name.ToLower().Contains(filters.Name.ToLower()));
            }

            if (!string.IsNullOrEmpty(query.QueryText))
            {
                // Search all fields
                set = set.Where(p => 
                    p.Name.Contains(query.QueryText));
            }

            return set;
        }

        public PgnPlayer GetPlayer(Guid id) => _chessGamesDbContext.PgnPlayers.Find(id);
    }
}