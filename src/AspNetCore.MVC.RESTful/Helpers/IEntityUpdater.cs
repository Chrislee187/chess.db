using System;

namespace AspNetCore.MVC.RESTful.Helpers
{
    public interface IEntityUpdater<in TEntity, in TId>
    {
        void SetId(TEntity entity, TId id);
    }
}