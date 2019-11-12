using System.Deployment.Application;
using AspNetCore.MVC.RESTful.Filters;

namespace AspNetCore.MVC.RESTful.Configuration
{
    /// <summary>
    /// Model to manage the parameters required by <see cref="SupportCollectionParamsActionFilter"/>
    /// </summary>
    public class CollectionConfig
    {
        private int _pageSize = DefaultPageSize;
        public const int MaxPageSize = 100;
        public const int DefaultPageSize = 20;

        /// <summary>
        /// Page size, must greater than zero and cannot be larger than <see cref="MaxPageSize"/>,
        /// defaults to <see cref="DefaultPageSize"/>
        /// </summary>
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = value < 1 || value > MaxPageSize ? MaxPageSize : value;
        }

        /// <summary>
        /// Current page
        /// </summary>
        public int Page { get; set; } = 1;

        /// <summary>
        /// <code>?order-by=name [asc|desc],...</code>
        /// List of resource property names and optional order direction fields. 
        /// </summary>
        public string OrderBy { get; set; }

        /// <summary>
        /// <code>?shape=name,...</code>
        /// List of resource property names to be included in the resource output.
        /// Note that NOT including the Id will disable Hateoas links in resource output.
        /// </summary>
        public string Shape { get; set; }

        /// <summary>
        /// <code>?search-query=text</code>
        /// Text that will supplied to the <see cref="IEntitySearch{TEntities}"/> implementation
        /// </summary>
        public string SearchText { get; set; }

        /// <summary>
        /// Appends any used arguments on the supplied url
        /// </summary>
        /// <param name="url"></param>
        /// <returns>URL with additional collection arguments added</returns>
        public string AppendToUrl(string url)
        {
            var paramsStart = "?";
            if (url.Contains("?"))
            {
                paramsStart = "&";
            }

            url += $"{paramsStart}page={Page}&page-size={PageSize}";

            if (!string.IsNullOrEmpty(OrderBy))
            {
                url += $"&order-by={OrderBy}";
            }
            if (!string.IsNullOrEmpty(Shape))
            {
                url += $"&shape={Shape}";
            }
            if (!string.IsNullOrEmpty(SearchText))
            {
                url += $"&search={SearchText}";
            }
            return url;
        }
    }
}