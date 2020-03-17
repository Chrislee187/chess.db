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
    /// a `pagesize`, `page`, 'search' and `orderby` (and some similar aliases) query string parameter.
    /// If found, sets the
    /// <see cref="CollectionConfig.PageSize"/>,
    /// <see cref="CollectionConfig.Page"/> and
    /// <see cref="CollectionConfig.OrderBy"/>
    /// <see cref="CollectionConfig.SearchText"/> properties respectively
    /// </summary>
    public class SupportCollectionParamsActionFilter : ActionFilterAttribute
    {
        private ILogger<SupportCollectionParamsActionFilter> _logger;

        public SupportCollectionParamsActionFilter(ILogger<SupportCollectionParamsActionFilter> logger = null)
        {
            _logger = logger ?? NullLogger<SupportCollectionParamsActionFilter>.Instance;
        }
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
            if (context.Controller is HateoasController contextController)
            {
                var queryCollection = contextController.Request.Query;

                PageSize(queryCollection, contextController);
                CurrentPage(queryCollection, contextController);
                OrderBy(queryCollection, contextController);
                SearchQuery(queryCollection, contextController);

                _logger.LogDebug("RESTful Collection Param: pagesize={pagesize}", contextController.CollectionConfig.PageSize);
                _logger.LogDebug("RESTful Collection Param: page={page}", contextController.CollectionConfig.Page);
                _logger.LogDebug("RESTful Collection Param: orderby={orderby}", contextController.CollectionConfig.OrderBy);
                _logger.LogDebug("RESTful Collection Param: search={search}", contextController.CollectionConfig.SearchText);
            }
        }

        private static void PageSize(IQueryCollection queryCollection, HateoasController contextController)
        {
            var val = queryCollection.ByAlias("pagesize", "page-size");

            if (int.TryParse(val, out var pageSize))
            {
                contextController.CollectionConfig.PageSize = pageSize;
            }
        }

        private static void CurrentPage(IQueryCollection queryCollection, HateoasController contextController)
        {
            var val = queryCollection.ByAlias("page", "currentpage", "current-page", "pagenumber", "page-number");

            if (int.TryParse(val, out var page))
            {
                contextController.CollectionConfig.Page = page;
            }

        }

        private static void OrderBy(IQueryCollection queryCollection, HateoasController contextController)
        {
            var val = queryCollection.ByAlias("orderby", "order-by");
            contextController.CollectionConfig.OrderBy = val.Unwrap('"');
        }

        private static void SearchQuery(IQueryCollection queryCollection, HateoasController contextController)
        {
            var val = queryCollection.ByAlias("search", "search-query", "search-text");
            contextController.CollectionConfig.SearchText = val;
        }
    }
}