using System;
using AspNetCore.MVC.RESTful.Configuration;
using AspNetCore.MVC.RESTful.Controllers;
using AspNetCore.MVC.RESTful.Helpers;
using AspNetCore.MVC.RESTful.Repositories;
using AspNetCore.MVC.RESTful.Services;
using AspNetCore.MVC.Restful.Tests.Builders;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.JsonPatch;

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


        [HttpPost(Name = CreateRouteName)]
        public IActionResult Create()
        {
            return ResourceCreate((object)null);
        }

        internal new IActionResult ResourceGet(Guid anyGuid)
        {
            return base.ResourceGet(anyGuid);
        }

        internal IActionResult ResourceCreate(TestCreationDto testCreationDto)
        {
            return base.ResourceCreate(testCreationDto);
        }

        internal object ResourcesGet(object parameters, IEntityFilter<TestEntity> p1, IEntitySearch<TestEntity> p2)
        {
            return base.ResourcesGet(parameters, p1, p2);
        }

        internal ActionResult ResourceUpsert(Guid anyGuid, TestCreationDto testCreationDto)
        {
            return base.ResourceUpsert(anyGuid, testCreationDto);
        }

        internal new ActionResult ResourcePatch(Guid anyGuid, JsonPatchDocument<TestDto> jsonPatchDocument)
        {
            return base.ResourcePatch(anyGuid, jsonPatchDocument);
        }

        internal new ActionResult ResourceDelete(Guid anyGuid)
        {
            return base.ResourceDelete(anyGuid);
        }

        internal IActionResult ResourceOptions()
        {
            return base.ResourceOptions();
        }
    }
}