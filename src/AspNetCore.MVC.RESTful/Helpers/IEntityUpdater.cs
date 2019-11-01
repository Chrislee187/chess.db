using System;

namespace AspNetCore.MVC.RESTful.Helpers
{
    public interface IEntityUpdater<in TEntity>
    {
        void SetId(TEntity entity, Guid id);
    }
}