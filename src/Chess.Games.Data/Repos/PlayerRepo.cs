using Chess.Games.Data.Entities;
using EasyEF.Repos;

namespace Chess.Games.Data.Repos;

public class PlayerRepo : EfRepositoryBase<PlayerEntity>, IPlayerRepository
{
    public PlayerRepo(ChessGamesDbContext dbContext) : base(dbContext)
    {
    }
}
public interface IPlayerRepository : ILinqRepository<PlayerEntity>
{
}

