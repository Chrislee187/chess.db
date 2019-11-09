using System;

namespace AspNetCore.MVC.RESTful.Controllers
{
    public class ResourceControllerConfig
    {
        public HateoasConfig Hateoas { get; } = new HateoasConfig();

        public class HateoasConfig
        {
            public bool AddDefaultLinksToCollectionResources { get; set; } = true;
            public bool AddDefaultLinksToIndividualResources { get; set; } = true;

        }


        private string _resourcesGetRouteName;
        public string ResourcesGetRouteName()
        {
            if (string.IsNullOrEmpty(_resourcesGetRouteName))
            {
                throw new NullReferenceException("ControllerConfig.ResourcesGetRouteName is not registered");
            }
            return _resourcesGetRouteName;
        }
        public ResourceControllerConfig RegisterResourcesGetRouteName(string resourcesGetRouteName)
        {
            _resourcesGetRouteName = resourcesGetRouteName;
            return this;
        }

        private string _resourceGetRouteName;
        public string ResourceGetRouteName()
        {
            if (string.IsNullOrEmpty(_resourceGetRouteName))
            {
                throw new NullReferenceException("ControllerConfig.ResourceGetRouteName is not registered");
            }
            return _resourceGetRouteName;
        }
        public ResourceControllerConfig RegisterResourceGetRouteName(string resourceGetRouteName)
        {
            _resourceGetRouteName = resourceGetRouteName;
            return this;
        }
    }
}