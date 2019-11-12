using System;
using System.Linq;
using AspNetCore.MVC.RESTful.Configuration;
using AspNetCore.MVC.RESTful.Controllers;
using AspNetCore.MVC.Restful.Tests.Builders;
using NUnit.Framework;
using Shouldly;

namespace AspNetCore.MVC.Restful.Tests.Controllers
{
    public class HateoasControllerTests
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
                .Any(l => l.Rel.Equals(HateoasConfig.Relationships.CurrentPage))
                .ShouldBeTrue();

            resourcesGetLinks
                .Any(l => l.Rel.Equals(HateoasConfig.Relationships.NextPage))
                .ShouldBeFalse();

            resourcesGetLinks
                .Any(l => l.Rel.Equals(HateoasConfig.Relationships.PreviousPage))
                .ShouldBeFalse();
        }

        [Test]
        public void ResourcesGetLinks_returns_previous_and_next_links()
        {
            var pagination = new PaginationMetadataBuilder()
                .HasPrevious()
                .HasNext()
                .Build();

            var resourcesGetLinks = _controller.ResourcesGetLinks<object>(null, pagination);

            resourcesGetLinks
                .Any(l => l.Rel.Equals(HateoasConfig.Relationships.NextPage))
                .ShouldBeTrue();

            resourcesGetLinks
                .Any(l => l.Rel.Equals(HateoasConfig.Relationships.PreviousPage))
                .ShouldBeTrue();
        }

        [Test]
        public void ResourcesGetLinks_returns_current_for_empty_list()
        {
            var pagination = new PaginationMetadataBuilder()
                .WithPage(1)
                .WithPageSize(20)
                .WithTotalCount(0)
                .Build();

            var resourcesGetLinks = _controller.ResourcesGetLinks<object>(null, pagination);

            resourcesGetLinks
                .Any(l => l.Rel.Equals(HateoasConfig.Relationships.NextPage))
                .ShouldBeFalse();

            resourcesGetLinks
                .Any(l => l.Rel.Equals(HateoasConfig.Relationships.PreviousPage))
                .ShouldBeFalse();
        }


        [Test]
        public void ResourceGetLinks_returns_current_standard_four_relationships()
        {
            var resourceGetLinks = _controller.ResourceGetLinks(Guid.NewGuid(), "");

            resourceGetLinks
                .ShouldContain(l 
                    => l.Rel.Equals(HateoasConfig.Relationships.Self))
                ;

            resourceGetLinks
                .ShouldContain(l 
                    => l.Rel.Equals(HateoasConfig.Relationships.Upsert))
                ;

            resourceGetLinks
                .ShouldContain(l 
                    => l.Rel.Equals(HateoasConfig.Relationships.Patch))
                ;

            resourceGetLinks
                .ShouldContain(l 
                    => l.Rel.Equals(HateoasConfig.Relationships.Delete))
                ;
        }

        [Test]
        public void ResourceCreateLinks_returns_get_and_delete_relationships()
        {
            var resourceGetLinks = _controller.ResourceCreateLinks(Guid.NewGuid());

            resourceGetLinks
                .ShouldContain(l 
                    => l.Rel.Equals(HateoasConfig.Relationships.Self))
                ;

            resourceGetLinks
                .ShouldContain(l =>
                    l.Rel.Equals(HateoasConfig.Relationships.Delete))
                ;
        }

        [Test]
        public void ResourceUpsertLinks_returns_get_and_delete_relationships()
        {
            var resourceGetLinks = _controller.ResourceUpsertLinks(Guid.NewGuid());

            resourceGetLinks
                .ShouldContain(l 
                    => l.Rel.Equals(HateoasConfig.Relationships.Self))
                ;

            resourceGetLinks
                .ShouldContain(l 
                    => l.Rel.Equals(HateoasConfig.Relationships.Delete))
                ;
        }
        
        [Test]
        public void ResourcePatchLinks_returns_get_and_delete_relationships()
        {
            var resourceGetLinks = _controller.ResourcePatchLinks(Guid.NewGuid());

            resourceGetLinks
                .ShouldContain(l 
                    => l.Rel.Equals(HateoasConfig.Relationships.Self))
                ;

            resourceGetLinks
                .Any(l => l.Rel.Equals(HateoasConfig.Relationships.Delete))
                .ShouldBeTrue();
        }
        
        [Test]
        public void ResourceDeleteLinks_returns_get_and_delete_relationships()
        {
            var resourceGetLinks = _controller.ResourceDeleteLinks();

            resourceGetLinks.ShouldContain(l 
                => l.Rel.Equals(HateoasConfig.Relationships.CurrentPage));

            resourceGetLinks.ShouldContain(l
                => l.Rel.Equals(HateoasConfig.Relationships.Create));
        }
    }
}