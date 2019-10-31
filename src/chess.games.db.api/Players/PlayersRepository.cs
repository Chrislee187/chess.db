using System;
using System.Collections.Generic;
using System.Linq;
using chess.games.db.api.Helpers;
using chess.games.db.Entities;

namespace chess.games.db.api.Players
{
    public class PlayersRepository : ResourceRepositoryBase<Player>, IPlayersRepository
    {
        public PlayersRepository(
            ChessGamesDbContext dbContext
            )
            : base(dbContext) { }

        public IEnumerable<Player> Get(IEnumerable<Guid> ids)
            => Resource.Where(p => ids.Contains(p.Id));

    }
}