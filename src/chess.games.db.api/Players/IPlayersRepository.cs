using System;
using System.Collections.Generic;
using AspNetCore.MVC.RESTful.Repositories;
using chess.games.db.Entities;

namespace chess.games.db.api.Players
{
    public interface IPlayersRepository : IResourceRepository<Player>
    {
        IEnumerable<Player> Get(IEnumerable<Guid> ids);
    }
}