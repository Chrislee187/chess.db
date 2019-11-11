using AspNetCore.MVC.RESTful.Configuration;
using AspNetCore.MVC.RESTful.Controllers;
using AspNetCore.MVC.RESTful.Helpers;
using AspNetCore.MVC.RESTful.Parameters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;

namespace AspNetCore.MVC.RESTful.Filters
{    /// <summary>
     ///  Checks if the controller is a HateoasController and for
     /// a `pagesize`, `page` and `orderby` query string parameter. If found, sets 
     /// <see cref="RestfulConfig.PageSize"/>, <see cref="RestfulConfig.Page"/> and <see cref="RestfulConfig.OrderBy"/>
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
            }
        }

        private static void GetPageSize(IQueryCollection queryCollection, HateoasController contextController)
        {
            var val = queryCollection.GetByAlias("pagesize", "page-size");

            if (int.TryParse(val, out var pageSize))
            {
                contextController.Restful.PageSize = pageSize;
            }
        }

        private static void GetCurrentPage(IQueryCollection queryCollection, HateoasController contextController)
        {
            var val = queryCollection.GetByAlias("page", "currentpage", "current-page", "pagenumber", "page-number");

            if (int.TryParse(val, out var page))
            {
                contextController.Restful.Page = page;
            }
        }


        private static void GetOrderBy(IQueryCollection queryCollection, HateoasController contextController)
        {
            var val = queryCollection.GetByAlias("orderby", "order-by");
            contextController.Restful.OrderBy = val;
        }
    }
}