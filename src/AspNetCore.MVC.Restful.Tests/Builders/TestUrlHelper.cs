using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;

namespace AspNetCore.MVC.Restful.Tests.Builders
{
    public class TestUrlHelper : IUrlHelper
    {
        public string Link(string routeName, object values) => "";
        
        public string Action(UrlActionContext actionContext) => throw new System.NotImplementedException();

        public string Content(string contentPath) => throw new System.NotImplementedException();

        public bool IsLocalUrl(string url) => throw new System.NotImplementedException();


        public string RouteUrl(UrlRouteContext routeContext) => throw new System.NotImplementedException();

        // ReSharper disable once UnassignedGetOnlyAutoProperty
        public ActionContext ActionContext { get; }
    }
}