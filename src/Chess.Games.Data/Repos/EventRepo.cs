using Chess.Games.Data.Entities;
using EasyEF.Repos;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Chess.Games.Data.Repos;

public class EventRepo : EfRepositoryBase<EventEntity>, IEventRepository
{
    public EventRepo(DbContext dbContext) : base(dbContext)
    {
    }

    public IEnumerable<EventEntity> GetAll() => Query;
}

public interface IEventRepository : ILinqRepository<EventEntity>
{
    public IEnumerable<EventEntity> GetAll();
}