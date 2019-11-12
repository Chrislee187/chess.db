using System;
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
                ResourcesGetLinkBuilder(parameters, HateoasConfig.Relationships.CurrentPage),
            };
            
            if (pagination.HasPrevious)
            {
                CollectionConfig.Page = currentPage - 1;
                links.Add(ResourcesGetLinkBuilder(parameters, HateoasConfig.Relationships.PreviousPage));
            }

            if (pagination.HasNext)
            {
                CollectionConfig.Page = currentPage + 1;
                links.Add(ResourcesGetLinkBuilder(parameters, HateoasConfig.Relationships.NextPage));
            }

            CollectionConfig.Page = currentPage;
            return links;
        }

        /// <summary>
        /// Factory method to build <see cref="HateoasLink"/>'s for individual resource responses
        /// </summary>
        /// <param name="id"></param>
        /// <param name="shape"></param>
        /// <returns></returns>
        public List<HateoasLink> ResourceGetLinks(TId id, string shape)
        {
            var links = new List<HateoasLink>
            {
                ResourceGetLinkBuilder(id, shape: shape, rel: HateoasConfig.Relationships.Self),
                ResourceUpsertLinkBuilder(id),
                ResourcePatchLinkBuilder(id),
                ResourceDeleteLinkBuilder(id)
            };

            return links;
        }

        /// <summary>
        /// Factory method to build <see cref="HateoasLink"/>'s for new resource creation responses
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public List<HateoasLink> ResourceCreateLinks(TId id)
        {
            var links = new List<HateoasLink>
            {
                ResourceGetLinkBuilder(id, shape: "", rel: HateoasConfig.Relationships.Self),
                ResourceDeleteLinkBuilder(id)
            };

            return links;
        }

        /// <summary>
        /// Factory method to build <see cref="HateoasLink"/>'s for resource upsert responses
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public List<HateoasLink> ResourceUpsertLinks(TId id)
        {
            var links = new List<HateoasLink>
            {
                ResourceGetLinkBuilder(id, shape: "", rel: HateoasConfig.Relationships.Self),
                ResourceDeleteLinkBuilder(id)
            };

            return links;
        }

        /// <summary>
        /// Factory method to build <see cref="HateoasLink"/>'s for resource patch responses
        /// </summary>
        /// <param name="id"></param>
        public List<HateoasLink> ResourcePatchLinks(TId id)
        {
            var links = new List<HateoasLink>
            {
                ResourceGetLinkBuilder(id, shape: "", rel: HateoasConfig.Relationships.Self),
                ResourceDeleteLinkBuilder(id)
            };

            return links;
        }

        /// <summary>
        /// Factory method to build <see cref="HateoasLink"/>'s for resource delete responses
        /// </summary>
        /// <returns></returns>
        public List<HateoasLink> ResourceDeleteLinks()
        {
            var links = new List<HateoasLink>
            {
                ResourcesGetLinkBuilder(null, HateoasConfig.Relationships.CurrentPage),
                ResourceCreateLinkBuilder()
            };

            return links;
        }
        
        private HateoasLink ResourcesGetLinkBuilder(object parameters, string rel = null)
        {
            var link = Url.Link(HateoasConfig.ResourcesGetRouteName, parameters);

            link = CollectionConfig.AppendToUrl(link);

            return HateoasLink.GetCollection(link, rel);
        }
        private HateoasLink ResourceGetLinkBuilder(TId id, string shape = null, string rel = null)
        {
            var s = string.IsNullOrEmpty(shape) ? "" : $"?shape={shape}";

            var link = $"{Url.Link(HateoasConfig.ResourceGetRouteName, new { id })}{s}";
            return HateoasLink.Get(link, rel);
        }
        private HateoasLink ResourceCreateLinkBuilder() => HateoasLink.Create($"{Url.Link(HateoasConfig.ResourceCreateRouteName, null)}");
        private HateoasLink ResourceUpsertLinkBuilder(TId id) => HateoasLink.Upsert($"{Url.Link(HateoasConfig.ResourceUpsertRouteName, new { id })}");
        private HateoasLink ResourcePatchLinkBuilder(TId id) => HateoasLink.Patch($"{Url.Link(HateoasConfig.ResourcePatchRouteName, new { id })}");
        private HateoasLink ResourceDeleteLinkBuilder(TId id) => HateoasLink.Delete($"{Url.Link(HateoasConfig.ResourceDeleteRouteName, new { id })}");

        protected void AddHateoasLinksToResourceCollection(
            IEnumerable<ExpandoObject> resources)
        {
            foreach (IDictionary<string, object> resource in resources)
            {
                // NOTE: Hateoas support is only available if the ID is available, if
                // the data has been reshaped to not include the Id, no links will be added.
                if (resource.TryGetValue("Id", out var idObj))
                {
                    var links = (IEnumerable<HateoasLink>) ResourceGetLinks(
                        (dynamic) idObj,
                        CollectionConfig.Shape);

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