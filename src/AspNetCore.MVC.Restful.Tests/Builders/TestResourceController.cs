using System;
using AspNetCore.MVC.RESTful.Configuration;
using AspNetCore.MVC.RESTful.Controllers;
using AspNetCore.MVC.RESTful.Helpers;
using AspNetCore.MVC.RESTful.Repositories;
using AspNetCore.MVC.RESTful.Services;
using AutoMapper;

namespace AspNetCore.MVC.Restful.Tests.Builders
{
    public class TestResourceController : ResourceControllerBase<TestDto, TestEntity, Guid>
    {
        public TestResourceController(
            IMapper mapper, 
            IResourceRepository<TestEntity, Guid> resourceRepository, 
            IOrderByPropertyMappingService<TestDto, TestEntity> orderByPropertyMappingService, 
            IEntityUpdater<TestEntity, Guid> entityUpdater, 
            Action<HateoasConfig> config = null) 
            : base(mapper, resourceRepository, orderByPropertyMappingService, entityUpdater, config)
        {
        }
    }
}