using System.Linq.Expressions;
using EasyEF.Entities;
using Microsoft.EntityFrameworkCore;

namespace EasyEF.Repos;
/// <summary>
/// Base class for concrete repo's for EF entities
/// </summary>
/// <typeparam name="TEntity"></typeparam>
public abstract class EasyEfRepositoryBase<TEntity> : ILinqRepository<TEntity>, IAsyncLinqRepository<TEntity> where TEntity : Entity
{
    private readonly DbContext _dbContext;
    private readonly DbSet<TEntity> _entitySet;

    protected EasyEfRepositoryBase(DbContext dbContext)
    {
        _dbContext = dbContext;
        _entitySet = _dbContext.Set<TEntity>();

    }
    
    public bool Save() => _dbContext.SaveChanges() >= 0;
    public async Task<bool> SaveAsync() => await _dbContext.SaveChangesAsync() >= 0;

    public void Add(params TEntity[] entities) => _entitySet.AddRange(entities);

    public void Update(params TEntity[] entities) => _entitySet.UpdateRange(entities);

    public void Delete(params TEntity[] entities) => _entitySet.RemoveRange(entities);

    public TEntity? Get(Guid id) => _dbContext.Find<TEntity>(id);
    public async Task<TEntity?> GetAsync(Guid id) => await _dbContext.FindAsync<TEntity>(id);

    public bool Exists(Guid id) => Get(id) != null;
    public async Task<bool> ExistsAsync(Guid id) => await GetAsync(id) != null;

    public IQueryable<TEntity> Get(Expression<Func<TEntity, bool>>? expr = null)
        => expr == null
            ? _entitySet.AsQueryable()
            : _entitySet.Where(expr);

    public IAsyncEnumerable<TEntity> GetAsync(Expression<Func<TEntity, bool>>? expr = null)
        => expr == null
            ? _entitySet.AsAsyncEnumerable()
            : _entitySet.Where(expr).AsAsyncEnumerable();

    public int Count(Expression<Func<TEntity, bool>>? expr)
        => expr == null
            ? _entitySet.Count()
            : _entitySet.Count(expr);

    public async Task<int> CountAsync(Expression<Func<TEntity, bool>>? expr)
        => expr == null
            ? await _entitySet.CountAsync()
            : await _entitySet.CountAsync(expr);

    public long LongCount(Expression<Func<TEntity, bool>>? expr)
        => expr == null
            ? _entitySet.LongCount()
            : Get().LongCount(expr);


    public async Task<long> LongCountAsync(Expression<Func<TEntity, bool>>? expr)
        => expr == null
            ? await _entitySet.LongCountAsync()
            : await Get().LongCountAsync(expr);

    public TEntity First(Expression<Func<TEntity, bool>>? expr)
        => expr == null
            ? _entitySet.First()
            : _entitySet.First(expr);

    public async Task<TEntity> FirstAsync(Expression<Func<TEntity, bool>>? expr)
        => expr == null
            ? await _entitySet.FirstAsync()
            : await _entitySet.FirstAsync(expr);

    public TEntity? FirstOrDefault(Expression<Func<TEntity, bool>>? expr)
        => expr == null
            ? _entitySet.FirstOrDefault()
            : _entitySet.FirstOrDefault(expr);

    public async Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>>? expr)
        => expr == null
            ? await _entitySet.FirstOrDefaultAsync()
            : await _entitySet.FirstOrDefaultAsync(expr);

    public TEntity Single(Expression<Func<TEntity, bool>>? expr)
        => expr == null
            ? _entitySet.Single()
            : _entitySet.Single(expr);
    public async Task<TEntity> SingleAsync(Expression<Func<TEntity, bool>>? expr)
        => expr == null
            ? await _entitySet.SingleAsync()
            : await _entitySet.SingleAsync(expr);

    public TEntity? SingleOrDefault(Expression<Func<TEntity, bool>>? expr)
        => expr == null
            ? _entitySet.SingleOrDefault()
            : _entitySet.SingleOrDefault(expr);

    public async Task<TEntity?> SingleOrDefaultAsync(Expression<Func<TEntity, bool>>? expr)
        => expr == null
            ? await _entitySet.SingleOrDefaultAsync()
            : await _entitySet.SingleOrDefaultAsync(expr);


    public TEntity Last(Expression<Func<TEntity, bool>>? expr)
        => expr == null
            ? _entitySet.Last()
            : _entitySet.Last(expr);

    public async Task<TEntity> LastAsync(Expression<Func<TEntity, bool>>? expr)
        => expr == null
            ? await _entitySet.LastAsync()
            : await _entitySet.LastAsync(expr);

    public TEntity? LastOrDefault(Expression<Func<TEntity, bool>>? expr)
        => expr == null
            ? _entitySet.LastOrDefault()
            : _entitySet.LastOrDefault(expr);

    public async Task<TEntity?> LastOrDefaultAsync(Expression<Func<TEntity, bool>>? expr)
        => expr == null
            ? await _entitySet.LastOrDefaultAsync()
            : await _entitySet.LastOrDefaultAsync(expr);

    public TEntity? Min(Expression<Func<TEntity, TEntity>>? expr)
        => expr == null
            ? _entitySet.Min()
            : _entitySet.Min(expr);

    public async Task<TEntity?> MinAsync(Expression<Func<TEntity, TEntity>>? expr)
        => expr == null
            ? await _entitySet.MinAsync()
            : await _entitySet.MinAsync(expr);

    public TEntity? Max(Expression<Func<TEntity, TEntity>>? expr)
        => expr == null
            ? _entitySet.Max()
            : _entitySet.Max(expr);

    public async Task<TEntity?> MaxAsync(Expression<Func<TEntity, TEntity>>? expr)
        => expr == null
            ? await _entitySet.MaxAsync()
            : await _entitySet.MaxAsync(expr);

    public double Average(Expression<Func<TEntity, int>> expr)
        => _entitySet.Average(expr);
    public double Average(Expression<Func<TEntity, double>> expr)
        => _entitySet.Average(expr);
    public float Average(Expression<Func<TEntity, float>> expr)
        => _entitySet.Average(expr);
    public decimal Average(Expression<Func<TEntity, decimal>> expr)
        => _entitySet.Average(expr);
    public double Average(Expression<Func<TEntity, long>> expr)
        => _entitySet.Average(expr);

    public async Task<double> AverageAsync(Expression<Func<TEntity, int>> expr)
        => await _entitySet.AverageAsync(expr);
    public async Task<double> AverageAsync(Expression<Func<TEntity, double>> expr)
        => await _entitySet.AverageAsync(expr);
    public async Task<float> AverageAsync(Expression<Func<TEntity, float>> expr)
        => await _entitySet.AverageAsync(expr);
    public async Task<decimal> AverageAsync(Expression<Func<TEntity, decimal>> expr)
        => await _entitySet.AverageAsync(expr);
    public async Task<double> AverageAsync(Expression<Func<TEntity, long>> expr)
        => await _entitySet.AverageAsync(expr);

    public int Sum(Expression<Func<TEntity, int>> expr)
        => _entitySet.Sum(expr);
    public double Sum(Expression<Func<TEntity, double>> expr)
        => _entitySet.Sum(expr);
    public float Sum(Expression<Func<TEntity, float>> expr)
        => _entitySet.Sum(expr);
    public decimal Sum(Expression<Func<TEntity, decimal>> expr)
        => _entitySet.Sum(expr);
    public long Sum(Expression<Func<TEntity, long>> expr)
        => _entitySet.Sum(expr);

    public async Task<int> SumAsync(Expression<Func<TEntity, int>> expr)
        => await _entitySet.SumAsync(expr);
    public async Task<double> SumAsync(Expression<Func<TEntity, double>> expr)
        => await _entitySet.SumAsync(expr);
    public async Task<float> SumAsync(Expression<Func<TEntity, float>> expr)
        => await _entitySet.SumAsync(expr);
    public async Task<decimal> SumAsync(Expression<Func<TEntity, decimal>> expr)
        => await _entitySet.SumAsync(expr);
    public async Task<long> SumAsync(Expression<Func<TEntity, long>> expr)
        => await _entitySet.SumAsync(expr);

}