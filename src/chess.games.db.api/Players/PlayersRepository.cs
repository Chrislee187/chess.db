using System;
using System.Collections.Generic;
using System.Linq;
using chess.games.db.api.Helpers;
using chess.games.db.Entities;

namespace chess.games.db.api.Players
{
    public class PlayersRepository : RepositoryBase<Player>, IPlayersRepository
    {

        public PlayersRepository(ChessGamesDbContext dbContext)
            : base(dbContext) { }

        public IEnumerable<Player> Get(IEnumerable<Guid> ids)
            => Resource.Where(p => ids.Contains(p.Id));

        public PagedList<Player> Get(
            PlayersFilters filters = null,
            PlayersSearchQuery query = null, 
            PaginationParameters pagination = null,
            OrderByParameters orderByParameters = null)
        {
            var f = filters ?? Query<Player>.Default;
            var q = query ?? Query<Player>.Default;
            var p = pagination ?? PaginationParameters.Default;
            var orderBy = orderByParameters ?? OrderByParameters.Default;

            var filtered = Reduce(Resource, f, q);
            filtered = filtered.ApplySort(orderBy.Clause, orderBy.Mappings);

            return PagedList<Player>.Create(filtered, p.Page, p.PageSize);
        }
    }
}