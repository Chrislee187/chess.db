using System.Collections.Generic;
using AspNetCore.MVC.RESTful.Configuration;
using AspNetCore.MVC.RESTful.Helpers;
using AspNetCore.MVC.RESTful.Services;

namespace AspNetCore.MVC.RESTful.Repositories
{
    /// <summary>
    /// Abstract Resource Repository interface
    /// </summary>
    public interface IResourceRepository<TEntity, TId> where TEntity : class
    {
        void Add(TEntity entity);
        
        bool Exists(TId id);
        
        void Update(TEntity player);
        
        TEntity Load(TId id);
        
        PagedList<TEntity> Load(int page = 1,
            int pageSize = 20,
            IEntityFilter<TEntity> filter = null,
            IEntitySearch<TEntity> search = null,
            string searchString = "",
            string orderBy = "",
            IDictionary<string, OrderByPropertyMappingValue> orderByMappings = null);
        
        void Delete(TEntity entity);
        bool Save();
    }
}