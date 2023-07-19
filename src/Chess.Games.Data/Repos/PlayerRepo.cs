using Chess.Games.Data.Entities;
using EasyEF.Repos;

namespace Chess.Games.Data.Repos;

public class PlayerRepo : EasyEfRepositoryBase<PlayerEntity>, IPlayerRepository
{
    public PlayerRepo(ChessGamesDbContext dbContext) : base(dbContext)
    {
    }
}
public interface IPlayerRepository : ILinqRepository<PlayerEntity>
{
}

