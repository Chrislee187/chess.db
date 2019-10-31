using System;

namespace AspNetCore.MVC.RESTful.Controllers
{
    public interface IResourceEntity
    {
        Guid Id { get; set; }
    }
}