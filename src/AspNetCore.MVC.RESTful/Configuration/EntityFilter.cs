using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace AspNetCore.MVC.RESTful.Configuration
{
    /// <summary>
    /// Abstract filter implementation for use with collection based resources
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public abstract class EntityFilter<TEntity> : IEntityFilter<TEntity>
    {
        public static readonly EntityFilter<TEntity> Default = new EmptyEntityFilter();
        public abstract bool Empty { get; }
        protected abstract IQueryable<TEntity> FilterImpl(
            [NotNull]IQueryable<TEntity> resources);

        public IQueryable<TEntity> Filter(
            IQueryable<TEntity> set)
        {
            if (!Empty)
            {
                set = FilterImpl(set);
            }

            return set;
        }

        private class EmptyEntityFilter : EntityFilter<TEntity>
        {
            public override bool Empty { get; } = true;
            protected override IQueryable<TEntity> FilterImpl(IQueryable<TEntity> resources) => resources;
        }    
    }
}