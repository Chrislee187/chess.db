using System;
using System.Collections.Generic;
using AspNetCore.MVC.RESTful.Controllers;
using AspNetCore.MVC.RESTful.Helpers;
using AspNetCore.MVC.RESTful.Models;

namespace AspNetCore.MVC.Restful.Tests.Builders
{
    public class TestHateoasController : HateoasController<TestEntity, Guid>
    {
        public TestHateoasController()
        {
            Url = new TestUrlHelper();
        }

        internal new List<HateoasLink> ResourcesGetLinks<T>(T p, IPaginationMetadata pagination)
        {
            return base.ResourcesGetLinks(p, pagination);
        }

        internal new List<HateoasLink> ResourceCreateLinks(Guid guid)
        {
            return base.ResourceCreateLinks(guid);
        }

        internal new List<HateoasLink> ResourceUpsertLinks(Guid guid)
        {
            return base.ResourceUpsertLinks(guid);
        }

        internal new List<HateoasLink> ResourceGetLinks(Guid guid, string v)
        {
            return base.ResourceGetLinks(guid, v);
        }

        internal new List<HateoasLink> ResourcePatchLinks(Guid guid)
        {
            return base.ResourcePatchLinks(guid);
        }

        internal  new List<HateoasLink>  ResourceDeleteLinks()
        {
            return base.ResourceDeleteLinks();
        }
    }
}