using System.Linq.Expressions;
using EasyEF.Entities;

namespace EasyEF.Repos;

public interface ILinqRepository<TEntity> where TEntity : Entity
{
    TEntity? Get(Guid id);
    IQueryable<TEntity> Get();
    IQueryable<TEntity> Get(Expression<Func<TEntity, bool>> expr);
    void Add(params TEntity[] entities);

    bool Exists(Guid id);

    void Update(params TEntity[] entities);
    void Delete(params TEntity[] entities);

    bool Save();

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
}