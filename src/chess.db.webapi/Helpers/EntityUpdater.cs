using System;
using AspNetCore.MVC.RESTful.Helpers;
using chess.games.db.Entities;

namespace chess.db.webapi.Helpers
{
        public class EntityUpdater<TEntity> : IEntityUpdater<TEntity>
            where TEntity : IDbEntity
        { 
            public void SetId(TEntity entity, Guid id) => entity.Id = id;
        }
}