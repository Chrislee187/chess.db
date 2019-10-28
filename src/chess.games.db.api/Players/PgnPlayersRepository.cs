using System;
using System.Collections.Generic;
using chess.games.db.Entities;

namespace chess.games.db.api.Players
{
    public class PgnPlayersRepository : RepositoryBase<PgnPlayer>, IPgnPlayersRepository
    {
        public PgnPlayersRepository(ChessGamesDbContext dbContext)
        : base(dbContext) { }

        public IEnumerable<PgnPlayer> Get(
            PgnPlayersFilters filters,
            PgnPlayersSearchQuery query)
            => Reduce(DbContext.PgnPlayers, filters, query);

        public new PgnPlayer Get(Guid id)
            => throw new NotSupportedException($"PgnPlayers do not have GUID primary key, use the Name instead.");

        public PgnPlayer Get(string name)
            => DbContext.PgnPlayers.Find(name);
    }
}