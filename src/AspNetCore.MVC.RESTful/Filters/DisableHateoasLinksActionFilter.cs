using AspNetCore.MVC.RESTful.Configuration;
using AspNetCore.MVC.RESTful.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace AspNetCore.MVC.RESTful.Filters
{
    /// <summary>
    ///  Checks if the controller is a HateoasController and for
    /// a `nolinks` query string parameter. If found, sets the
    /// <see cref="HateoasConfig.AddLinks"/> flag to false.
    /// Restores original values after action is executed
    /// </summary>
    public class DisableHateoasLinksActionFilter : IActionFilter
    {
        private bool _originalLinksStatus;

        private readonly string _linksArgName;
        private ILogger<DisableHateoasLinksActionFilter> _logger;

        public DisableHateoasLinksActionFilter(string linksArgName = "nolinks", 
            ILogger<DisableHateoasLinksActionFilter> logger = null)
        {
            _logger = logger ?? NullLogger<DisableHateoasLinksActionFilter>.Instance;
            _linksArgName = linksArgName;
        }
        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (context.Controller is HateoasController contextController)
            {
                _originalLinksStatus = contextController.HateoasConfig.AddLinks;

                if (contextController.Request.Query.ContainsKey(_linksArgName))
                {
                    contextController.HateoasConfig.AddLinks = false;
                }
                _logger.LogDebug($"RESTful Param: {_linksArgName}=" + "{nolinks}", !contextController.HateoasConfig.AddLinks);
            }
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            if (context.Controller is HateoasController contextController)
            {

                contextController.HateoasConfig.AddLinks = _originalLinksStatus;
            }
        }
    }
}