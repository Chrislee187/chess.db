using System.Linq;

namespace AspNetCore.MVC.RESTful.Configuration
{
    public class DefaultResourceSearch<T> : IResourceSearch<T>
    {
        public IQueryable<T> Search(IQueryable<T> resources, string searchText) => resources;
    }
}