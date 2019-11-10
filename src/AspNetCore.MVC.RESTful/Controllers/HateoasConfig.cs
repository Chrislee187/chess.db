namespace AspNetCore.MVC.RESTful.Controllers
{
    public class HateoasConfig
    {
        public bool AddLinksToCollectionResources { get; set; } = true;
        public bool AddLinksToIndividualResources { get; set; } = true;


        public readonly MustSet<string> ResourcesGetRouteName = new MustSet<string>(nameof(ResourcesGetRouteName));
        public readonly MustSet<string> ResourceGetRouteName = new MustSet<string>(nameof(ResourceGetRouteName));
        public readonly MustSet<string> ResourceCreateRouteName = new MustSet<string>(nameof(ResourceCreateRouteName));
        public readonly MustSet<string> ResourceUpsertRouteName = new MustSet<string>(nameof(ResourceUpsertRouteName));
        public readonly MustSet<string> ResourcePatchRouteName = new MustSet<string>(nameof(ResourcePatchRouteName));
        public readonly MustSet<string> ResourceDeleteRouteName = new MustSet<string>(nameof(ResourceDeleteRouteName));

        public HateoasConfig(string entityName)
        {
            ResourcesGetRouteName.Set($"Get{entityName}sRouteName");
            ResourceGetRouteName.Set($"Get{entityName}RouteName");
            ResourceCreateRouteName.Set($"Create{entityName}RouteName");
            ResourceUpsertRouteName.Set($"Upsert{entityName}RouteName");
            ResourcePatchRouteName.Set($"Patch{entityName}RouteName");
            ResourceDeleteRouteName.Set($"Delete{entityName}RouteName");
        }
    }
}