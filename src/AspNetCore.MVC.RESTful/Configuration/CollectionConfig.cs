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
        /// defaults to <see cref="DefaultPageSize"/>.
        ///
        /// Set by querystring parameters:
        /// <code>?page-size=x</code>
        /// <code>?pagesize=x</code>
        /// </summary>
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = value < 1 || value > MaxPageSize ? MaxPageSize : value;
        }

        /// <summary>
        /// Current page
        ///
        /// Set by querystring parameters:
        /// <code>?page=x</code>
        /// <code>?currentpage=x</code>
        /// <code>?current-page=x</code>
        /// <code>?pagenumber=x</code>
        /// <code>?page-number=x</code>
        /// </summary>
        public int Page { get; set; } = 1;

        /// <summary>
        /// 
        /// List of resource property names and optional order direction fields.
        /// 
        /// Set by querystring parameters:
        /// <code>?order-by=name [asc|desc],...</code>
        /// <code>?orderby=name [asc|desc],...</code>
        /// </summary>
        public string OrderBy { get; set; }

        /// <summary>
        /// 
        /// List of resource property names to be included in the resource output.
        /// Note that NOT including the Id will disable Hateoas links in resource output.
        /// 
        /// Set by querystring parameters:
        /// <code>?shape=name,...</code>
        /// </summary>
        public string Shape { get; set; }

        /// <summary>
        /// 
        /// Text that will supplied to the <see cref="IEntitySearch{TEntities}"/> implementation
        /// 
        /// Set by querystring parameters:
        /// <code>?searchy=text</code>
        /// <code>?search-text=text</code>
        /// <code>?search-text=text</code>
        /// </summary>
        public string SearchText { get; set; }

        /// <summary>
        /// Appends any used <see cref="CollectionConfig "/> values that have been set to the supplied
        /// URL.
        /// </summary>
        /// <param name="url">URL to append the parameters to</param>
        /// <returns>URL with additional collection arguments appended</returns>
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