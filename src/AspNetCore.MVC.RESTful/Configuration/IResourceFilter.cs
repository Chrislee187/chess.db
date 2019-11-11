using System.Linq;

namespace AspNetCore.MVC.RESTful.Configuration
{
    public interface IResourceFilter<T>
    {
        bool Empty { get; }
        IQueryable<T> Filter(IQueryable<T> resources);
    }
}