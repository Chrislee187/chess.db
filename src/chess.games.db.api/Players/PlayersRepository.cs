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

        public IQueryable<Player> GetPlayers() 
            => _chessGamesDbContext.Players;

        public IQueryable<Player> GetPlayers(
            PlayersFilterParams filters, 
            PlayersSearchQuery query)
        {
            if (string.IsNullOrEmpty(filters.Name) && string.IsNullOrEmpty(query.QueryText))
            {
                return GetPlayers();
            }

            var set = _chessGamesDbContext.Players as IQueryable<Player>;

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

        public Player GetPlayer(Guid id) => _chessGamesDbContext.Players.Find(id);
    }
}