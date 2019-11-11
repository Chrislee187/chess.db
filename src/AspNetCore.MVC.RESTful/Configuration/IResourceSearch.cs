using System.Linq;

namespace AspNetCore.MVC.RESTful.Configuration
{
    public interface IResourceSearch<T>
    {
        IQueryable<T> Search(IQueryable<T> resources, string searchText);
    }
}