using System.Linq.Expressions;
using EasyEF.Entities;

namespace EasyEF.Repos;

public interface IAsyncLinqRepository<TEntity> where TEntity : Entity
{
    Task<bool> SaveAsync();
    Task<TEntity?> GetAsync(Guid id);
    IAsyncEnumerable<TEntity> GetAsync(Expression<Func<TEntity, bool>>? expr = null);
    Task<bool> ExistsAsync(Guid id);
    Task<int> CountAsync(Expression<Func<TEntity, bool>>? expr);
    Task<long> LongCountAsync(Expression<Func<TEntity, bool>>? expr);
    Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>>? expr);
    Task<TEntity> SingleAsync(Expression<Func<TEntity, bool>>? expr);
    Task<TEntity?> SingleOrDefaultAsync(Expression<Func<TEntity, bool>>? expr);
    Task<TEntity> LastAsync(Expression<Func<TEntity, bool>>? expr);
    Task<TEntity?> LastOrDefaultAsync(Expression<Func<TEntity, bool>>? expr);
    Task<TEntity?> MinAsync(Expression<Func<TEntity, TEntity>>? expr);
    Task<TEntity?> MaxAsync(Expression<Func<TEntity, TEntity>>? expr);
    Task<double> AverageAsync(Expression<Func<TEntity, int>> expr);
    Task<double> AverageAsync(Expression<Func<TEntity, double>> expr);
    Task<float> AverageAsync(Expression<Func<TEntity, float>> expr);
    Task<decimal> AverageAsync(Expression<Func<TEntity, decimal>> expr);
    Task<double> AverageAsync(Expression<Func<TEntity, long>> expr);
    Task<int> SumAsync(Expression<Func<TEntity, int>> expr);
    Task<double> SumAsync(Expression<Func<TEntity, double>> expr);
    Task<float> SumAsync(Expression<Func<TEntity, float>> expr);
    Task<decimal> SumAsync(Expression<Func<TEntity, decimal>> expr);
    Task<long> SumAsync(Expression<Func<TEntity, long>> expr);
}

/// <summary>
/// A Repository pattern based on LINQ and for use with Entity Framework.
/// 
/// EF is an implicit unit-of-work pattern by way of the fact that the changes are NOT
/// committed directly to the database unit M DbContext.SaveChanges() is called and any changes are lost if SaveChanges()
/// has not been called by the time the DbContext is disposed. Therefore there is no implicit transaction support
/// and the unit-of-work is committed by the <see cref="Save"/> method
/// </summary>
public interface ILinqRepository<TEntity> where TEntity : Entity
{
    bool Save();

    TEntity? Get(Guid id);
    IQueryable<TEntity> Get(Expression<Func<TEntity, bool>>? expr = null);
    void Add(params TEntity[] entities);

    bool Exists(Guid id);

    void Update(params TEntity[] entities);
    void Delete(params TEntity[] entities);

    int Count(Expression<Func<TEntity, bool>>? expr);
    TEntity First(Expression<Func<TEntity, bool>>? expr);
    TEntity? FirstOrDefault(Expression<Func<TEntity, bool>>? expr);
    TEntity Single(Expression<Func<TEntity, bool>>? expr);
    TEntity? SingleOrDefault(Expression<Func<TEntity, bool>>? expr);
    TEntity Last(Expression<Func<TEntity, bool>>? expr);
    TEntity? LastOrDefault(Expression<Func<TEntity, bool>>? expr);
    TEntity? Min(Expression<Func<TEntity, TEntity>>? expr);
    TEntity? Max(Expression<Func<TEntity, TEntity>>? expr);
    double Average(Expression<Func<TEntity, int>> expr);
    double Average(Expression<Func<TEntity, double>> expr);
    float Average(Expression<Func<TEntity, float>> expr);
    decimal Average(Expression<Func<TEntity, decimal>> expr);
    double Average(Expression<Func<TEntity, long>> expr);
    int Sum(Expression<Func<TEntity, int>> expr);
    double Sum(Expression<Func<TEntity, double>> expr);
    float Sum(Expression<Func<TEntity, float>> expr);
    decimal Sum(Expression<Func<TEntity, decimal>> expr);
    long Sum(Expression<Func<TEntity, long>> expr);

    long LongCount(Expression<Func<TEntity, bool>>? expr);
}