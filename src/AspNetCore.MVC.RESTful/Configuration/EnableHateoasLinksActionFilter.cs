using AspNetCore.MVC.RESTful.Controllers;
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
        public EnableHateoasLinksActionFilter(string linksArgName = "nolinks")
        {
            _linksArgName = linksArgName;
        }
        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (context.Controller is HateoasController contextController)
            {
                _collectionSaved = contextController.HateoasConfig.AddLinksToCollectionResources;
                _individualSaved = contextController.HateoasConfig.AddLinksToIndividualResources;

                if (contextController.Request.Query.ContainsKey(_linksArgName))
                {
                    contextController.HateoasConfig.AddLinksToCollectionResources = false;
                    contextController.HateoasConfig.AddLinksToIndividualResources = false;
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