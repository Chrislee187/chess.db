using System;
using AspNetCore.MVC.RESTful.Repositories;
using chess.games.db.Entities;

namespace chess.games.db.api.PgnPlayers
{
    public class PgnPlayersRepository : EntityFrameworkResourceRepository<PgnPlayer, Guid>, IPgnPlayersRepository
    {
        public PgnPlayersRepository(ChessGamesDbContext dbContext)
        : base(dbContext) { }

        public new PgnPlayer Load(Guid id)
            => throw new NotSupportedException($"PgnPlayers do not have GUID primary key, use the Name instead.");

        public PgnPlayer Get(string name)
            => Resource.Find(name);
    }
}