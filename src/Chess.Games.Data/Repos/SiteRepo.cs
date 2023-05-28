using Chess.Games.Data.Entities;
using EasyEF.Repos;
using Microsoft.EntityFrameworkCore;

namespace Chess.Games.Data.Repos;

public class SiteRepo : EfRepositoryBase<SiteEntity>, ISiteRepository
{
    public SiteRepo(DbContext dbContext) : base(dbContext)
    {
    }
}

public interface ISiteRepository : ILinqRepository<SiteEntity>
{
}
