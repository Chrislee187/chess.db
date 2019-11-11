using System;
using AspNetCore.MVC.RESTful.Controllers;
using AspNetCore.MVC.RESTful.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;

namespace AspNetCore.MVC.RESTful.Configuration
{
    public class SupportsCollectionParams : ActionFilterAttribute
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
            var pageSizeKeys = new[] {"pagesize", "page-size"};

            var val = queryCollection.ArgValue(pageSizeKeys);

            if (Int32.TryParse(val, out int pageSize))
            {
                contextController.Restful.PageSize = pageSize;
            }
        }

        private static void GetCurrentPage(IQueryCollection queryCollection, HateoasController contextController)
        {
            var currentPageKeys = new[] {"page", "currentpage", "current-page", "pagenumber", "page-number"};
            var val = queryCollection.ArgValue(currentPageKeys);

            if (Int32.TryParse(val, out int page))
            {
                contextController.Restful.Page = page;
            }
        }


        private static void GetOrderBy(IQueryCollection queryCollection, HateoasController contextController)
        {
            var currentPageKeys = new[] {"orderby", "order-by"};
            var val = queryCollection.ArgValue(currentPageKeys);
            contextController.Restful.OrderBy = val;
        }
    }
}