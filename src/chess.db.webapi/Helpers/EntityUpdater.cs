using System;
using AspNetCore.MVC.RESTful.Helpers;
using chess.games.db.Entities;

namespace chess.db.webapi.Helpers
{
        public class EntityUpdater<TEntity, TId> : IEntityUpdater<TEntity, TId>
            where TEntity : IDbEntity<TId>
        { 
            public void SetId(TEntity entity, TId id) => entity.Id = id;
        }
}