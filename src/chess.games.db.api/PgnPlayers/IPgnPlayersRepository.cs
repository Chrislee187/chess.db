using System;
using AspNetCore.MVC.RESTful.Repositories;
using chess.games.db.Entities;

namespace chess.games.db.api.PgnPlayers
{
    public interface IPgnPlayersRepository : IResourceRepository<PgnPlayer, Guid>
    {
        PgnPlayer Get(string name);
    }
}