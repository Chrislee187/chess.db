using System.Linq;
using System.Net.Http;
using System.Text.Json;

namespace chess.db.webapi.integration.tests
{
    public static class ResponseHelperExtensions
    {
        public class Pagination
        {
            public int PageSize { get; set; }
            public int TotalCount { get; set; }
            public int CurrentPage { get; set; }
            public int TotalPages { get; set; }
            public string NextPage { get; set; }
            public string PreviousPage { get; set; }
        }
        public static Pagination PaginationValues(this HttpResponseMessage response)
        {
            if (!response.Headers.Contains("X-Pagination")) return null;

            var value = response
                .Headers.GetValues("X-Pagination")
                .First();

            var json = JsonDocument.Parse(value);

            return new Pagination
            {
                PreviousPage = json.Element("PreviousPage")?.ToString() ?? "",
                NextPage = json.Element("NextPage")?.ToString() ?? "",
                PageSize = json.Element("PageSize")?.GetInt32() ?? 0,
                TotalCount = json.Element("TotalCount")?.GetInt32() ?? 0,
                CurrentPage = json.Element("CurrentPage")?.GetInt32() ?? 0,
                TotalPages = json.Element("TotalPages")?.GetInt32() ?? 0,
            };
        }

        private static JsonElement? Element(this JsonDocument pagination, string key)
        {
            if (pagination.RootElement.TryGetProperty(key, out var prev))
            {
                return prev;
            }
            else
            {
                return null;
            }
        }
    }
}