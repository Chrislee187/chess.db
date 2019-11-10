using AspNetCore.MVC.RESTful.Controllers;
using AspNetCore.MVC.RESTful.Parameters;
using Microsoft.AspNetCore.Mvc.Filters;

namespace AspNetCore.MVC.RESTful.Configuration
{
    /// <summary>
    /// A filter that checks if the controller is a HateoasController and for
    /// a `links=true|false` query string parameter which it uses to enable/disable links
    /// for an individual call to an action.
    ///
    /// NB. Although `links` will accept `true` it as AND'd with controller level values found
    /// in ResourceBaseController.HateoasConfig.AddLinksXXX values, ensuring that links cannot be enabled
    /// for a controller that explicitly has disabled them
    /// </summary>
    public class EnableHateoasLinksActionFilter : IActionFilter
    {
        private bool _collectionSaved;
        private bool _individualSaved;

        private readonly string _linksArgName;
        public EnableHateoasLinksActionFilter(string linksArgName = "links")
        {
            _linksArgName = linksArgName;
        }
        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (context.Controller is HateoasController contextController)
            {
                _collectionSaved = contextController.HateoasConfig.AddLinksToCollectionResources;
                _individualSaved = contextController.HateoasConfig.AddLinksToIndividualResources;

                if (contextController.Request.Query.TryGetValue(_linksArgName, out var showLinks))
                {
                    var links = bool.Parse(showLinks);
                    contextController.HateoasConfig.AddLinksToCollectionResources = links && _collectionSaved;
                    contextController.HateoasConfig.AddLinksToIndividualResources = links && _collectionSaved;
                }
            }
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            if (context.Controller is HateoasController contextController)
            {
                contextController.HateoasConfig.AddLinksToCollectionResources = _collectionSaved;
                contextController.HateoasConfig.AddLinksToIndividualResources = _individualSaved;
            }
        }
    }
}