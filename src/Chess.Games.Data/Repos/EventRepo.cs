using Chess.Games.Data.Entities;
using EasyEF.Repos;

namespace Chess.Games.Data.Repos;

public class EventRepo : EasyEfRepositoryBase<EventEntity>, IEventRepository
{
    public EventRepo(ChessGamesDbContext dbContext) : base(dbContext)
    {
    }
}

public interface IEventRepository : ILinqRepository<EventEntity>
{
}