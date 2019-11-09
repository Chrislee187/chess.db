using System;
using System.Text.Encodings.Web;
using System.Text.Json;
using AspNetCore.MVC.RESTful.Parameters;

namespace AspNetCore.MVC.RESTful.Helpers
{
    public class XPaginationHeader
    {
        public string Key { get; set; }
        public string Value { get; set; }

        public XPaginationHeader(
            IPaginationMetadata pagination,
            CommonResourceParameters common,
            Func<object, string> urlBuilder
            )
        {
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
            CommonResourceParameters common,
            ResourceUriType type,
            Func<object, string> urlBuilder
            
            )
        {
            var currentPage = common.PageNumber;
            switch (type)
            {
                case ResourceUriType.PreviousPage:
                    common.PageNumber -= 1;
                    break;
                case ResourceUriType.NextPage:
                    common.PageNumber += 1;
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }

            var link = urlBuilder(common);

            common.PageNumber = currentPage;
            return link;
        }
    }
}