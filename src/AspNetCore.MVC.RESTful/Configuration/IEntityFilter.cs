using System.Linq;

namespace AspNetCore.MVC.RESTful.Configuration
{
    /// <summary>
    /// Interface for collection based resources filter implementations
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public interface IEntityFilter<TEntity>
    {
        bool Empty { get; }
        IQueryable<TEntity> Filter(IQueryable<TEntity> resources);
    }
}