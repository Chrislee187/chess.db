using System;
using System.Collections.Generic;
using chess.games.db.Entities;

namespace chess.games.db.api.Players
{
    public interface IPlayersRepository : IResourceRepositoryBase<Player>
    {
        IEnumerable<Player> Get(IEnumerable<Guid> ids);
    }
}