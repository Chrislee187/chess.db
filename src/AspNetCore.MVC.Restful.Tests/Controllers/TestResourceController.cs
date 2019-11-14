using System;
using AspNetCore.MVC.RESTful.Configuration;
using AspNetCore.MVC.RESTful.Controllers;
using AspNetCore.MVC.RESTful.Helpers;
using AspNetCore.MVC.RESTful.Repositories;
using AspNetCore.MVC.RESTful.Services;
using AspNetCore.MVC.Restful.Tests.Builders;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace AspNetCore.MVC.Restful.Tests.Controllers
{
    public class TestResourceController : ResourceControllerBase<TestDto, TestEntity, Guid>
    {
        public TestResourceController(
            IMapper mapper, 
            IResourceRepository<TestEntity, Guid> resourceRepository, 
            IOrderByPropertyMappingService<TestDto, TestEntity> orderByPropertyMappingService, 
            IEntityUpdater<TestEntity, Guid> entityUpdater, 
            Action<HateoasConfig> config = null) 
            : base(mapper, resourceRepository, entityUpdater, orderByPropertyMappingService, config)
        {
        }

        public const string GetsRouteName = "Gets";
        public const string CreateRouteName = "Create";

        [HttpGet(Name = GetsRouteName)]
        [HttpHead]
        public IActionResult Gets()
        {
            return ResourcesGet((object) null, null, null);
        }

        [HttpPost(Name = CreateRouteName)]
        public IActionResult Create()
        {
            return ResourceCreate((object)null);
        }
    }
}