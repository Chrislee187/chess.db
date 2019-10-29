namespace chess.games.db.api.Players
{
    public class PaginationParameters
    {
        private const int MaxPageSize = 100;
        private const int DefaultPageSize = 20;

        public static PaginationParameters Default { get; } = new PaginationParameters() { PageSize = DefaultPageSize, Page = 1};

        public int PageSize { get; set; }
        public int Page { get; set; }
    }
}