using System.Linq;

namespace AspNetCore.MVC.RESTful.Parameters
{
    public interface IResourceSearch<T>
    {
        IQueryable<T> Search(IQueryable<T> resources, string searchText);
    }
}