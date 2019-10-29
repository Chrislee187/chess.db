using System;
using System.Collections.Generic;
using chess.games.db.Entities;

namespace chess.games.db.api.Players
{
    public interface IPlayersRepository : IRepositoryBase<Player>
    {
        PagedList<Player> Get(
            PlayersFilters filters = null,
            PlayersSearchQuery query = null, 
            PaginationParameters pagination = null,
            OrderByParameters orderByParameters = null);

        IEnumerable<Player> Get(IEnumerable<Guid> ids);

    }
}