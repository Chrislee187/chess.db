using System.Linq;

namespace AspNetCore.MVC.RESTful.Parameters
{
    public interface IResourceFilter<T>
    {
        bool Empty { get; }
        IQueryable<T> Filter(IQueryable<T> resources);
    }
}