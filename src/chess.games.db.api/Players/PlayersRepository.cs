using System;
using System.Collections.Generic;
using System.Linq;
using chess.games.db.Entities;

namespace chess.games.db.api.Players
{
    public class PlayersRepository : RepositoryBase, IPlayersRepository
    {
        public PlayersRepository(ChessGamesDbContext dbContext)
            : base(dbContext) { }

        public IQueryable<Player> GetPlayers()
            => DbContext.Players;

        public IQueryable<Player> GetPlayers(IEnumerable<Guid> ids)
            => DbContext.Players.Where(p => ids.Contains(p.Id));

        public IQueryable<Player> GetPlayers(
            PlayersFilters filters,
            PlayersSearchQuery query) 
            => Reduce(GetPlayers(), filters, query);

        public Player GetPlayer(Guid id)
            => DbContext.Players.Find(id);

        public void Add(Player entity) 
            => DbContext.Players.Add(entity);

    }
}