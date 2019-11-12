using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using AspNetCore.MVC.RESTful.Controllers;
using AspNetCore.MVC.RESTful.Helpers;
using AspNetCore.MVC.RESTful.Models;
using AspNetCore.MVC.Restful.Tests.Builders;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using NUnit.Framework;
using Shouldly;

namespace AspNetCore.MVC.Restful.Tests.Controllers
{
    public class ResourceBaseControllerTests
    {
        private ResourceControllerBase<TestDto, TestEntity, Guid> _controller;
        private TestResourceControllerMockery _mockery;

        [SetUp]
        public void Setup()
        {
            _mockery = new TestResourceControllerMockery();

            _controller = _mockery.BuildController();
        }

        [Test]
        public void ResourcesGet_BadRequest_should_be_thrown_for_invalid_shape_property_names()
        {
            _mockery.WithInvalidShape();

            _controller
                .ResourcesGet((object) null, null, null)
                .ShouldBeOfType<BadRequestObjectResult>();
        }

        [Test]
        public void ResourcesGet_BadRequest_should_be_thrown_for_invalid_order_by_clauses()
        {
            _mockery.WithInvalidOrderByClause();

            _controller
                .ResourcesGet((object)null, null, null)
                .ShouldBeOfType<BadRequestObjectResult>();
        }

        [Test]
        public void ResourcesGet_valid_request_returns_Ok()
        {

            var resourcesResult = _controller
                .ResourcesGet((object)null, null, null);

            resourcesResult.ShouldBeOfType<OkObjectResult>();
        }

        [Test]
        public void ResourcesGet_valid_request_contains_links()
        {
            _mockery.WithResourceList();

            var resourcesResult = (OkObjectResult) _controller
                .ResourcesGet((object)null, null, null);

            var resources = resourcesResult.Value as ExpandoObject;
            var dict = (IDictionary<string, object>) resources ?? new ConcurrentDictionary<string, object>();
            var collectionLinks =
                _mockery.ExtractProperty<IEnumerable<HateoasLink>>(resources,
                    _controller.HateoasConfig.LinksPropertyName);

            collectionLinks.Count().ShouldBeGreaterThan(0, "Collection links missing!");
            
            if (dict.TryGetValue("value", out var valuesObject))
            {
                var childResources = (IEnumerable<ExpandoObject>) valuesObject;

                childResources.ToList().ForEach(child =>
                {
                    child.ToExpandoDict()
                        .ContainsKey(_controller.HateoasConfig.LinksPropertyName)
                        .ShouldBe(true);

                    var links = _mockery
                        .ExtractProperty<IEnumerable<HateoasLink>>(child, _controller.HateoasConfig.LinksPropertyName);

                    links.Count().ShouldBeGreaterThan(0, $"Child resource links missing");
                });
            }

        }

        [Test]
        public void ResourcesGet_nolinks_param_disables_links()
        {
            _mockery.WithResourceList();
            _mockery.NoLinks();

            var resourcesResult = (OkObjectResult)_controller
                .ResourcesGet((object)null, null, null);


            var dictCollection = ((IEnumerable<ExpandoObject>) resourcesResult.Value)
                .Select(e => e.ToExpandoDict())
                .ToList();

            dictCollection.ShouldNotBeNull();

            dictCollection.ToList().ForEach(d =>
            {
                d.TryGetValue(_controller.HateoasConfig.LinksPropertyName, out _).ShouldBeFalse();
            });
        }

        [Test]
        public void ResourcesGet_paginated_resource_list_sets_XPagination_Header()
        {
            _mockery.WithResourceList();
            _mockery.WithCurrentPage(2); 
            _mockery.WithPageSize(2);

            _controller
                .ResourcesGet((object)null, null, null);

            _controller.Response.Headers
                .ContainsKey("X-Pagination")
                .ShouldBeTrue();

            PaginationHeaderShouldContain(
                _controller.Response.Headers["X-Pagination"], 
                2, 2, 5, 10);
        }
        
        [TestCase(true, false, false, false)]
        [TestCase(false, true, false, false)]
        [TestCase(false, false, true, false)]
        [TestCase(false, false, false, true)]
        public void Construction_null_dependencies_should_throw_exceptions(bool nullMapper, bool nullRepo, bool nullOrderByMapper, bool nullUpdater)
        {
            Should.Throw<ArgumentNullException>(() 
                => new TestResourceController(
                    nullMapper ? null : _mockery.Mapper.Object,
                    nullRepo ? null : _mockery.ResourceRepository.Object,
                    nullOrderByMapper ? null : _mockery.OrderByPropertyMappingService.Object,
                    nullUpdater ? null : _mockery.EntityUpdater.Object));
        }

        private void PaginationHeaderShouldContain(StringValues headerValues, int currentPage, int pageSize, int totalPages, int pageCount)
        {
            var p = JsonConvert.DeserializeObject<ExpandoObject>(headerValues);
            var pagination = new Dictionary<string, object>(p);

            pagination.ContainsKey("CurrentPage").ShouldBeTrue();
            int.Parse(pagination["CurrentPage"].ToString()).ShouldBe(currentPage);

            pagination.ContainsKey("PageSize").ShouldBeTrue();
            int.Parse(pagination["PageSize"].ToString()).ShouldBe(pageSize);

            pagination.ContainsKey("TotalPages").ShouldBeTrue();
            int.Parse(pagination["TotalPages"].ToString()).ShouldBe(totalPages);

            pagination.ContainsKey("TotalCount").ShouldBeTrue();
            int.Parse(pagination["TotalCount"].ToString()).ShouldBe(pageCount);

            if (totalPages > currentPage)
            {
                pagination.ContainsKey("NextPage").ShouldBeTrue();
                pagination["NextPage"].ToString().ShouldNotBeNullOrEmpty();
            }

            if (currentPage > 1)
            {
                pagination.ContainsKey("PreviousPage").ShouldBeTrue();
                pagination["PreviousPage"].ToString().ShouldNotBeNullOrEmpty();
            }

        }
    }
}