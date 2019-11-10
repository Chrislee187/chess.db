namespace AspNetCore.MVC.RESTful.Parameters
{
    public class PaginationParameters
    {
        private int _pageSize = DefaultPageSize;
        private const int MaxPageSize = 100;
        private const int DefaultPageSize = 20;

        public static PaginationParameters Default { get; } = new PaginationParameters() { PageSize = DefaultPageSize, Page = 1};

        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = value > MaxPageSize ? MaxPageSize : value;
        }

        public int Page { get; set; } = 1;

        public string AppendToUrl(string url)
        {
            string paramsStart = "?";
            if (url.Contains("?"))
            {
                paramsStart = "&";
            }

            url += $"{paramsStart}page={Page}&page-size={PageSize}";
            return url;
        }
    }
}