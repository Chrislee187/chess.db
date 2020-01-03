using System;
using AspNetCore.MVC.RESTful.Repositories;
using chess.games.db.Entities;

namespace chess.games.db.api.Players
{
    public interface ISitesRepository : IResourceRepository<Site, Guid>
    {

    }
    public class SitesRepository : EntityFrameworkResourceRepository<Site, Guid>, ISitesRepository
    {
        public SitesRepository(
            ChessGamesDbContext dbContext
        )
            : base(dbContext) { }

    }
}