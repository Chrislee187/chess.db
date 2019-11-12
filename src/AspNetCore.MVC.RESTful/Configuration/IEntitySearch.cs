using System.Linq;

namespace AspNetCore.MVC.RESTful.Configuration
{
    public interface IEntitySearch<TEntity>
    {
        IQueryable<TEntity> Search(IQueryable<TEntity> entities, string searchText);
    }
}