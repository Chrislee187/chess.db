using System;
using chess.db.webapi.Helpers;
using chess.db.webapi.ResourceParameters;
using chess.games.db.api;

namespace chess.db.webapi.Controllers
{
    public class ResourceUriBuilder : IResourceUris
    {
        public string Previous { get; }
        public string Next { get; }
        public ResourceUriBuilder(IPaginationMetadata pagination,
            CommonResourceParameters common,
            Func<object, string> urlBuilder)
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