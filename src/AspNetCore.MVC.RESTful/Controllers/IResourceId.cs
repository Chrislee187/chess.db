using System;

namespace AspNetCore.MVC.RESTful.Controllers
{
    public interface IResourceId
    {
        Guid Id { get; set; }
    }
}