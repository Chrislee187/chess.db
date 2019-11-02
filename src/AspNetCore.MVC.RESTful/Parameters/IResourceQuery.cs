using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace AspNetCore.MVC.RESTful.Parameters
{
    public interface IResourceQuery<T>
    {
        bool Empty { get; }
        IQueryable<T> ApplyQuery([NotNull]IQueryable<T> resources);
        IQueryable<T> Apply(IQueryable<T> set);
    }
}