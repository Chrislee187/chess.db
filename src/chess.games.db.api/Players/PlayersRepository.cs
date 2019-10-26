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
            if (filters.Empty && query.Empty)
            {
                return GetPgnPlayers();
            }

            var set = _chessGamesDbContext.PgnPlayers as IQueryable<PgnPlayer>;

            set = filters.Apply(set);
            set = query.Apply(set);

            return set;
        }

        public PgnPlayer GetPlayer(Guid id) => _chessGamesDbContext.PgnPlayers.Find(id);
    }

}