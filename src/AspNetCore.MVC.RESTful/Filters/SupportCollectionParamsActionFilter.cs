using AspNetCore.MVC.RESTful.Configuration;
using AspNetCore.MVC.RESTful.Controllers;
using AspNetCore.MVC.RESTful.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;

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
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
            if (context.Controller is HateoasController contextController)
            {
                var queryCollection = contextController.Request.Query;

                GetPageSize(queryCollection, contextController);
                GetCurrentPage(queryCollection, contextController);
                GetOrderBy(queryCollection, contextController);
                GetSearchQuery(queryCollection, contextController);
            }
        }

        private static void GetPageSize(IQueryCollection queryCollection, HateoasController contextController)
        {
            var val = queryCollection.GetByAlias("pagesize", "page-size");

            if (int.TryParse(val, out var pageSize))
            {
                contextController.CollectionConfig.PageSize = pageSize;
            }
        }

        private static void GetCurrentPage(IQueryCollection queryCollection, HateoasController contextController)
        {
            var val = queryCollection.GetByAlias("page", "currentpage", "current-page", "pagenumber", "page-number");

            if (int.TryParse(val, out var page))
            {
                contextController.CollectionConfig.Page = page;
            }
        }

        private static void GetOrderBy(IQueryCollection queryCollection, HateoasController contextController)
        {
            var val = queryCollection.GetByAlias("orderby", "order-by");
            contextController.CollectionConfig.OrderBy = val;
        }

        private static void GetSearchQuery(IQueryCollection queryCollection, HateoasController contextController)
        {
            var val = queryCollection.GetByAlias("search", "search-query", "search-text");
            contextController.CollectionConfig.SearchText = val;
        }
    }
}