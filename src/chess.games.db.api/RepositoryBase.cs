using System;
using System.Collections.Generic;
using System.Linq;
using chess.games.db.Entities;
using Microsoft.EntityFrameworkCore;

namespace chess.games.db.api
{
    public abstract class RepositoryBase<T> : IRepositoryBase<T> where T : class
    {
        private readonly ChessGamesDbContext _dbContext;

        protected  DbSet<T> Resource => _dbContext.Set<T>();

        protected RepositoryBase(ChessGamesDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void Add(T entity) => _dbContext.Set<T>().Add(entity);
        public bool Exists(Guid id) => Get(id) != null;
        public void Update(T player) { } // NOTE: No code needed, EF tracking handles it, here for testing/completeness
        public IEnumerable<T> Get() => _dbContext.Set<T>();
        public T Get(Guid id) => _dbContext.Set<T>().Find(id);
        public void Delete(T entity) => _dbContext.Set<T>().Remove(entity);
        public bool Save() => (_dbContext.SaveChanges() >= 0);


        // NOTE: This is only using <T> and query should it be here? on Query?
        protected IQueryable<T> Reduce(IQueryable<T> source, Query<T> filters, Query<T> query)
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