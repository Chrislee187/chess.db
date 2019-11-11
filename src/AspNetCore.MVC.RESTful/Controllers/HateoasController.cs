using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using AspNetCore.MVC.RESTful.Configuration;
using AspNetCore.MVC.RESTful.Filters;
using AspNetCore.MVC.RESTful.Helpers;
using AspNetCore.MVC.RESTful.Models;
using AspNetCore.MVC.RESTful.Parameters;
using Microsoft.AspNetCore.Mvc;

namespace AspNetCore.MVC.RESTful.Controllers
{
    public class HateoasController : ControllerBase
    {
        public readonly HateoasConfig HateoasConfig;
        public readonly RestfulConfig Restful;

        protected HateoasController(string entityName)
        {
            HateoasConfig = new HateoasConfig(entityName);
            Restful = new RestfulConfig();
        }
    }
    /// <summary>
    /// Base functionality required for Hateoas links support.
    /// Links can be enabled/disabled at Controller level (see <see cref="Configuration.HateoasConfig"/>)
    /// and also on a per call level <see cref="DisableHateoasLinksActionFilter"/>)
    /// </summary>
    public abstract class HateoasController<TId> : HateoasController
    {

        protected HateoasController(string entityName) : base(entityName) 
        { }


        protected List<HateoasLink> ResourcesGetLinks<TParameters>(
            TParameters parameters,
            IPaginationMetadata pagination)
        {
            var currentPage = Restful.Page;
            var links = new List<HateoasLink>
            {
                ResourcesGetLinkBuilder("current-page", parameters),
            };
            
            if (pagination.HasPrevious)
            {
                Restful.Page = currentPage - 1;
                links.Add(ResourcesGetLinkBuilder("prev-page", parameters));
            }

            if (pagination.HasNext)
            {
                Restful.Page = currentPage + 1;
                links.Add(ResourcesGetLinkBuilder("next-page", parameters));
            }

            Restful.Page = currentPage;
            return links;
        }

        public List<HateoasLink> ResourceGetLinks(TId id, string shape,
            IEnumerable<HateoasLink> additionalLinks = null)
        {
            var links = new List<HateoasLink>
            {
                ResourceGetLinkBuilder("self", id, shape),
                ResourceUpsertLinkBuilder(id),
                ResourcePatchLinkBuilder(id),
                ResourceDeleteLinkBuilder(id)
            };

            AddCustomLinks(links, additionalLinks);

            return links;
        }

        public List<HateoasLink> ResourceCreateLinks(TId id,
            IEnumerable<HateoasLink> additionalLinks = null)
        {
            var links = new List<HateoasLink>
            {
                ResourceGetLinkBuilder("self", id, ""),
                ResourceDeleteLinkBuilder(id)
            };

            AddCustomLinks(links, additionalLinks);

            return links;
        }

        public List<HateoasLink> ResourceUpsertLinks(TId id,
            IEnumerable<HateoasLink> additionalLinks = null)
        {
            var links = new List<HateoasLink>
            {
                ResourceGetLinkBuilder("self", id, ""),
                ResourceDeleteLinkBuilder(id)
            };

            AddCustomLinks(links, additionalLinks);

            return links;
        }

        public List<HateoasLink> ResourcePatchLinks(TId id,
            IEnumerable<HateoasLink> additionalLinks = null)
        {
            var links = new List<HateoasLink>
            {
                ResourceGetLinkBuilder("self", id, ""),
                ResourceDeleteLinkBuilder(id)
            };

            AddCustomLinks(links, additionalLinks);

            return links;
        }

        public List<HateoasLink> ResourceDeleteLinks(IEnumerable<HateoasLink> additionalLinks = null)
        {
            var links = new List<HateoasLink>
            {
                ResourcesGetLinkBuilder("current-page", null),
                ResourceCreateLinkBuilder()
            };

            AddCustomLinks(links, additionalLinks);

            return links;
        }

        private HateoasLink ResourcesGetLinkBuilder(string rel, object parameters)
        {
            var link = Url.Link(HateoasConfig.ResourcesGetRouteName, parameters);

            link = Restful.AppendToUrl(link);

            return new HateoasLink(
                rel,
                "GET",
                link
            );
        }

        private HateoasLink ResourceGetLinkBuilder(string rel, TId id, string shape)
        {
            var s = string.IsNullOrEmpty(shape) ? "" : $"?shape={shape}";

            return new HateoasLink(
                rel,
                "GET",
                $"{Url.Link(HateoasConfig.ResourceGetRouteName, new { id })}{s}");
        }

        private HateoasLink ResourceCreateLinkBuilder() =>
            new HateoasLink(
                "create",
                "POST",
                $"{Url.Link(HateoasConfig.ResourceCreateRouteName, null)}");

        private HateoasLink ResourceUpsertLinkBuilder(TId id) =>
            new HateoasLink(
                "update",
                "PUT",
                $"{Url.Link(HateoasConfig.ResourceUpsertRouteName, new { id })}");

        private HateoasLink ResourcePatchLinkBuilder(TId id) =>
            new HateoasLink(
                "patch",
                "PATCH",
                $"{Url.Link(HateoasConfig.ResourcePatchRouteName, new { id })}");

        private HateoasLink ResourceDeleteLinkBuilder(TId id) =>
            new HateoasLink(
                "delete",
                "DELETE",
                $"{Url.Link(HateoasConfig.ResourceDeleteRouteName, new { id })}");

        protected static void AddCustomLinks(List<HateoasLink> links, IEnumerable<HateoasLink> additionalLinks)
        {
            var hateoasLinks = additionalLinks?.ToArray() ?? new List<HateoasLink>().ToArray();
            if (hateoasLinks.Any())
            {
                links.AddRange(hateoasLinks);
            }
        }

        protected void AddHateoasLinksToResourceCollection(
            IEnumerable<ExpandoObject> resources, 
            IEnumerable<HateoasLink> additionalLinks)
        {
            additionalLinks = additionalLinks?.ToList() ?? new List<HateoasLink>();

            foreach (IDictionary<string, object> resource in resources)
            {
                // NOTE: Hateoas "_links" support is only available if the ID is available, if
                // the data has been reshaped to not include the Id, no links will be added.
                if (resource.TryGetValue("Id", out var idObj))
                {
                    List<HateoasLink> links = new List<HateoasLink>();
                    links = ResourceGetLinks(
                        (dynamic) idObj,
                        Restful.Shape,
                        additionalLinks);

                    if (links.Any())
                    {
                        resource.Add("_links", links);
                    }
                }

            }
        }

        protected void ConfigureHateoas(Action<HateoasConfig> config) 
            => config?.Invoke(HateoasConfig);
    }
}