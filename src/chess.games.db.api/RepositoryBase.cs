using System.Linq;
using chess.games.db.Entities;

namespace chess.games.db.api
{
    public interface IRepositoryBase
    {
        bool Save();
    }

    public abstract class RepositoryBase : IRepositoryBase
    {
        protected readonly ChessGamesDbContext DbContext;

        protected RepositoryBase(ChessGamesDbContext dbContext)
        {
            DbContext = dbContext;
        }

        protected IQueryable<T> Reduce<T>(IQueryable<T> source, Query<T> filters, Query<T> query)
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

        public bool Save()
        {
            return (DbContext.SaveChanges() >= 0);
        }
    }
}