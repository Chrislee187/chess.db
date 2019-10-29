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

        public PagedList<Player> Get(
            PlayersFilters filters = null,
            PlayersSearchQuery query = null, 
            PaginationParameters pages = null)
        {
            var f = filters ?? Query<Player>.Null;
            var q = query ?? Query<Player>.Null;
            var p = pages ?? PaginationParameters.Default;

            var filtered = Reduce(DbContext.Players, f, q);

            return PagedList<Player>.Create(filtered, p.Page, p.PageSize);
        }
    }
}