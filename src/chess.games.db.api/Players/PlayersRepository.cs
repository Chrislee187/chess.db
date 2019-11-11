using System;
using System.Collections.Generic;
using System.Linq;
using AspNetCore.MVC.RESTful.Repositories;
using chess.games.db.Entities;

namespace chess.games.db.api.Players
{
    public class PlayersRepository : EntityFrameworkResourceRepository<Player, Guid>, IPlayersRepository
    {
        public PlayersRepository(
            ChessGamesDbContext dbContext
            )
            : base(dbContext) { }

        public IEnumerable<Player> Get(IEnumerable<Guid> ids)
            => Resource.Where(p => ids.Contains(p.Id));

    }
}