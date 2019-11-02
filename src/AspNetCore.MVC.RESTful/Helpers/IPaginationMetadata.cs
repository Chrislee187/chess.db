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
}