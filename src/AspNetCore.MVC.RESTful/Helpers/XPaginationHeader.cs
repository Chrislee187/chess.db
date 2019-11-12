using System;
using System.Collections.Generic;
using System.Text.Encodings.Web;
using System.Text.Json;
using AspNetCore.MVC.RESTful.Configuration;
using Microsoft.Extensions.Primitives;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace AspNetCore.MVC.RESTful.Helpers
{
    /// <summary>
    /// 
    /// </summary>
    public class XPaginationHeader 
    {
        private readonly CollectionConfig _collectionConfig;

        public KeyValuePair<string, StringValues> Value { get; }

        public XPaginationHeader(
            IPaginationMetadata pagination,
            Func<object, string> urlBuilder, 
            CollectionConfig collectionConfig)
        {
            _collectionConfig = collectionConfig;
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

            var key = "X-Pagination";
            var value = JsonSerializer.Serialize(metadata,
                new JsonSerializerOptions()
                {
                    // NOTE: Stops the '?' & '&' chars in the links being escaped
                    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                    IgnoreNullValues = true
                });

            Value = new KeyValuePair<string, StringValues>(key, value);
        }

        private string CreatePlayersResourceUri(ResourceUriType type,
            Func<object, string> urlBuilder
            )
        {
            var originalPage = _collectionConfig.Page;
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

            _collectionConfig.Page = page;
            var link = _collectionConfig.AppendToUrl(urlBuilder(null));
            
            _collectionConfig.Page = originalPage;
            return link;
        }

    }
}