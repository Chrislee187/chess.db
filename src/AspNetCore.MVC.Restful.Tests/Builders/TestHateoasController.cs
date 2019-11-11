using AspNetCore.MVC.RESTful.Controllers;

namespace AspNetCore.MVC.Restful.Tests.Builders
{
    public class TestHateoasController : HateoasController<TestEntity>
    {
        public TestHateoasController(string entityName) : base(entityName)
        {
            Url = new TestUrlHelper();
        }
    }
}