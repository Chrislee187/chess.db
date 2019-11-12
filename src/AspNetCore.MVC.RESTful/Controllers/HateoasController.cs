﻿using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using AspNetCore.MVC.RESTful.Configuration;
using AspNetCore.MVC.RESTful.Filters;
using AspNetCore.MVC.RESTful.Helpers;
using AspNetCore.MVC.RESTful.Models;

namespace AspNetCore.MVC.RESTful.Controllers
{
    public class HateoasController : ControllerBase
    {
        public readonly HateoasConfig HateoasConfig;
        public readonly CollectionConfig CollectionConfig;

        protected HateoasController(string entityName)
        {
            HateoasConfig = new HateoasConfig(entityName);
            CollectionConfig = new CollectionConfig();
        }
    }
    /// <summary>
    /// Base functionality required for Hateoas links support.
    /// Links can be enabled/disabled at Controller level (see <see cref="Configuration.HateoasConfig"/>)
    /// and also on a per call level <see cref="DisableHateoasLinksActionFilter"/>)
    /// </summary>
    public abstract class HateoasController<TEntity, TId> : HateoasController
    {

        protected HateoasController() : base(typeof(TEntity).Name) 
        { }

        /// <summary>
        /// Factory method to build <see cref="HateoasLink"/>'s for resource collection responses
        /// </summary>
        /// <typeparam name="TParameters"></typeparam>
        /// <param name="parameters"></param>
        /// <param name="pagination"></param>
        /// <returns></returns>
        public List<HateoasLink> ResourcesGetLinks<TParameters>(
            TParameters parameters,
            IPaginationMetadata pagination)
        {
            var currentPage = CollectionConfig.Page;
            var links = new List<HateoasLink>
            {
                ResourcesGetLinkBuilder(HateoasConfig.Relationships.CurrentPage, parameters),
            };
            
            if (pagination.HasPrevious)
            {
                CollectionConfig.Page = currentPage - 1;
                links.Add(ResourcesGetLinkBuilder(HateoasConfig.Relationships.PreviousPage, parameters));
            }

            if (pagination.HasNext)
            {
                CollectionConfig.Page = currentPage + 1;
                links.Add(ResourcesGetLinkBuilder(HateoasConfig.Relationships.NextPage, parameters));
            }

            CollectionConfig.Page = currentPage;
            return links;
        }

        /// <summary>
        /// Factory method to build <see cref="HateoasLink"/>'s for individual resource responses
        /// </summary>
        /// <param name="id"></param>
        /// <param name="shape"></param>
        /// <param name="additionalLinks"></param>
        /// <returns></returns>
        public List<HateoasLink> ResourceGetLinks(TId id, string shape,
            IEnumerable<HateoasLink> additionalLinks = null)
        {
            var links = new List<HateoasLink>
            {
                ResourceGetLinkBuilder(HateoasConfig.Relationships.Self, id, shape),
                ResourceUpsertLinkBuilder(id),
                ResourcePatchLinkBuilder(id),
                ResourceDeleteLinkBuilder(id)
            };

            AddCustomLinks(links, additionalLinks);

            return links;
        }

        /// <summary>
        /// Factory method to build <see cref="HateoasLink"/>'s for new resource creation responses
        /// </summary>
        /// <param name="id"></param>
        /// <param name="additionalLinks"></param>
        /// <returns></returns>
        public List<HateoasLink> ResourceCreateLinks(TId id,
            IEnumerable<HateoasLink> additionalLinks = null)
        {
            var links = new List<HateoasLink>
            {
                ResourceGetLinkBuilder(HateoasConfig.Relationships.Self, id, ""),
                ResourceDeleteLinkBuilder(id)
            };

            AddCustomLinks(links, additionalLinks);

            return links;
        }

        /// <summary>
        /// Factory method to build <see cref="HateoasLink"/>'s for resource upsert responses
        /// </summary>
        /// <param name="id"></param>
        /// <param name="additionalLinks"></param>
        /// <returns></returns>
        public List<HateoasLink> ResourceUpsertLinks(TId id,
            IEnumerable<HateoasLink> additionalLinks = null)
        {
            var links = new List<HateoasLink>
            {
                ResourceGetLinkBuilder(HateoasConfig.Relationships.Self, id, ""),
                ResourceDeleteLinkBuilder(id)
            };

            AddCustomLinks(links, additionalLinks);

            return links;
        }

        /// <summary>
        /// Factory method to build <see cref="HateoasLink"/>'s for resource patch responses
        /// </summary>
        /// <param name="id"></param>
        /// <param name="additionalLinks"></param>
        /// <returns></returns>
        public List<HateoasLink> ResourcePatchLinks(TId id,
            IEnumerable<HateoasLink> additionalLinks = null)
        {
            var links = new List<HateoasLink>
            {
                ResourceGetLinkBuilder(HateoasConfig.Relationships.Self, id, ""),
                ResourceDeleteLinkBuilder(id)
            };

            AddCustomLinks(links, additionalLinks);

            return links;
        }

        /// <summary>
        /// Factory method to build <see cref="HateoasLink"/>'s for resource delete responses
        /// </summary>
        /// <param name="id"></param>
        /// <param name="additionalLinks"></param>
        /// <returns></returns>
        public List<HateoasLink> ResourceDeleteLinks(IEnumerable<HateoasLink> additionalLinks = null)
        {
            var links = new List<HateoasLink>
            {
                ResourcesGetLinkBuilder(HateoasConfig.Relationships.CurrentPage, null),
                ResourceCreateLinkBuilder()
            };

            AddCustomLinks(links, additionalLinks);

            return links;
        }

        private HateoasLink ResourcesGetLinkBuilder(string rel, object parameters)
        {
            var link = Url.Link(HateoasConfig.ResourcesGetRouteName, parameters);

            link = CollectionConfig.AppendToUrl(link);

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
                HateoasConfig.Relationships.Create,
                "POST",
                $"{Url.Link(HateoasConfig.ResourceCreateRouteName, null)}");

        private HateoasLink ResourceUpsertLinkBuilder(TId id) =>
            new HateoasLink(
                HateoasConfig.Relationships.Update,
                "PUT",
                $"{Url.Link(HateoasConfig.ResourceUpsertRouteName, new { id })}");

        private HateoasLink ResourcePatchLinkBuilder(TId id) =>
            new HateoasLink(
                HateoasConfig.Relationships.Patch,
                "PATCH",
                $"{Url.Link(HateoasConfig.ResourcePatchRouteName, new { id })}");

        private HateoasLink ResourceDeleteLinkBuilder(TId id) =>
            new HateoasLink(
                HateoasConfig.Relationships.Delete,
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
                // NOTE: Hateoas support is only available if the ID is available, if
                // the data has been reshaped to not include the Id, no links will be added.
                if (resource.TryGetValue("Id", out var idObj))
                {
                    var links = (IEnumerable<HateoasLink>) ResourceGetLinks(
                        (dynamic) idObj,
                        CollectionConfig.Shape,
                        additionalLinks);

                    if (links.Any())
                    {
                        resource.Add(HateoasConfig.LinksPropertyName, links);
                    }
                }

            }
        }

        protected void ConfigureHateoas(Action<HateoasConfig> config) 
            => config?.Invoke(HateoasConfig);
    }
}