using System;
using System.Collections.Generic;
using chess.games.db.Entities;

namespace chess.games.db.api.Players
{
    public interface IPlayersRepository : IRepositoryBase<Player>
    {
        PagedList<Player> Get(PlayersFilters filters,
            PlayersSearchQuery query, PaginationParameters pages);

        IEnumerable<Player> Get(IEnumerable<Guid> ids);

    }
}