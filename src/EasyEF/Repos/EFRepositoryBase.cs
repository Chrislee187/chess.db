using System.Linq.Expressions;
using EasyEF.Entities;
using Microsoft.EntityFrameworkCore;

namespace EasyEF.Repos;

public abstract class EfRepositoryBase<TEntity> : ILinqRepository<TEntity> where TEntity : Entity
{
    // TODO: Add Async counterparts to interface
    protected readonly DbContext DbContext;
    private readonly DbSet<TEntity> _entitySet;

    protected IQueryable<TEntity> Query => _entitySet;

    protected EfRepositoryBase(DbContext dbContext)
    {
        DbContext = dbContext;
        _entitySet = DbContext.Set<TEntity>();

    }
    
    public void Add(params TEntity[] entities) => _entitySet.AddRange(entities);
    public bool Exists(Guid id) => Get(id) != null;
    
    public TEntity? Get(Guid id) => DbContext.Find<TEntity>(id);
    public IQueryable<TEntity> Get() => _entitySet;
    public IQueryable<TEntity> Get(Expression<Func<TEntity, bool>> expr)
        => _entitySet.Where(expr);

    public void Update(params TEntity[] entities) => _entitySet.UpdateRange(entities);

    public void Delete(params TEntity[] entities) => _entitySet.RemoveRange(entities);

    public bool Save() => DbContext.SaveChanges() >= 0;

    public int Count(Expression<Func<TEntity, bool>>? expr)
        => expr == null
            ? Query.Count()
            : Query.Count(expr);
    public long LongCount(Expression<Func<TEntity, bool>>? expr)
        => expr == null
            ? Query.LongCount()
            : Query.LongCount(expr);

    public TEntity First(Expression<Func<TEntity, bool>>? expr)
        => expr == null
            ? Query.First()
            : Query.First(expr);

    public TEntity? FirstOrDefault(Expression<Func<TEntity, bool>>? expr)
        => expr == null
            ? Query.FirstOrDefault()
            : Query.FirstOrDefault(expr);

    public TEntity Single(Expression<Func<TEntity, bool>>? expr)
        => expr == null
            ? Query.Single()
            : Query.Single(expr);

    public TEntity? SingleOrDefault(Expression<Func<TEntity, bool>>? expr)
        => expr == null
            ? Query.SingleOrDefault()
            : Query.SingleOrDefault(expr);


    public TEntity Last(Expression<Func<TEntity, bool>>? expr)
        => expr == null
            ? Query.Last()
            : Query.Last(expr);

    public TEntity? LastOrDefault(Expression<Func<TEntity, bool>>? expr)
        => expr == null
            ? Query.LastOrDefault()
            : Query.LastOrDefault(expr);

    public TEntity? Min(Expression<Func<TEntity, TEntity>>? expr)
        => expr == null
            ? Query.Min()
            : Query.Min(expr);


    public TEntity? Max(Expression<Func<TEntity, TEntity>>? expr)
        => expr == null
            ? Query.Max()
            : Query.Max(expr);

    public double Average(Expression<Func<TEntity, int>> expr)
        => Query.Average(expr);
    public double Average(Expression<Func<TEntity, double>> expr)
        => Query.Average(expr);
    public float Average(Expression<Func<TEntity, float>> expr)
        => Query.Average(expr);
    public decimal Average(Expression<Func<TEntity, decimal>> expr)
        => Query.Average(expr);
    public double Average(Expression<Func<TEntity, long>> expr)
        => Query.Average(expr);


    public int Sum(Expression<Func<TEntity, int>> expr)
        => Query.Sum(expr);
    public double Sum(Expression<Func<TEntity, double>> expr)
        => Query.Sum(expr);
    public float Sum(Expression<Func<TEntity, float>> expr)
        => Query.Sum(expr);
    public decimal Sum(Expression<Func<TEntity, decimal>> expr)
        => Query.Sum(expr);
    public long Sum(Expression<Func<TEntity, long>> expr)
        => Query.Sum(expr);

}