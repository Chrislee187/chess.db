using System.Linq;

namespace AspNetCore.MVC.RESTful.Configuration
{
    /// <summary>
    /// Default <see cref="IEntitySearch{TEntities}"/> implementation that returns all entities.
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class DefaultEntitySearch<TEntity> : IEntitySearch<TEntity>
    {
        public IQueryable<TEntity> Search(IQueryable<TEntity> entities, string searchText) => entities;
    }
}