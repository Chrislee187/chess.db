namespace AspNetCore.MVC.RESTful.Helpers
{
    /// <summary>
    /// Represents the pagination metadata placed in the <see cref="XPaginationHeader"/>.
    /// </summary>
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