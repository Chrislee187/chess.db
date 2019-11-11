using AspNetCore.MVC.RESTful.Controllers;

namespace AspNetCore.MVC.RESTful.Configuration
{
    public class HateoasConfig
    {
        public bool AddLinksToCollectionResources { get; set; } = true;
        public bool AddLinksToIndividualResources { get; set; } = true;


        public string ResourcesGetRouteName { get; set; }
        public string ResourceGetRouteName{ get; set; }
        public string ResourceCreateRouteName{ get; set; }
        public string ResourceUpsertRouteName{ get; set; }
        public string ResourcePatchRouteName{ get; set; }
        public string ResourceDeleteRouteName{ get; set; }

        public HateoasConfig(string entityName)
        {
            ResourcesGetRouteName = $"Get{entityName}s";
            ResourceGetRouteName = $"Get{entityName}";
            ResourceCreateRouteName = $"Create{entityName}";
            ResourceUpsertRouteName = $"Upsert{entityName}";
            ResourcePatchRouteName = $"Patch{entityName}";
            ResourceDeleteRouteName = $"Delete{entityName}";
        }
    }
}