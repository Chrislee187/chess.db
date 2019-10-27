using System.Linq;
using chess.games.db.Entities;

namespace chess.games.db.api.Players
{
    public class PgnPlayersRepository : RepositoryBase, IPgnPlayersRepository
    {
        public PgnPlayersRepository(ChessGamesDbContext dbContext)
        : base(dbContext) { }

        public IQueryable<PgnPlayer> GetPgnPlayers()
            => DbContext.PgnPlayers;

        public IQueryable<PgnPlayer> GetPgnPlayers(
            PgnPlayersFilters filters,
            PgnPlayersSearchQuery query)
            => Reduce(GetPgnPlayers(), filters, query);

        public PgnPlayer GetPgnPlayer(string name)
            => DbContext.PgnPlayers.Find(name);
    }
}