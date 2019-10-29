using System;
using System.Collections.Generic;
using System.Linq;
using chess.games.db.Entities;

namespace chess.games.db.api
{
    public abstract class RepositoryBase<T> : IRepositoryBase<T> where T : class
    {
        protected readonly ChessGamesDbContext DbContext;

        protected RepositoryBase(ChessGamesDbContext dbContext)
        {
            DbContext = dbContext;
        }

        public void Add(T entity) => DbContext.Set<T>().Add(entity);
        public bool Exists(Guid id) => Get(id) != null;
        public void Update(T player) { } // NOTE: No code needed, EF tracking handles it, here for testing/completeness
        public IEnumerable<T> Get() => DbContext.Set<T>();
        public T Get(Guid id) => DbContext.Set<T>().Find(id);
        public void Delete(T entity) => DbContext.Set<T>().Remove(entity);
        public bool Save() => (DbContext.SaveChanges() >= 0);


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