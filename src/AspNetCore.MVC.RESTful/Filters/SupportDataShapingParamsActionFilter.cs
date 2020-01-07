using AspNetCore.MVC.RESTful.Configuration;
using AspNetCore.MVC.RESTful.Controllers;
using AspNetCore.MVC.RESTful.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace AspNetCore.MVC.RESTful.Filters
{   
    /// <summary>
    ///  Checks if the controller is a HateoasController and for
    /// a `shape` query string parameter. If found, sets 
    /// <see cref="CollectionConfig.Shape"/> property
    /// </summary>
    public class SupportDataShapingParamsActionFilter : ActionFilterAttribute
    {
        private ILogger<SupportDataShapingParamsActionFilter> _logger;

        public SupportDataShapingParamsActionFilter(ILogger<SupportDataShapingParamsActionFilter> logger = null)
        {
            _logger = logger ?? NullLogger<SupportDataShapingParamsActionFilter>.Instance;
        }
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
            if (context.Controller is HateoasController contextController)
            {
                var queryCollection = contextController.Request.Query;

                GetShape(queryCollection, contextController);
            }
        }

        private void GetShape(IQueryCollection queryCollection, HateoasController contextController)
        {
            var val = queryCollection.GetByAlias("shape", "data-shape");
            _logger.LogDebug("RESTful Collection Param: shape={shape}", val);
            contextController.CollectionConfig.Shape = val;
        }
    }
}