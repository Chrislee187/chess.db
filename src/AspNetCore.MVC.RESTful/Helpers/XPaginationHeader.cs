using System;
using System.Text.Encodings.Web;
using System.Text.Json;
using AspNetCore.MVC.RESTful.Parameters;

namespace AspNetCore.MVC.RESTful.Helpers
{
    public class XPaginationHeader
    {
        private readonly PaginationParameters _paginationParameters;
        public string Key { get; set; }
        public string Value { get; set; }

        public XPaginationHeader(IPaginationMetadata pagination,
            CommonResourcesGetParameters common,
            Func<object, string> urlBuilder, PaginationParameters paginationParameters)
        {
            _paginationParameters = paginationParameters;
            var metadata = new
            {
                pagination.TotalCount,
                pagination.PageSize,
                pagination.CurrentPage,
                pagination.TotalPages,
                PreviousPage = pagination.HasPrevious
                    ? CreatePlayersResourceUri(common, ResourceUriType.PreviousPage, urlBuilder)
                    : null,
                NextPage = pagination.HasNext
                    ? CreatePlayersResourceUri(common, ResourceUriType.NextPage, urlBuilder)
                    : null
            };

            Key = "X-Pagination";
            Value = JsonSerializer.Serialize(metadata,
                new JsonSerializerOptions()
                {
                    // NOTE: Stops the '?' & '&' chars in the links being escaped
                    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                    IgnoreNullValues = true
                });
        }

        private string CreatePlayersResourceUri(
            CommonResourcesGetParameters common,
            ResourceUriType type,
            Func<object, string> urlBuilder
            
            )
        {
            var originalPage = _paginationParameters.Page;
            var page = originalPage;
            switch (type)
            {
                case ResourceUriType.PreviousPage:
                    page -= 1;
                    break;
                case ResourceUriType.NextPage:
                    page += 1;
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }

            _paginationParameters.Page = page;
            var link = _paginationParameters.AppendToUrl(urlBuilder(common));
            _paginationParameters.Page = originalPage;

            return link;
        }
    }
}