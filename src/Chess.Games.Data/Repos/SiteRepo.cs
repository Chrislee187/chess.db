using Chess.Games.Data.Entities;
using EasyEF.Repos;

namespace Chess.Games.Data.Repos;

public class SiteRepo : EfRepositoryBase<SiteEntity>, ISiteRepository
{
    public SiteRepo(ChessGamesDbContext dbContext) : base(dbContext)
    {
    }
}

public interface ISiteRepository : ILinqRepository<SiteEntity>
{
}
