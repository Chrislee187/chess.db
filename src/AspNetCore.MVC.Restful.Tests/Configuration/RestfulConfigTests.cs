using AspNetCore.MVC.RESTful.Configuration;
using NUnit.Framework;
using Shouldly;

namespace AspNetCore.MVC.Restful.Tests.Configuration
{
    public class RestfulConfigTests
    {
        private CollectionConfig _cfg;

        [SetUp]
        public void Setup()
        {
            _cfg = new CollectionConfig();
        }

        [Test]
        public void PageSize_is_defaulted()
        {
            _cfg.PageSize.ShouldBeGreaterThan(1);
            _cfg.PageSize.ShouldBeLessThanOrEqualTo(CollectionConfig.MaxPageSize);
        }

        [Test]
        public void Page_is_defaulted()
        {
            _cfg.Page.ShouldBe(1);
        }

        [Test]
        public void Other_params_are_empty()
        {
            _cfg.OrderBy.ShouldBeNullOrEmpty();
            _cfg.Shape.ShouldBeNullOrEmpty();
            _cfg.SearchText.ShouldBeNullOrEmpty();
        }

        [Test]
        public void AppendToUrl_pagination_parameters_are_append()
        {
            _cfg.AppendToUrl("")
                .ShouldBe($"?page=1&page-size={CollectionConfig.DefaultPageSize}");
        }

        [Test]
        public void AppendToUrl_ampersand_is_used_if_url_has_existing_parameters()
        {
            _cfg.AppendToUrl("?blah")
                .ShouldBe($"?blah&page=1&page-size={CollectionConfig.DefaultPageSize}");
        }

        [Test]
        public void AppendToUrl_order_by_params_are_appended()
        {
            var root = $"?page=1&page-size={CollectionConfig.DefaultPageSize}";

            _cfg.OrderBy = "firstname";
            _cfg.AppendToUrl("")
                .ShouldBe(root + $"&order-by=firstname");
        }

        [Test]
        public void AppendToUrl_shape_params_are_appended()
        {
            var root = $"?page=1&page-size={CollectionConfig.DefaultPageSize}";

            _cfg.Shape = "firstname";
            _cfg.AppendToUrl("")
                .ShouldBe(root + $"&shape=firstname");
        }

        [Test]
        public void AppendToUrl_search_params_are_appended()
        {
            var root = $"?page=1&page-size={CollectionConfig.DefaultPageSize}";

            _cfg.SearchText = "dave";
            _cfg.AppendToUrl("")
                .ShouldBe(root + $"&search-query=dave");
        }
    }
}