using System;
using System.Collections.Generic;
using AspNetCore.MVC.RESTful.Helpers;
using AspNetCore.MVC.RESTful.Parameters;

namespace AspNetCore.MVC.RESTful.Repositories
{
    public interface IResourceRepository<T> where T : class
    {
        void Add(T entity);
        
        bool Exists(Guid id);
        
        void Update(T player);
        
        T Load(Guid id);
        
        PagedList<T> Load(int page = 1,
            int pageSize = 20,
            IResourceFilter<T> filters = null,
            IResourceSearch<T> search = null,
            string searchString = "",
            string orderBy = "",
            IDictionary<string, OrderByPropertyMappingValue> orderByMappings = null);
        
        void Delete(T entity);
        bool Save();
    }
}