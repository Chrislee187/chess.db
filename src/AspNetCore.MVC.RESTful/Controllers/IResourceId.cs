namespace AspNetCore.MVC.RESTful.Controllers
{
    public interface IResourceId<T>
    {
        T Id { get; set; }
    }
}