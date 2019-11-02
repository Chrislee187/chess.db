using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace AspNetCore.MVC.RESTful.Parameters
{
    public abstract class ResourceQuery<T> : IResourceQuery<T>
    {
        public static readonly ResourceQuery<T> Default = new EmptyResourceQuery();
        public abstract bool Empty { get; }
        public abstract IQueryable<T> ApplyQuery([NotNull]IQueryable<T> resources);

        public IQueryable<T> Apply(IQueryable<T> set)
        {
            if (!Empty)
            {
                set = ApplyQuery(set);
            }

            return set;
        }

        private class EmptyResourceQuery : ResourceQuery<T>
        {
            public override bool Empty { get; } = true;
            public override IQueryable<T> ApplyQuery(IQueryable<T> resources) => resources;
        }    
    }

    
}