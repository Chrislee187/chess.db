using System.Collections.Generic;
using System.Linq;
using AspNetCore.MVC.RESTful.Configuration;
using AspNetCore.MVC.RESTful.Helpers;
using AspNetCore.MVC.RESTful.Services;
using Microsoft.EntityFrameworkCore;

namespace AspNetCore.MVC.RESTful.Repositories
{
    public abstract class EntityFrameworkResourceRepository<TEntity, TId> : IResourceRepository<TEntity, TId> where TEntity : class
    {
        private readonly DbContext _dbContext;

        protected  DbSet<TEntity> Resource => _dbContext.Set<TEntity>();

        protected EntityFrameworkResourceRepository(DbContext dbContext) 
            => _dbContext = dbContext;

        public void Add(TEntity entity) => _dbContext.Set<TEntity>().Add(entity);
        public bool Exists(TId id) => Load(id) != null;
        public void Update(TEntity player) { } // NOTE: No code needed, EF tracking handles it

        public PagedList<TEntity> Load(int page = 1,
            int pageSize = 20,
            IResourceFilter<TEntity> filter = null,
            IResourceSearch<TEntity> search = null,
            string searchString = "",
            string orderBy = "",
            IDictionary<string, OrderByPropertyMappingValue> orderByMappings = null)
        {
            orderByMappings ??= new Dictionary<string, OrderByPropertyMappingValue>();
            
            var filtered = Reduce(
                Resource, 
                filter ?? ResourceFilter<TEntity>.Default, 
                search ?? new DefaultResourceSearch<TEntity>(), 
                searchString);

            var sorted = filtered.ApplySort(orderBy, orderByMappings);

            return new PagedList<TEntity>(sorted, pageSize, page);
        }

        public TEntity Load(TId id) => _dbContext.Set<TEntity>().Find(id);
        
        public void Delete(TEntity entity) => _dbContext.Set<TEntity>().Remove(entity);
        
        public bool Save() => (_dbContext.SaveChanges() >= 0);
        
        private IQueryable<TEntity> Reduce(
            IQueryable<TEntity> resources, 
            IResourceFilter<TEntity> filter, 
            IResourceSearch<TEntity> search,
            string searchText)
        {
            if (filter.Empty && string.IsNullOrEmpty(searchText))
            {
                return resources;
            }

            var reduced = resources;

            if (!filter.Empty)
            {
                reduced = filter.Filter(reduced);
            }

            if (!string.IsNullOrEmpty(searchText))
            {
                reduced = search.Search(reduced, searchText);
            }

            return reduced;
        }
    }
}