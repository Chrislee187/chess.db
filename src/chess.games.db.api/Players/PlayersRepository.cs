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

        public IQueryable<PgnPlayer> GetPlayers() 
            => _chessGamesDbContext.PgnPlayers;

        public IQueryable<PgnPlayer> GetPlayers(
            PgnPlayersFilterParams filters, 
            PgnPlayersSearchQuery query)
        {
            if (string.IsNullOrEmpty(filters.Name) && string.IsNullOrEmpty(query.QueryText))
            {
                return GetPlayers();
            }

            var set = _chessGamesDbContext.PgnPlayers as IQueryable<PgnPlayer>;

            if (!string.IsNullOrEmpty(filters.Name))
            {
                set = set.Where(p => p.Name.Contains(filters.Name));
            }

            if (!string.IsNullOrEmpty(query.QueryText))
            {
                // Search all fields
                set = set.Where(p => 
                    p.Id.ToString().Contains(query.QueryText) 
                    || p.Name.Contains(query.QueryText));
            }

            return set;
        }

        public PgnPlayer GetPlayer(Guid id) => _chessGamesDbContext.PgnPlayers.Find(id);
    }
}