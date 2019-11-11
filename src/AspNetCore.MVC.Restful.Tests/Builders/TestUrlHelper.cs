using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;

namespace AspNetCore.MVC.Restful.Tests.Builders
{
    public class TestUrlHelper : IUrlHelper
    {
        public string Action(UrlActionContext actionContext)
        {
            throw new System.NotImplementedException();
        }

        public string Content(string contentPath)
        {
            throw new System.NotImplementedException();
        }

        public bool IsLocalUrl(string url)
        {
            throw new System.NotImplementedException();
        }

        public string Link(string routeName, object values)
        {
            return "";
        }

        public string RouteUrl(UrlRouteContext routeContext)
        {
            throw new System.NotImplementedException();
        }

        public ActionContext ActionContext { get; }
    }
}