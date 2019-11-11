namespace AspNetCore.MVC.RESTful.Configuration
{
    public class RestfulConfig
    {
        private int _pageSize = DefaultPageSize;
        public const int MaxPageSize = 100;
        public const int DefaultPageSize = 20;

        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = value > MaxPageSize ? MaxPageSize : value;
        }

        public int Page { get; set; } = 1;

        public string OrderBy { get; set; }
        public string Shape { get; set; }
        public string SearchQuery { get; set; }

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
            if (!string.IsNullOrEmpty(SearchQuery))
            {
                url += $"&search-query={SearchQuery}";
            }
            return url;
        }
    }
}