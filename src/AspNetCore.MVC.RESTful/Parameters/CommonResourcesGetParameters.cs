
namespace AspNetCore.MVC.RESTful.Parameters
{
    public abstract class CommonResourcesGetParameters 
    {
        public string SearchQuery { get; set; }

        public string OrderBy { get; set; }

        public string Shape { get; set; } = "";
    }

}