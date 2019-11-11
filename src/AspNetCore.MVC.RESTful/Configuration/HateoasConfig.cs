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
        public RelationshipNames Relationships { get; } = new RelationshipNames();
        public string LinksPropertyName { get; set; } = "_links";

        public HateoasConfig(string entityName)
        {
            ResourcesGetRouteName = $"Get{entityName}s";
            ResourceGetRouteName = $"Get{entityName}";
            ResourceCreateRouteName = $"Create{entityName}";
            ResourceUpsertRouteName = $"Upsert{entityName}";
            ResourcePatchRouteName = $"Patch{entityName}";
            ResourceDeleteRouteName = $"Delete{entityName}";
        }

        public class RelationshipNames
        {
            public string Self { get; set; } = "self";
            public string Create { get; set; } = "create";
            public string Update { get; set; } = "update";
            public string Patch { get; set; } = "patch";
            public string Delete { get; set; } = "delete";
            public string CurrentPage { get; set; } = "current-page";
            public string NextPage { get; set; } =  "next-page";
            public string PreviousPage { get; set; } =  "prev-page";
        }
    }
}