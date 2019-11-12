using System;
using System.Linq;
using AspNetCore.MVC.RESTful.Controllers;
using AspNetCore.MVC.Restful.Tests.Builders;
using NUnit.Framework;
using Shouldly;

namespace AspNetCore.MVC.Restful.Tests
{
    public class Spikes
    {
        private HateoasController<TestEntity, Guid> _controller;

        [SetUp]
        public void Setup()
        {
            _controller = new TestHateoasController();
        }
        [Test]
        public void ResourcesGetLinks_returns_current_page_only_for_single_page_lists()
        {
            var pagination = new PaginationMetadataBuilder()
                .Build();

            var resourcesGetLinks = _controller.ResourcesGetLinks<object>(null, pagination);

            resourcesGetLinks
                .Any(l => l.Rel.Equals(_controller.HateoasConfig.Relationships.CurrentPage))
                .ShouldBeTrue();

            resourcesGetLinks
                .Any(l => l.Rel.Equals(_controller.HateoasConfig.Relationships.NextPage))
                .ShouldBeFalse();

            resourcesGetLinks
                .Any(l => l.Rel.Equals(_controller.HateoasConfig.Relationships.PreviousPage))
                .ShouldBeFalse();
        }
   }
}