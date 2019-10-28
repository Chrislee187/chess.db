using System;
using System.Collections.Generic;
using System.Linq;
using chess.games.db.Entities;

namespace chess.games.db.api.Players
{
    public interface IPlayersRepository : IRepositoryBase
    {
        IQueryable<Player> GetPlayers();

        IQueryable<Player> GetPlayers(
            PlayersFilters filters,
            PlayersSearchQuery query);

        Player GetPlayer(Guid id);
        void Add(Player entity);
        IQueryable<Player> GetPlayers(IEnumerable<Guid> ids);
    }
}