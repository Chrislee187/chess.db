using System;
using System.Collections.Generic;
using System.Linq;
using AspNetCore.MVC.RESTful.Parameters;

namespace AspNetCore.MVC.RESTful.Helpers
{
    public interface IPaginationMetadata
    {
        int CurrentPage { get; }
        int TotalPages { get; }
        int PageSize { get; }
        int TotalCount { get; }
        bool HasPrevious { get; }
        bool HasNext { get; }
    }

    public class PagedList<T> : List<T>, IPaginationMetadata
    {
        public int CurrentPage { get; }
        public int TotalPages { get; }
        public int PageSize { get; }
        public int TotalCount { get; }
        public bool HasPrevious => (CurrentPage > 1);
        public bool HasNext => (CurrentPage < TotalPages);

        public PagedList(List<T> items, int count, int pageNumber, int pageSize)
        {
            TotalCount = count;
            PageSize = pageSize;
            CurrentPage = pageNumber;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            AddRange(items);
        }

        public static PagedList<T> Create(IQueryable<T> source, PaginationParameters pages)
            => Create(source, pages.Page, pages.PageSize);

        public static PagedList<T> Create(IQueryable<T> source, int pageNumber, int pageSize)
        {
            return new PagedList<T>(
                source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList(), 
                source.Count(), 
                pageNumber, 
                pageSize);
        }
    }
}