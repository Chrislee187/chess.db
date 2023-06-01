using Chess.Games.Data.Entities;
using EasyEF.Repos;

namespace Chess.Games.Data.Repos;

public class EventRepo : EfRepositoryBase<EventEntity>, IEventRepository
{
    public EventRepo(ChessGamesDbContext dbContext) : base(dbContext)
    {
    }

    public IEnumerable<EventEntity> GetAll() => Get();
}

public interface IEventRepository : ILinqRepository<EventEntity>
{
    public IEnumerable<EventEntity> GetAll();
}