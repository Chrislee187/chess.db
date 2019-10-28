using System;
using System.Collections.Generic;
using System.Linq;
using chess.games.db.Entities;

namespace chess.games.db.api.Players
{
    public class PlayersRepository : RepositoryBase<Player>, IPlayersRepository
    {
        public PlayersRepository(ChessGamesDbContext dbContext)
            : base(dbContext) { }

        public IEnumerable<Player> Get(IEnumerable<Guid> ids)
            => DbContext.Players.Where(p => ids.Contains(p.Id));

        public IEnumerable<Player> Get(
            PlayersFilters filters,
            PlayersSearchQuery query) 
            => Reduce(DbContext.Players, filters, query);


    }
}