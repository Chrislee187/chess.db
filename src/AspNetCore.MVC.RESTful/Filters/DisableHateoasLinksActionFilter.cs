using AspNetCore.MVC.RESTful.Configuration;
using AspNetCore.MVC.RESTful.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;

namespace AspNetCore.MVC.RESTful.Filters
{
    /// <summary>
    ///  Checks if the controller is a HateoasController and for
    /// a `nolinks` query string parameter. If found, sets the <see cref="HateoasConfig.AddLinksToCollectionResources"/>
    /// and <see cref="HateoasConfig.AddLinksToIndividualResources"/> flags to false.
    ///
    /// Restores original values after action is executed
    /// </summary>
    public class DisableHateoasLinksActionFilter : IActionFilter
    {
        private bool _collectionSaved;
        private bool _individualSaved;

        private readonly string _linksArgName;
        public DisableHateoasLinksActionFilter(string linksArgName = "nolinks")
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