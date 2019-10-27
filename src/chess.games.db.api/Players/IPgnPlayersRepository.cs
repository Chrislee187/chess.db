using System.Linq;
using chess.games.db.Entities;

namespace chess.games.db.api.Players
{
    public interface IPgnPlayersRepository : IRepositoryBase
    {
        IQueryable<PgnPlayer> GetPgnPlayers();

        IQueryable<PgnPlayer> GetPgnPlayers(PgnPlayersFilters filters, PgnPlayersSearchQuery searchQuery);
        PgnPlayer GetPgnPlayer(string name);
    }
}