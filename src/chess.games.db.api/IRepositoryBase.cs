using System;
using System.Collections.Generic;

namespace chess.games.db.api
{
    public interface IRepositoryBase<T> where T : class
    {
        void Add(T entity);
        bool Exists(Guid id);
        void Update(T player);
        IEnumerable<T> Get();
        T Get(Guid id);
        bool Save();
    }
}