using Chess.Games.Data.Entities;
using EasyEF.Repos;
using Microsoft.EntityFrameworkCore;

namespace Chess.Games.Data.Repos;

public class PlayerRepo : EfRepositoryBase<PlayerEntity>, IPlayerRepository
{
    public PlayerRepo(DbContext dbContext) : base(dbContext)
    {
    }
}
public interface IPlayerRepository : ILinqRepository<PlayerEntity>
{
}

