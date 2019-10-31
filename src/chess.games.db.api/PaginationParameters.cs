namespace chess.games.db.api
{
    public class PaginationParameters
    {
        private int _pageSize;
        private const int MaxPageSize = 100;
        private const int DefaultPageSize = 20;

        public static PaginationParameters Default { get; } = new PaginationParameters() { PageSize = DefaultPageSize, Page = 1};

        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = value > MaxPageSize ? MaxPageSize : value;
        }

        public int Page { get; set; }
    }
}