using System.Linq.Expressions;
using EasyEF.Entities;
using Microsoft.EntityFrameworkCore;

namespace EasyEF.Repos;
/// <summary>
/// Base class for concrete repo's for EF entities
/// </summary>
/// <typeparam name="TEntity"></typeparam>
public abstract class EfRepositoryBase<TEntity> : ILinqRepository<TEntity> where TEntity : Entity
{
    // TODO: Add Async counterparts to interface
    private readonly DbContext _dbContext;
    private readonly DbSet<TEntity> _entitySet;

    // NOTE: This is deliberately protected to avoid

    protected EfRepositoryBase(DbContext dbContext)
    {
        _dbContext = dbContext;
        _entitySet = _dbContext.Set<TEntity>();

    }
    public bool Save() => _dbContext.SaveChanges() >= 0;

    public void Add(params TEntity[] entities) => _entitySet.AddRange(entities);
    public bool Exists(Guid id) => Get(id) != null;
    
    public TEntity? Get(Guid id) => _dbContext.Find<TEntity>(id);
    public IQueryable<TEntity> Get() => _entitySet;
    public IQueryable<TEntity> Get(Expression<Func<TEntity, bool>> expr)
        => _entitySet.Where(expr);

    public void Update(params TEntity[] entities) => _entitySet.UpdateRange(entities);

    public void Delete(params TEntity[] entities) => _entitySet.RemoveRange(entities);
    
    public int Count(Expression<Func<TEntity, bool>>? expr)
        => expr == null
            ? Get().Count()
            : Get().Count(expr);
    public long LongCount(Expression<Func<TEntity, bool>>? expr)
        => expr == null
            ? Get().LongCount()
            : Get().LongCount(expr);

    public TEntity First(Expression<Func<TEntity, bool>>? expr)
        => expr == null
            ? Get().First()
            : Get().First(expr);

    public TEntity? FirstOrDefault(Expression<Func<TEntity, bool>>? expr)
        => expr == null
            ? Get().FirstOrDefault()
            : Get().FirstOrDefault(expr);

    public TEntity Single(Expression<Func<TEntity, bool>>? expr)
        => expr == null
            ? Get().Single()
            : Get().Single(expr);

    public TEntity? SingleOrDefault(Expression<Func<TEntity, bool>>? expr)
        => expr == null
            ? Get().SingleOrDefault()
            : Get().SingleOrDefault(expr);


    public TEntity Last(Expression<Func<TEntity, bool>>? expr)
        => expr == null
            ? Get().Last()
            : Get().Last(expr);

    public TEntity? LastOrDefault(Expression<Func<TEntity, bool>>? expr)
        => expr == null
            ? Get().LastOrDefault()
            : Get().LastOrDefault(expr);

    public TEntity? Min(Expression<Func<TEntity, TEntity>>? expr)
        => expr == null
            ? Get().Min()
            : Get().Min(expr);


    public TEntity? Max(Expression<Func<TEntity, TEntity>>? expr)
        => expr == null
            ? Get().Max()
            : Get().Max(expr);

    public double Average(Expression<Func<TEntity, int>> expr)
        => Get().Average(expr);
    public double Average(Expression<Func<TEntity, double>> expr)
        => Get().Average(expr);
    public float Average(Expression<Func<TEntity, float>> expr)
        => Get().Average(expr);
    public decimal Average(Expression<Func<TEntity, decimal>> expr)
        => Get().Average(expr);
    public double Average(Expression<Func<TEntity, long>> expr)
        => Get().Average(expr);


    public int Sum(Expression<Func<TEntity, int>> expr)
        => Get().Sum(expr);
    public double Sum(Expression<Func<TEntity, double>> expr)
        => Get().Sum(expr);
    public float Sum(Expression<Func<TEntity, float>> expr)
        => Get().Sum(expr);
    public decimal Sum(Expression<Func<TEntity, decimal>> expr)
        => Get().Sum(expr);
    public long Sum(Expression<Func<TEntity, long>> expr)
        => Get().Sum(expr);

}