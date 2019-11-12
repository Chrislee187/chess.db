using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using AspNetCore.MVC.RESTful.Configuration;
using AspNetCore.MVC.RESTful.Helpers;
using AspNetCore.MVC.RESTful.Repositories;
using AspNetCore.MVC.RESTful.Services;
using AspNetCore.MVC.Restful.Tests.Builders;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace AspNetCore.MVC.Restful.Tests.Controllers
{
    public class TestResourceControllerMockery
    {
        private string _shape;
        private TestResourceController _testResourceController;
        private bool _disableLinks;
        private int _currentPage = 1;
        private int _pageSize = 20;
        public Mock<IMapper> Mapper { get; } = new Mock<IMapper>();
        public Mock<IResourceRepository<TestEntity, Guid>> ResourceRepository { get; } = new Mock<IResourceRepository<TestEntity, Guid>>();
        public Mock<IOrderByPropertyMappingService<TestDto, TestEntity>> OrderByPropertyMappingService { get; } = new Mock<IOrderByPropertyMappingService<TestDto, TestEntity>>();
        public Mock<IEntityUpdater<TestEntity, Guid>> EntityUpdater { get; } = new Mock<IEntityUpdater<TestEntity, Guid>>();
        public Mock<IUrlHelper> UrlHelper { get; private set; } = new Mock<IUrlHelper>();

        public TestResourceControllerMockery()
        {
            WithValidOrderByClause().
            WithValidShape().
            WithUrlHelperLinkReturning("https://example.com/testresources").
            WithEmptyResourceList();
        }

        public TestResourceController BuildController(Action<HateoasConfig> config = null)
        {

            var testResourceController = new TestResourceController(
                Mapper.Object,
                ResourceRepository.Object,
                OrderByPropertyMappingService.Object,
                EntityUpdater.Object,
                config
            ) {Url = UrlHelper.Object};

            _testResourceController = testResourceController;
            _testResourceController.ControllerContext = new ControllerContext();
            _testResourceController.ControllerContext.HttpContext = new DefaultHttpContext();

            _testResourceController.CollectionConfig.Shape = _shape;
            _testResourceController.CollectionConfig.PageSize = _pageSize;
            _testResourceController.CollectionConfig.Page = _currentPage;
            _testResourceController.HateoasConfig.AddLinks = !_disableLinks;

            return _testResourceController;
        }

        public TestResourceControllerMockery WithEmptyResourceList()
        {
            ResourceRepository.Setup(r => r.Load(
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<IEntityFilter<TestEntity>>(),
                    It.IsAny<IEntitySearch<TestEntity>>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<IDictionary<string, OrderByPropertyMappingValue>>()
                ))
                .Returns(new PagedList<TestEntity>(
                    new List<TestEntity>().AsQueryable(),
                    20, 1));

            Mapper.Setup(m => m.Map<TestDto>(It.IsAny<TestEntity>()))
                .Returns(new TestDto {Id = Guid.NewGuid()});

            return this;
        }
        public TestResourceControllerMockery WithResourceList()
        {

            var testEntities = Enumerable.Range(1, 10).Select(i =>
            {
                var e = new TestEntity() { Id= Guid.NewGuid()};

                return e;
            }).ToList();

            ResourceRepository.Setup(r => r.Load(
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<IEntityFilter<TestEntity>>(),
                    It.IsAny<IEntitySearch<TestEntity>>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<IDictionary<string, OrderByPropertyMappingValue>>()
                ))
                .Returns((
                        int pageSize, 
                        int page, 
                        IEntityFilter<TestEntity> ef, 
                        IEntitySearch<TestEntity> es, string ss, string ob, IDictionary<string, OrderByPropertyMappingValue> om) =>
                    new PagedList<TestEntity>(
                        testEntities.AsQueryable(),
                        pageSize,
                        page
                        ));

            Mapper.Setup(m => m.Map<IEnumerable<TestDto>>(It.IsAny<IEnumerable<TestEntity>>()))
                .Returns((IEnumerable<TestEntity> e) => e.Select(ent => new TestDto() {Id = ent.Id}));
            return this;
        }
        private TestResourceControllerMockery WithUrlHelperLinkReturning(string link)
        {
            UrlHelper = new Mock<IUrlHelper>();
            UrlHelper
                .Setup(h => h.Link(It.IsAny<string>(), It.IsAny<object>()))
                .Returns(link);

            return this;
        }

        public TestResourceControllerMockery WithValidOrderByClause()
        {
            OrderByPropertyMappingService
                .Setup(s => s.ClauseIsValid(It.IsAny<string>()))
                .Returns((true, null));

            return this;
        }
        public TestResourceControllerMockery WithInvalidOrderByClause()
        {
            OrderByPropertyMappingService
                .Setup(s => s.ClauseIsValid(It.IsAny<string>()))
                .Returns((false, new ProblemDetails()));

            return this;
        }

        public TestResourceControllerMockery WithValidShape(string shape = "")
        {
            _shape = shape;

            if (_testResourceController != null)
            {
                _testResourceController.CollectionConfig.Shape = shape;
            }

            return this;
        }
        public TestResourceControllerMockery WithInvalidShape()
        {
            var invalidShape = Guid.NewGuid().ToString();
            _shape = invalidShape;

            if (_testResourceController != null)
            {
                _testResourceController.CollectionConfig.Shape = invalidShape;
            }

            return this;
        }

        public TestResourceControllerMockery NoLinks()
        {
            _disableLinks = true;

            if (_testResourceController != null)
            {
                _testResourceController.HateoasConfig.AddLinks = false;
            }

            return this;
        }

        public T ExtractProperty<T>(ExpandoObject obj, string propertyName)
        {
            var resourcesDict = (IDictionary<string, object>)obj;

            if (resourcesDict.TryGetValue(propertyName, out var linksObject))
            {
                return (T)linksObject;
            }

            return default;
        }

        public TestResourceControllerMockery WithCurrentPage(int page)
        {
            _currentPage = page;
            if (_testResourceController != null)
            {
                _testResourceController.CollectionConfig.Page = page;
            }

            return this;
        }
        public TestResourceControllerMockery WithPageSize(int pageSize)
        {
            _pageSize = pageSize;
            if (_testResourceController != null)
            {
                _testResourceController.CollectionConfig.PageSize = pageSize;
            }

            return this;
        }
    }
}