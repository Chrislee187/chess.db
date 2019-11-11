using AspNetCore.MVC.RESTful.Helpers;

namespace AspNetCore.MVC.Restful.Tests.Builders
{
    public class PaginationMetadataBuilder
    {
        private int _currentPage = 1;

        private int _totalPages;
        private int _pageSize = 20;
        private int _totalCount = 100;
        private bool _hasPrevious;
        private bool _hasNext;

        public IPaginationMetadata Build()
        {
            return new PaginationMetadata
            {
                CurrentPage = _currentPage,
                TotalPages = _totalPages,
                PageSize = _pageSize,
                TotalCount = _totalCount,
                HasPrevious = _hasPrevious,
                HasNext = _hasNext
            };
        }

        public PaginationMetadataBuilder WithPage(int currentPage)
        {
            _currentPage = currentPage;
            return this;
        }

        public PaginationMetadataBuilder WithPageSize(int pageSize)
        {
            _pageSize = pageSize;
            _totalPages = _totalCount / pageSize;
            return this;
        }

        public PaginationMetadataBuilder WithTotalCount(int totalCount)
        {
            _totalCount = totalCount;
            _totalPages = totalCount / _pageSize;
            return this;
        }

        public PaginationMetadataBuilder HasPrevious()
        {
            _hasPrevious = true;
            return this;
        }

        public PaginationMetadataBuilder HasNext()
        {
            _hasNext = true;
            return this;
        }

        private class PaginationMetadata : IPaginationMetadata {
            public int CurrentPage { get; set; }
            public int TotalPages { get; set; }
            public int PageSize { get; set; }
            public int TotalCount { get; set; }
            public bool HasPrevious { get; set; }
            public bool HasNext { get; set; }
        }

    }
}