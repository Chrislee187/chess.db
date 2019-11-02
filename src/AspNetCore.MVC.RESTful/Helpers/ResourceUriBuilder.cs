using System;
using System.Diagnostics.CodeAnalysis;
using AspNetCore.MVC.RESTful.Parameters;

namespace AspNetCore.MVC.RESTful.Helpers
{
    public class ResourceUriBuilder : IResourceUris
    {
        public string Previous { get; }
        public string Next { get; }
        public ResourceUriBuilder(
            [NotNull] IPaginationMetadata pagination,
            [NotNull] CommonResourceParameters common,
            [NotNull] Func<object, string> urlBuilder)
        {

            Previous = pagination.HasPrevious
                ? CreatePlayersResourceUri(common, ResourceUriType.PreviousPage, urlBuilder)
                : null;
            Next = pagination.HasNext
                ? CreatePlayersResourceUri(common, ResourceUriType.NextPage, urlBuilder)
                : null;
        }

        private string CreatePlayersResourceUri(
            CommonResourceParameters common,
            ResourceUriType type, 
            Func<object, string> urlBuilder)
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