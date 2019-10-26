using System;
using System.Linq;
using chess.games.db.Entities;

namespace chess.games.db.api.Players
{
    public interface IPlayersRepository
    {
        IQueryable<PgnPlayer> GetPgnPlayers();

        IQueryable<PgnPlayer> GetPgnPlayers(PgnPlayersFilters filters, PgnPlayersSearchQuery searchQuery);
        PgnPlayer GetPlayer(Guid id);
    }
}