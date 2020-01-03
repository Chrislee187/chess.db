using System;
using System.Linq;
using AspNetCore.MVC.RESTful.Repositories;
using chess.games.db.Entities;
using Microsoft.EntityFrameworkCore;

namespace chess.games.db.api.Players
{
    public interface IGamesRepository : IResourceRepository<Game, Guid>
    {

    }
    public class GamesRepository : EntityFrameworkResourceRepository<Game, Guid>, IGamesRepository
    {
        public GamesRepository(
            ChessGamesDbContext dbContext
        )
            : base(dbContext) { }

        protected override IQueryable<Game> Resource =>
           base.Resource
                .Include(g => g.Event)
                .Include(g => g.Site)
                .Include(g => g.White)
                .Include(g => g.Black)
                .AsQueryable()
        ;
    }
}