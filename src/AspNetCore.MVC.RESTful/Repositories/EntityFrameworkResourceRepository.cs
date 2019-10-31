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
        {
            _dbContext = dbContext;
        }

        public void Add(TEntity entity) => _dbContext.Set<TEntity>().Add(entity);
        public bool Exists(Guid id) => Get(id) != null;
        public void Update(TEntity player) { } // NOTE: No code needed, EF tracking handles it, here for testing/completeness

        public PagedList<TEntity> Get(Query<TEntity> filters = null,
            Query<TEntity> query = null,
            PaginationParameters pagination = null,
            OrderByParameters orderByParameters = null,
            IDictionary<string, OrderByPropertyMappingValue> orderByMappings = null)
        {
            var f = filters ?? Query<TEntity>.Default;
            var q = query ?? Query<TEntity>.Default;
            var p = pagination ?? PaginationParameters.Default;
            var orderBy = orderByParameters ?? OrderByParameters.Default;
            orderByMappings ??= new Dictionary<string, OrderByPropertyMappingValue>();
            
            var filtered = Reduce(Resource, f, q);

            filtered = filtered.ApplySort(orderBy.Clause, orderByMappings);

            return PagedList<TEntity>.Create(filtered, p.Page, p.PageSize);
        }

        public TEntity Get(Guid id) => _dbContext.Set<TEntity>().Find(id);
        public void Delete(TEntity entity) => _dbContext.Set<TEntity>().Remove(entity);
        public bool Save() => (_dbContext.SaveChanges() >= 0);

        private IQueryable<TEntity> Reduce(IQueryable<TEntity> source, Query<TEntity> filters, Query<TEntity> query)
        {
            if (filters.Empty && query.Empty)
            {
                return source;
            }

            var set = source;

            if (!filters.Empty)
            {
                set = filters.Apply(set);
            }

            if (!query.Empty)
            {
                set = query.Apply(set);
            }

            return set;
        }
    }
}