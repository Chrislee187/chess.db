using System;

namespace AspNetCore.MVC.RESTful.Controllers
{
    public class ThrowIfNotSetString
    {
        private readonly string _name;
        public ThrowIfNotSetString(string name)
        {
            _name = name;
        }

        private string _string;

        public string Get()
        {
            if (string.IsNullOrEmpty(_string))
            {
                throw new NullReferenceException($"HateoasConfig.{_name} is not Set");
            }
            return _string;
        }
        public void Set(string routeName)
        {
            _string = routeName;
        }

    }
    public class HateoasConfig<TEntity>
    {
        public bool AddDefaultLinksToCollectionResources { get; set; } = true;
        public bool AddDefaultLinksToIndividualResources { get; set; } = true;


        public readonly ThrowIfNotSetString ResourcesGetRouteName = new ThrowIfNotSetString(nameof(ResourcesGetRouteName));
        public readonly ThrowIfNotSetString ResourceGetRouteName = new ThrowIfNotSetString(nameof(ResourceGetRouteName));
        public readonly ThrowIfNotSetString ResourceCreateRouteName = new ThrowIfNotSetString(nameof(ResourceCreateRouteName));
        public readonly ThrowIfNotSetString ResourceUpsertRouteName = new ThrowIfNotSetString(nameof(ResourceUpsertRouteName));
        public readonly ThrowIfNotSetString ResourcePatchRouteName = new ThrowIfNotSetString(nameof(ResourcePatchRouteName));
        public readonly ThrowIfNotSetString ResourceDeleteRouteName = new ThrowIfNotSetString(nameof(ResourceDeleteRouteName));

        public HateoasConfig()
        {
            var entity = nameof(TEntity);
            ResourcesGetRouteName.Set($"Get{entity}sRouteName");
            ResourceGetRouteName.Set($"Get{entity}RouteName");
            ResourceCreateRouteName.Set($"Create{entity}RouteName");
            ResourceUpsertRouteName.Set($"Upsert{entity}RouteName");
            ResourcePatchRouteName.Set($"Patch{entity}RouteName");
            ResourceDeleteRouteName.Set($"Delete{entity}RouteName");

        }
    }
}