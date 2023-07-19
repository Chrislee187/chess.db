namespace AspNetCore.MVC.RESTful.Helpers
{
    /// <summary>
    /// Updates the <typeparamref name="TEntity"/>'s entity Key/Id field. Only used by the Upsert process
    /// </summary>
    /// <typeparam name="TEntity">Resource entity type</typeparam>
    /// <typeparam name="TId">Type of the Id used in the resource</typeparam>
    public interface IEntityUpdater<in TEntity, in TId>
    {
        void SetId(TEntity entity, TId id);
    }
}