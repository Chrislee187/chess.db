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
            PgnPlayersFilters filters, 
            PgnPlayersSearchQuery query)
        {
            if (filters.IsEmpty && query.IsEmpty)
            {
                return GetPgnPlayers();
            }

            var set = _chessGamesDbContext.PgnPlayers as IQueryable<PgnPlayer>;

            set = set.ApplyFilters(filters)
                .ApplyQuery(query);

            return set;
        }

        public PgnPlayer GetPlayer(Guid id) => _chessGamesDbContext.PgnPlayers.Find(id);
    }

    public static class IQueryablePgnPlayerExtensions
    {
        public static IQueryable<PgnPlayer> ApplyQuery(this IQueryable<PgnPlayer> set, PgnPlayersSearchQuery query)
        {
            if (!query.IsEmpty)
            {
                // Search all fields
                set = set.Where(p =>
                    p.Name.Contains(query.QueryText));
            }

            return set;
        }

        public static IQueryable<PgnPlayer> ApplyFilters(this IQueryable<PgnPlayer> set, PgnPlayersFilters filters)
        {
            if (!filters.IsEmpty)
            {
                set = set.Where(p =>
                    p.Name.ToLower().Contains(filters.Name.ToLower()));
            }

            return set;
        }
    }
}