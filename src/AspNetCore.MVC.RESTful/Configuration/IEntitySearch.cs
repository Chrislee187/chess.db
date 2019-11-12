using System.Linq;

namespace AspNetCore.MVC.RESTful.Configuration
{
    /// <summary>
    /// Interface for collection based resource search implementations
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public interface IEntitySearch<TEntity>
    {
        IQueryable<TEntity> Search(IQueryable<TEntity> entities, string searchText);
    }
}