using System;
using System.Collections.Generic;
using System.Linq;
using AspNetCore.MVC.RESTful.Helpers;
using AspNetCore.MVC.RESTful.Parameters;
using Microsoft.EntityFrameworkCore;

namespace AspNetCore.MVC.RESTful.Repositories
{
    public abstract class EntityFrameworkResourceRepository<TEntity> : IResourceRepository<TEntity> where TEntity : class
    {
        private readonly DbContext _dbContext;

        protected  DbSet<TEntity> Resource => _dbContext.Set<TEntity>();

        protected EntityFrameworkResourceRepository(DbContext dbContext) 
            => _dbContext = dbContext;

        public void Add(TEntity entity) => _dbContext.Set<TEntity>().Add(entity);
        public bool Exists(Guid id) => Load(id) != null;
        public void Update(TEntity player) { } // NOTE: No code needed, EF tracking handles it

        public PagedList<TEntity> Load(
            IResourceQuery<TEntity> filters = null,
            IResourceQuery<TEntity> resourceQuery = null,
            PaginationParameters pagination = null,
            OrderByParameters orderByParameters = null,
            IDictionary<string, OrderByPropertyMappingValue> orderByMappings = null)
        {
            var f = filters ?? ResourceQuery<TEntity>.Default;
            var q = resourceQuery ?? ResourceQuery<TEntity>.Default;
            var p = pagination ?? PaginationParameters.Default;
            var orderBy = orderByParameters ?? OrderByParameters.Default;
            orderByMappings ??= new Dictionary<string, OrderByPropertyMappingValue>();
            
            var filtered = Reduce(Resource, f, q);

            filtered = filtered.ApplySort(orderBy.Clause, orderByMappings);

            return new PagedList<TEntity>(filtered, p);
        }

        public TEntity Load(Guid id) => _dbContext.Set<TEntity>().Find(id);


        public void Delete(TEntity entity) => _dbContext.Set<TEntity>().Remove(entity);
        
        public bool Save() => (_dbContext.SaveChanges() >= 0);
        
        private IQueryable<TEntity> Reduce(
            IQueryable<TEntity> resources, 
            IResourceQuery<TEntity> filters, 
            IResourceQuery<TEntity> resourceQuery)
        {
            if (filters.Empty && resourceQuery.Empty)
            {
                return resources;
            }

            var reduced = resources;

            if (!filters.Empty)
            {
                reduced = filters.Apply(reduced);
            }

            if (!resourceQuery.Empty)
            {
                reduced = resourceQuery.Apply(reduced);
            }

            return reduced;
        }
    }
}