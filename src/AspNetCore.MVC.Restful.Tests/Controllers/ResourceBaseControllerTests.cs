using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using AspNetCore.MVC.RESTful.Controllers;
using AspNetCore.MVC.RESTful.Helpers;
using AspNetCore.MVC.RESTful.Models;
using AspNetCore.MVC.Restful.Tests.Builders;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using NUnit.Framework;
using Shouldly;

namespace AspNetCore.MVC.Restful.Tests.Controllers
{
    // An experiment on how much of a controller you can truly unit-test.
    // Most of it is the answer, anything relating to pre/post action execution
    // so automatic model validation (action filters),
    // anything handled by result formatting (post execution action filter code I believe)
    public class ResourceBaseControllerTests 
    {
        private ResourceControllerBase<TestDto, TestEntity, Guid> _controller;
        private TestResourceControllerMockery _mockery;
        private static readonly Guid _anyGuid = Guid.NewGuid();
        private readonly TestEntity AnyEntity = new TestEntity() { Id= _anyGuid};

        [SetUp]
        public void Setup()
        {
            _mockery = new TestResourceControllerMockery();

            _controller = _mockery.BuildController();
        }

        [Test]
        public void ResourcesGet_BadRequest_should_be_returned_for_invalid_shape_property_names()
        {
            _mockery.WithInvalidShape();

            _controller
                .ResourcesGet((object) null, null, null)
                .ShouldBeOfType<BadRequestObjectResult>();
        }

        [Test]
        public void ResourcesGet_BadRequest_should_be_returned_for_invalid_order_by_clauses()
        {
            _mockery.WithInvalidOrderByClause();

            _controller
                .ResourcesGet((object)null, null, null)
                .ShouldBeOfType<BadRequestObjectResult>();
        }

        [Test]
        public void ResourcesGet_Ok_should_be_returned_for_valid_request()
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
            _mockery.WithNoLinks();

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
            _mockery
                .WithCurrentPage(2)
                .WithPageSize(2)
                .WithResourceList();

            _controller
                .ResourcesGet((object)null, null, null);

            _controller.Response.Headers
                .ContainsKey("X-Pagination")
                .ShouldBeTrue();

            PaginationHeaderShouldContain(
                _controller.Response.Headers["X-Pagination"], 
                2, 2, 5, 10);
        }

        [Test]
        public void ResourceGet_BadRequest_should_be_thrown_for_invalid_shape_property_names()
        {
            _mockery.WithInvalidShape();

            _controller
                .ResourceGet(_anyGuid)
                .ShouldBeOfType<BadRequestObjectResult>();
        }

        [Test]
        public void ResourceGet_Ok_should_be_returned_for_valid_request()
        {

            var resourcesResult = _controller
                .ResourceGet(_anyGuid);

            resourcesResult.ShouldBeOfType<OkObjectResult>();
        }

        [Test]
        public void ResourceGet_valid_request_contains_links()
        {
            var resourcesResult = (OkObjectResult)_controller
                .ResourceGet(_anyGuid);

            ((IDictionary<string, object>) resourcesResult.Value)
                .ContainsKey(_controller.HateoasConfig.LinksPropertyName)
                .ShouldBe(true);
        }

        [Test]
        public void ResourceGet_nolinks_param_disables_links()
        {
            _mockery.WithNoLinks();

            var resourcesResult = (OkObjectResult)_controller
                .ResourceGet(_anyGuid);

            ((IDictionary<string, object>)resourcesResult.Value)
                .ContainsKey(_controller.HateoasConfig.LinksPropertyName)
                .ShouldBe(false);

        }


        [Test]
        public void ResourceCreate_CreateAtRoute_is_returned_for_successful_resource_creeation()
        {
            _mockery.WithValidModelState();

            var resourcesResult = _controller.ResourceCreate(new TestCreationDto());

            resourcesResult
                .ShouldBeOfType<CreatedAtRouteResult>();


            _mockery
                .AndResourceWasCreated()
                .AndChangesWhereSaved();
        }

        [Test]
        public void ResourceUpsert_CreatedAtRoute_is_returned_for_new_resource_creation()
        {
            _mockery.WithNoExistingResource();
            var resourcesResult = _controller.ResourceUpsert(_anyGuid, new TestCreationDto());

            resourcesResult
                .ShouldBeOfType<CreatedAtRouteResult>();

            _mockery
                .AndResourceWasCreated()
                .AndChangesWhereSaved();

        }

        [Test]
        public void ResourceUpsert_NoContent_is_returned_for_successful_existing_resource_update()
        {
            _mockery.WithExistingResource(AnyEntity);

            var resourcesResult = _controller.ResourceUpsert(AnyEntity.Id, new TestCreationDto());

            resourcesResult
                .ShouldBeOfType<NoContentResult>();

            _mockery
                .AndResourceWasUpdated(AnyEntity.Id)
                .AndChangesWhereSaved();
        }

        [Test]
        public void ResourcePatch_NotFound_is_returned_for_non_existing_resources()
        {
            _mockery.WithNoExistingResource();

            var resourcesResult = _controller.ResourcePatch(_anyGuid, new JsonPatchDocument<TestDto>());

            resourcesResult
                .ShouldBeOfType<NotFoundResult>();
        }

        [Test]
        public void ResourcePatch_UnprocessableEntityObjectResult_is_returned_for_validation_errors()
        {

            _mockery.WithInvalidModelState();

            var resourcesResult = _controller.ResourcePatch(_anyGuid, new JsonPatchDocument<TestDto>());

            resourcesResult
                .ShouldBeOfType<UnprocessableEntityObjectResult>();
        }

        [Test]
        public void ResourcePatch_Ok_is_returned_for_successful_update()
        {
            _mockery
                .WithExistingResource(AnyEntity);
            var resourcesResult = _controller.ResourcePatch(AnyEntity.Id, new JsonPatchDocument<TestDto>());

            resourcesResult
                .ShouldBeOfType<OkObjectResult>();

            _mockery
                .AndResourceWasUpdated(AnyEntity.Id)
                .AndChangesWhereSaved();
        }
        [Test]
        public void ResourcePatch_NoContent_is_returned_for_successful_update_when_links_disabled()
        {
            _mockery
                .WithExistingResource(AnyEntity).WithNoLinks();
            var resourcesResult = _controller.ResourcePatch(AnyEntity.Id, new JsonPatchDocument<TestDto>());

            resourcesResult
                .ShouldBeOfType<NoContentResult>();
            
            _mockery
                .AndResourceWasUpdated(AnyEntity.Id)
                .AndChangesWhereSaved();
        }

        [Test]
        public void ResourceDelete_NotFound_is_returned_for_non_existing_resources()
        {
            _mockery.WithNoExistingResource();

            var resourcesResult = _controller.ResourceDelete(_anyGuid);
            resourcesResult
                .ShouldBeOfType<NotFoundResult>();

        }

        [Test]
        public void ResourceDelete_NoContent_is_returned_for_successful_delete()
        {
            _mockery.WithExistingResource();

            var resourcesResult = _controller.ResourceDelete(_anyGuid);
            resourcesResult
                .ShouldBeOfType<NoContentResult>();

            _mockery
                .AndResourceWasDeleted(_anyGuid)
                .AndChangesWhereSaved();
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
                    nullUpdater ? null : _mockery.EntityUpdater.Object,
                    (cfg) =>
                    {
                        cfg.ResourceCreateRouteName = TestResourceController.CreateRouteName;
                        cfg.ResourcesGetRouteName = TestResourceController.GetsRouteName;
                    })
                );
        }

        [Test]
        public void ResourceCreate_Location_Header_is_added_to_Response_on_successful_resource_creation()
        {
            Assert.Inconclusive("Header is only added to when the action result is formatted, " +
                        "so can't test here as formatting is handled elsewhere (ActionFilters?)");
            _mockery.WithValidModelState();

            var resourceCreate = _controller.ResourceCreate(new TestCreationDto());

            _controller.Response.Headers
                .ContainsKey("Location").ShouldBeTrue();
        }

        [Test]
        public void ResourceOptions_adds_to_header()
        {
            Assert.Inconclusive("Header is only added to when the action result is formatted, " +
                        "so can't test here as formatting is handled elsewhere (ActionFilters?)");
            var resourcesResult = _controller.ResourceOptions();

            resourcesResult
                .ShouldBeOfType<OkResult>();

            _controller.Response.Headers
                .ContainsKey("Allow")
                .ShouldBeFalse();
        }

        [Test]
        public void ResourceCreate_UnprocessableEntityObjectResult_should_be_returned_for_validation_errors()
        {
            Assert.Inconclusive("ActionFilters pre-execution triggers handle model validation, so cannot be directly unit-tested");

            _mockery.WithInvalidModelState();

            var resourcesResult = _controller.ResourceCreate(new TestCreationDto());

            resourcesResult
                .ShouldBeOfType<UnprocessableEntityObjectResult>();
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