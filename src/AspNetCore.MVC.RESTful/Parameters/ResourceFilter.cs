using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace AspNetCore.MVC.RESTful.Parameters
{
    public abstract class ResourceFilter<T> : IResourceFilter<T>
    {
        public static readonly ResourceFilter<T> Default = new EmptyResourceFilter();
        public abstract bool Empty { get; }
        protected abstract IQueryable<T> FilterImpl(
            [NotNull]IQueryable<T> resources);

        public IQueryable<T> Filter(
            IQueryable<T> set)
        {
            if (!Empty)
            {
                set = FilterImpl(set);
            }

            return set;
        }

        private class EmptyResourceFilter : ResourceFilter<T>
        {
            public override bool Empty { get; } = true;
            protected override IQueryable<T> FilterImpl(IQueryable<T> resources) => resources;
        }    
    }
}