using System;
using AspNetCore.MVC.RESTful.Controllers;

namespace AspNetCore.MVC.Restful.Tests.Builders
{
    public class TestHateoasController : HateoasController<TestEntity, Guid>
    {
        public TestHateoasController()
        {
            Url = new TestUrlHelper();
        }
    }
}