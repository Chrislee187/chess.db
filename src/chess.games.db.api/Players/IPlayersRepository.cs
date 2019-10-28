using System;
using System.Collections.Generic;
using chess.games.db.Entities;

namespace chess.games.db.api.Players
{
    public interface IPlayersRepository : IRepositoryBase<Player>
    {
        IEnumerable<Player> Get(
            PlayersFilters filters,
            PlayersSearchQuery query);

        IEnumerable<Player> Get(IEnumerable<Guid> ids);

    }
}