using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using AspNetCore.MVC.RESTful.Parameters;

namespace AspNetCore.MVC.RESTful.Helpers
{
    public class PagedList<T> : List<T>, IPaginationMetadata
    {
        public int CurrentPage { get; }
        public int TotalPages { get; }
        public int PageSize { get; }
        public int TotalCount { get; }
        public bool HasPrevious => (CurrentPage > 1);
        public bool HasNext => (CurrentPage < TotalPages);

        public PagedList(
            [NotNull] IQueryable<T> items, 
            [NotNull] PaginationParameters pagination)
        {
            TotalCount = items.Count();
            PageSize = pagination.PageSize;
            CurrentPage = pagination.Page;
            TotalPages = (int)Math.Ceiling(TotalCount / (double)PageSize);
            AddRange(items.Skip((CurrentPage - 1) * PageSize).Take(PageSize));
        }
    }
}