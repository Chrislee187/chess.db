using System.ComponentModel.DataAnnotations;

namespace AspNetCore.MVC.RESTful.Parameters
{
    public abstract class CommonResourceParameters {
        private const int MaxPageSize = 100;
        private const int DefaultPageSize = 20;
        private int _pageSize = DefaultPageSize;

        public string SearchQuery { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "pageNumber must be a postive integer.")]
        public int PageNumber { get; set; } = 1;

        [Range(1, MaxPageSize, ErrorMessage = "pageSize is to large")]
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = value > MaxPageSize ? MaxPageSize : value;
        }

        public string OrderBy { get; set; }

        public string Shape { get; set; } = "";
    }
}