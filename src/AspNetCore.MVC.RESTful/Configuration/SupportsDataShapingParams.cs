using AspNetCore.MVC.RESTful.Controllers;
using AspNetCore.MVC.RESTful.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;

namespace AspNetCore.MVC.RESTful.Configuration
{
    public class SupportsDataShapingParams : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
            if (context.Controller is HateoasController contextController)
            {
                var queryCollection = contextController.Request.Query;


                GetShape(queryCollection, contextController);
            }
        }

        private static void GetShape(IQueryCollection queryCollection, HateoasController contextController)
        {
            var currentPageKeys = new[] {"shape"};
            var val = queryCollection.ArgValue(currentPageKeys);
            contextController.Restful.Shape = val;
        }
    }
}