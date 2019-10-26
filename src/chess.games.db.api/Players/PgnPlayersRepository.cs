using System.Linq;
using chess.games.db.Entities;

namespace chess.games.db.api.Players
{
    public class PgnPlayersRepository : RepositoryBase, IPgnPlayersRepository
    {
        private readonly ChessGamesDbContext _chessGamesDbContext;

        public PgnPlayersRepository(ChessGamesDbContext chessGamesDbContext)
            => _chessGamesDbContext = chessGamesDbContext;

        public IQueryable<PgnPlayer> GetPgnPlayers()
            => _chessGamesDbContext.PgnPlayers;

        public IQueryable<PgnPlayer> GetPgnPlayers(
            PgnPlayersFilters filters,
            PgnPlayersSearchQuery query)
            => Reduce(GetPgnPlayers(), filters, query);

        public PgnPlayer GetPgnPlayer(string name)
            => _chessGamesDbContext.PgnPlayers.Find(name);
    }
}