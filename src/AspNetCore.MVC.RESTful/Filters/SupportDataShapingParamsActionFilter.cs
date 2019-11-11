using AspNetCore.MVC.RESTful.Configuration;
using AspNetCore.MVC.RESTful.Controllers;
using AspNetCore.MVC.RESTful.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;

namespace AspNetCore.MVC.RESTful.Filters
{   
    /// <summary>
    ///  Checks if the controller is a HateoasController and for
    /// a `shape`query string parameter. If found, sets 
    /// <see cref="RestfulConfig.Shape"/>
    /// </summary>
    public class SupportDataShapingParamsActionFilter : ActionFilterAttribute
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
            var val = queryCollection.GetByAlias("shape", "data-shape");
            contextController.Restful.Shape = val;
        }
    }
}