using System;
using System.Linq;
using chess.games.db.Entities;

namespace chess.games.db.api.Players
{
    public interface IPlayersRepository
    {
        IQueryable<PgnPlayer> GetPlayers();

        IQueryable<PgnPlayer> GetPlayers(PgnPlayersFilterParams filters, PgnPlayersSearchQuery searchQuery);
        PgnPlayer GetPlayer(Guid id);
    }
}