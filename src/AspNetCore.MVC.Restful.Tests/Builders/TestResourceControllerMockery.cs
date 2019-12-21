using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Dynamic;
using System.Linq;
using AspNetCore.MVC.RESTful.Configuration;
using AspNetCore.MVC.RESTful.Helpers;
using AspNetCore.MVC.RESTful.Repositories;
using AspNetCore.MVC.RESTful.Services;
using AspNetCore.MVC.Restful.Tests.Controllers;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Moq;
using ValidationContext = System.ComponentModel.DataAnnotations.ValidationContext;

namespace AspNetCore.MVC.Restful.Tests.Builders
{
    /// <summary>
    /// A "Mockery" is a test pattern designed for objects with multiple or complex dependencies.
    ///
    /// Similar to the "Builder" pattern conceptually, the Mockery builds and manages all the Mocks
    /// and interactions with them, this allows the tests themselves to be more concise and easy to read
    /// and understand. 
    /// </summary>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
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
            WithExistingResource();
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

            SetupModelValidation();

            return _testResourceController;
        }

        delegate void ObjectValidatorDelegate(ActionContext actionContext, ValidationStateDictionary validationState, string prefix, object model); 

        private ObjectValidatorDelegate _objectValidatorDelegate;
        private void ObjectValidatorExecutor(ActionContext actionContext, ValidationStateDictionary validationState, string prefix, object model)
        {
            var validationResults = new List<ValidationResult>();
            Validator.TryValidateObject(model, new ValidationContext(model), validationResults); 

            foreach (var result in validationResults)
            {
                _testResourceController.ModelState.AddModelError(result.MemberNames.FirstOrDefault(), result.ErrorMessage);
            }
        }
        private void SetupModelValidation()
        {
            _objectValidatorDelegate += ObjectValidatorExecutor; 

            var objectValidator = new Mock<IObjectModelValidator>();
            objectValidator.Setup(x => x.Validate(It.IsAny<ActionContext>(),
                    It.IsAny<ValidationStateDictionary>(),
                    It.IsAny<string>(),
                    It.IsAny<object>()))
                .Callback(_objectValidatorDelegate);

            _testResourceController.ObjectValidator = objectValidator.Object;
        }

        public TestResourceControllerMockery WithEmptyResourceList()
        {
            SetResourcesGetEntities(new List<TestEntity>().AsQueryable());

            Mapper.Setup(m => m.Map<TestDto>(It.IsAny<TestEntity>()))
                .Returns(new TestDto {Id = Guid.NewGuid()});

            return this;
        }
        /// <summary>
        /// This uses other values set via "With" methods so we terminate
        /// the chain by returning void
        /// </summary>
        public void WithResourceList()
        {

            var testEntities = Enumerable.Range(1, 10).Select(i =>
                {
                    var e = new TestEntity() { Id = Guid.NewGuid() };

                    return e;
                })
                ;
            SetResourcesGetEntities(testEntities.AsQueryable());
        }
        private void SetResourcesGetEntities(IQueryable<TestEntity> entities)
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
                    entities,
                    _pageSize, _currentPage));

            // NOTE: Fake the mapping, we only use Id for tests
            Mapper.Setup(m => m.Map<IEnumerable<TestDto>>(It.IsAny<IEnumerable<TestEntity>>()))
                .Returns((IEnumerable<TestEntity> e) 
                    => e.Select(ent => new TestDto() { Id = ent.Id }));

            Mapper.Setup(m => m.Map<IEnumerable<TestDto>>(It.IsAny<IEnumerable<TestEntity>>()))
                .Returns((IEnumerable<TestEntity> e) => e.Select(ent => new TestDto() { Id = ent.Id }));


            Mapper.Setup(m => m.Map<TestEntity>(It.IsAny<TestDto>()))
                .Returns( (TestDto d) => new TestEntity {Id = d.Id});

            Mapper.Setup(m => m.Map<TestDto>(It.IsAny<TestEntity>()))
                .Returns( (TestEntity e)  => new TestDto() { Id = e.Id});

        }
        
        public TestResourceControllerMockery WithExistingResource(TestEntity entity = null)
        {
            var testEntity = entity ?? new TestEntity();



            ResourceRepository.Setup(r => r.Load(
                    It.IsAny<Guid>()
                ))
                .Returns(testEntity);


            return this;
        }
        public TestResourceControllerMockery WithNoExistingResource()
        {
            ResourceRepository.Setup(r => r.Load(
                    It.IsAny<Guid>()
                ))
                .Returns((TestEntity) null);

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

        public TestResourceControllerMockery WithInvalidModelState()
        {
            return WithInvalidField("Value");
        }
        public TestResourceControllerMockery WithValidModelState()
        {
            _testResourceController.ModelState.Clear();
            return this;
        }
        public TestResourceControllerMockery WithInvalidField(string fieldName, string errorMessage="invalid-field")
        {
            _testResourceController
                .ModelState
                .AddModelError(fieldName, errorMessage);

            return this;
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

        public TestResourceControllerMockery AndResourceWasDeleted(Guid guid)
        {
            ResourceRepository
                .Verify(r => 
                    r.Delete(It.IsAny<TestEntity>()), 
                    Times.Once);
            return this;
        }
        public TestResourceControllerMockery AndChangesWhereSaved()
        {
            ResourceRepository.Verify(r => r.Save(), Times.Once);
            return this;
        }

        public TestResourceControllerMockery AndResourceWasUpdated(Guid guid)
        {
            ResourceRepository
                .Verify(r =>
                        r.Update(It.Is<TestEntity>(e => e.Id.Equals(guid))),
                    Times.Once);
            return this;
        }

        public TestResourceControllerMockery AndResourceWasCreated()
        {
            ResourceRepository
                .Verify(r =>
                        r.Add(It.IsAny<TestEntity>()),
                    Times.Once);
            return this;
        }

        public T ExtractProperty<T>(ExpandoObject obj, string propertyName) => obj.ExtractProperty<T>(propertyName);

        private TestResourceControllerMockery WithUrlHelperLinkReturning(string link)
        {
            UrlHelper = new Mock<IUrlHelper>();
            UrlHelper
                .Setup(h => h.Link(It.IsAny<string>(), It.IsAny<object>()))
                .Returns(link);

            return this;
        }

        public TestResourceControllerMockery WithNoLinks()
        {
            _disableLinks = true;

            if (_testResourceController != null)
            {
                _testResourceController.HateoasConfig.AddLinks = false;
            }

            return this;
        }

    }
}