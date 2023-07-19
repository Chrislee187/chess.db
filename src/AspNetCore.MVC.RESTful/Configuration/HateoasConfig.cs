using AspNetCore.MVC.RESTful.Controllers;
using Microsoft.AspNetCore.JsonPatch;

namespace AspNetCore.MVC.RESTful.Configuration
{
    /// <summary>
    /// Configuration values for the Hateoas features of the <see cref="ResourceControllerBase{TDto,TEntity,TId}"/>
    /// </summary>
    public class HateoasConfig
    {
        /// <summary>
        /// Whether to add Hateoas links to individual resources
        /// </summary>
        public bool AddLinks { get; set; } = true;

        /// <summary>
        /// RouteName for <code>GET /{EntityName}s</code>
        /// Default name of the route that is used to get the full resource collection (note pagination defaults will prevent the full list actually being returned),
        /// defaults to <code>Get{EntityName}s</code> 
        /// </summary>
        public string ResourcesGetRouteName { get; set; }
        /// <summary>
        /// RouteName for <code>GET /{EntityName}s/{id}</code>
        /// Default name of the route that is used to get individual resource,
        /// defaults to <code>Get{EntityName}</code> 
        /// </summary>
        public string ResourceGetRouteName{ get; set; }
        /// <summary>
        /// RouteName for <code>POST /{EntityName}s</code>
        /// Default name of the route that is used to create new individual resources
        /// defaults to <code>Create{EntityName}</code>
        ///
        /// Request body should contained the serialised representation of the resource
        /// </summary>
        public string ResourceCreateRouteName{ get; set; }
        /// <summary>
        /// RouteName for <code>PUT /{EntityName}s</code>
        /// Default name of the route that is used to upsert new individual resources
        /// defaults to <code>Upsert{EntityName}</code> 
        /// Request body should contained the serialised representation of the resource
        ///
        /// Note that CollectionConfig PUT functionality dictates that's any fields not present
        /// in the body will be set to their default value.
        /// </summary>
        public string ResourceUpsertRouteName{ get; set; }
        /// <summary>
        /// RouteName for <code>PATCH /{EntityName}s</code>
        /// Default name of the route that is used to patch (update one or more individual
        /// fields without worrying about default missing fields) new individual resources
        /// defaults to <code>Patch{EntityName}</code> 
        /// Request body should contained the serialised representation of a <see cref="JsonPatchDocument{TModel}"/>
        /// </summary>
        public string ResourcePatchRouteName{ get; set; }
        /// <summary>
        /// RouteName for <code>DELETE /{EntityName}s/{id}</code>
        /// Default name of the route that is used to delete individual resources
        /// defaults to <code>Delete{EntityName}</code> 
        /// </summary>
        public string ResourceDeleteRouteName{ get; set; }

        /// <summary>
        /// Name of the property containing the Hateoas links in the serialized output.
        /// </summary>
        public string LinksPropertyName { get; set; } = "_links";


        /// <summary>
        /// Default strings used for the the default Hateoas links "rel" properties
        /// </summary>
        private static readonly RelationshipNames Relations = new RelationshipNames();
        public static RelationshipNames Relationships { get; } = Relations;

        public HateoasConfig(string entityName)
        {
            ResourcesGetRouteName = $"Get{entityName}s";
            ResourceGetRouteName = $"Get{entityName}";
            ResourceCreateRouteName = $"Create{entityName}";
            ResourceUpsertRouteName = $"Upsert{entityName}";
            ResourcePatchRouteName = $"Patch{entityName}";
            ResourceDeleteRouteName = $"Delete{entityName}";
        }
        
        /// <summary>
        /// Default names used for the Hateoas link "rel" properties.
        /// </summary>
        public class RelationshipNames
        {
            public string Self { get; set; } = "self";
            public string Create { get; set; } = "create";
            public string Upsert { get; set; } = "upsert";
            public string Patch { get; set; } = "patch";
            public string Delete { get; set; } = "delete";
            public string CurrentPage { get; set; } = "current-page";
            public string NextPage { get; set; } = "next-page";
            public string PreviousPage { get; set; } = "prev-page";
        }
    }
}