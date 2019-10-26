using System;
using System.Linq;
using chess.games.db.Entities;

namespace chess.games.db.api.Players
{
    public interface IPlayersRepository
    {
        IQueryable<Player> GetPlayers();

        IQueryable<Player> GetPlayers(
            PlayersFilters filters,
            PlayersSearchQuery query);

        Player GetPlayer(Guid id);
    }
}