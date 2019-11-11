using System;
using System.Text.Encodings.Web;
using System.Text.Json;
using AspNetCore.MVC.RESTful.Configuration;

namespace AspNetCore.MVC.RESTful.Helpers
{
    public class XPaginationHeader
    {
        private readonly RestfulConfig _restful;
        public string Key { get; set; }
        public string Value { get; set; }

        public XPaginationHeader(
            IPaginationMetadata pagination,
            Func<object, string> urlBuilder, 
            RestfulConfig restful)
        {
            _restful = restful;
            var metadata = new
            {
                pagination.TotalCount,
                pagination.PageSize,
                pagination.CurrentPage,
                pagination.TotalPages,
                PreviousPage = pagination.HasPrevious
                    ? CreatePlayersResourceUri(ResourceUriType.PreviousPage, urlBuilder)
                    : null,
                NextPage = pagination.HasNext
                    ? CreatePlayersResourceUri(ResourceUriType.NextPage, urlBuilder)
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

        private string CreatePlayersResourceUri(ResourceUriType type,
            Func<object, string> urlBuilder
            
            )
        {
            var originalPage = _restful.Page;
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

            _restful.Page = page;
            var link = _restful.AppendToUrl(urlBuilder(null));
            _restful.Page = originalPage;

            return link;
        }
    }
}