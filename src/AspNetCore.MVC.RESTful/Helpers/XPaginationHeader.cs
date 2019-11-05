using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text.Encodings.Web;
using System.Text.Json;
using AspNetCore.MVC.RESTful.Models;
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

    public class ResourcesGetLinksBuilder<TParameters> where TParameters : CommonResourceParameters
    {
        private Func<TParameters, string> _resourcesGetLinkBuilder;

        public ResourcesGetLinksBuilder(Func<object, string> resourcesGetLinkBuilder)
        {
            _resourcesGetLinkBuilder = resourcesGetLinkBuilder;
        }

        public List<HateoasLink> ResourcesGetLinks(TParameters parameters,
            IPaginationMetadata pagination,
            IEnumerable<HateoasLink> additionalLinks = null)
        {
            var links = new List<HateoasLink>
            {
                new HateoasLink("current-page", "GET", _resourcesGetLinkBuilder(parameters)),
            };

            if (pagination.HasPrevious)
            {
                parameters.PageNumber = pagination.CurrentPage - 1;
                links.Add(new HateoasLink("prev-page", "GET", _resourcesGetLinkBuilder(parameters)));
            }

            if (pagination.HasNext)
            {
                parameters.PageNumber = pagination.CurrentPage + 1;
                links.Add(new HateoasLink("next-page", "GET", _resourcesGetLinkBuilder(parameters)));
            }



            parameters.PageNumber = pagination.CurrentPage;
            return links;

        }

        private List<HateoasLink> ResourceGetLinks(
            Func<Guid, string, string> resourceGetRouteBuilder,
            Guid id, string shape,
            IEnumerable<HateoasLink> additionalLinks = null)
        {
            var links = new List<HateoasLink>
            {
                new HateoasLink("self", "GET", resourceGetRouteBuilder(id, shape)),
            };
            if (additionalLinks != null)
            {
                links.AddRange(additionalLinks);
            }

            return links;

        }
    }

    public class ResourceGetLinksBuilder<TId>
    {
        private readonly Func<TId, string, string> _resourceGetLinkBuilder;

        public ResourceGetLinksBuilder(Func<TId, string, string> resourceGetLinkBuilder)
        {
            _resourceGetLinkBuilder = resourceGetLinkBuilder;
        }

        public List<HateoasLink> ResourceGetLinks(TId id, string shape,
            IEnumerable<HateoasLink> additionalLinks = null)
        {
            var links = new List<HateoasLink>
            {
                new HateoasLink("self", "GET", _resourceGetLinkBuilder(id, shape)),
            };
            if (additionalLinks != null)
            {
                links.AddRange(additionalLinks);
            }

            return links;

        }
    }
}