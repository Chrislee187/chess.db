namespace AspNetCore.MVC.RESTful.Controllers
{
    public interface IResourceId<T>
    {
        T Id { get; set; }
    }

    public class Resource<T> : IResourceId<T>
    {
        public T Id { get; set; }
    }
}