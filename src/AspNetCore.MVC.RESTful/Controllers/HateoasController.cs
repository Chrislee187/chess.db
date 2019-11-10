using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using AspNetCore.MVC.RESTful.Configuration;
using AspNetCore.MVC.RESTful.Helpers;
using AspNetCore.MVC.RESTful.Models;
using AspNetCore.MVC.RESTful.Parameters;
using Microsoft.AspNetCore.Mvc;

namespace AspNetCore.MVC.RESTful.Controllers
{
    /// <summary>
    /// Base functionality required for Hateoas links support.
    /// Links can be enabled/disabled at Controller level (see <see cref="Controllers.HateoasConfig"/>)
    /// and also on a per call level <see cref="EnableHateoasLinksActionFilter"/>)
    /// </summary>
    public abstract class HateoasController : ControllerBase
    {
        public readonly HateoasConfig HateoasConfig;

        protected HateoasController(string entityName)
        {
            HateoasConfig = new HateoasConfig(entityName);
        }

        protected List<HateoasLink> ResourcesGetLinks<TParameters>(
            TParameters parameters,
            IPaginationMetadata pagination)
            where TParameters : CommonResourcesGetParameters
        {
            var links = new List<HateoasLink>
            {
                ResourcesGetLinkBuilder("current-page", parameters)
            };

            if (pagination.HasPrevious)
            {
                parameters.PageNumber = pagination.CurrentPage - 1;
                links.Add(ResourcesGetLinkBuilder("prev-page", parameters));
            }

            if (pagination.HasNext)
            {
                parameters.PageNumber = pagination.CurrentPage + 1;
                links.Add(ResourcesGetLinkBuilder("next-page", parameters));
            }

            parameters.PageNumber = pagination.CurrentPage;

            return links;
        }

        public List<HateoasLink> ResourceGetLinks(Guid id, string shape,
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

        public List<HateoasLink> ResourceCreateLinks(Guid id,
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

        public List<HateoasLink> ResourceUpsertLinks(Guid id,
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

        public List<HateoasLink> ResourcePatchLinks(Guid id,
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
            => new HateoasLink(
                rel,
                "GET",
                Url.Link(HateoasConfig.ResourcesGetRouteName.Get(), parameters)
            );

        private HateoasLink ResourceGetLinkBuilder(string rel, Guid id, string shape)
        {
            var s = string.IsNullOrEmpty(shape) ? "" : $"?shape={shape}";

            return new HateoasLink(
                rel,
                "GET",
                $"{Url.Link(HateoasConfig.ResourceGetRouteName.Get(), new { id })}{s}");
        }

        private HateoasLink ResourceCreateLinkBuilder() =>
            new HateoasLink(
                "create",
                "POST",
                $"{Url.Link(HateoasConfig.ResourceCreateRouteName.Get(), null)}");

        private HateoasLink ResourceUpsertLinkBuilder(Guid id) =>
            new HateoasLink(
                "update",
                "PUT",
                $"{Url.Link(HateoasConfig.ResourceUpsertRouteName.Get(), new { id })}");

        private HateoasLink ResourcePatchLinkBuilder(Guid id) =>
            new HateoasLink(
                "patch",
                "PATCH",
                $"{Url.Link(HateoasConfig.ResourcePatchRouteName.Get(), new { id })}");

        private HateoasLink ResourceDeleteLinkBuilder(Guid id) =>
            new HateoasLink(
                "delete",
                "DELETE",
                $"{Url.Link(HateoasConfig.ResourceDeleteRouteName.Get(), new { id })}");

        protected static void AddCustomLinks(List<HateoasLink> links, IEnumerable<HateoasLink> additionalLinks)
        {
            var hateoasLinks = additionalLinks?.ToArray() ?? new List<HateoasLink>().ToArray();
            if (hateoasLinks.Any())
            {
                links.AddRange(hateoasLinks);
            }
        }

        protected void AddHateoasLinksToResourceCollection<TParameters>(
            IEnumerable<ExpandoObject> resources,
            TParameters usedParameters, 
            IEnumerable<HateoasLink> additionalIndividualLinks)
            where TParameters : CommonResourcesGetParameters
        {
            var individualLinks = additionalIndividualLinks?.ToList() ?? new List<HateoasLink>();

            foreach (IDictionary<string, object> resource in resources)
            {
                // NOTE: Hateoas "_links" support is only available if the ID is available, if
                // the data has been reshaped to not include the Id, no links will be added.
                if (resource.TryGetValue("Id", out var idObj))
                {
                    var links = ResourceGetLinks(
                        new Guid(idObj.ToString()),
                        usedParameters.Shape,
                        individualLinks);

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