using System;
using System.Collections.Generic;
using AspNetCore.MVC.RESTful.Helpers;
using AspNetCore.MVC.RESTful.Parameters;

namespace AspNetCore.MVC.RESTful.Repositories
{
    public interface IResourceRepository<TEntity, TId> where TEntity : class
    {
        void Add(TEntity entity);
        
        bool Exists(TId id);
        
        void Update(TEntity player);
        
        TEntity Load(TId id);
        
        PagedList<TEntity> Load(int page = 1,
            int pageSize = 20,
            IResourceFilter<TEntity> filters = null,
            IResourceSearch<TEntity> search = null,
            string searchString = "",
            string orderBy = "",
            IDictionary<string, OrderByPropertyMappingValue> orderByMappings = null);
        
        void Delete(TEntity entity);
        bool Save();
    }
}