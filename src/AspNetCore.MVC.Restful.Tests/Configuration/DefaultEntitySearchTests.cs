using System.Linq;
using AspNetCore.MVC.RESTful.Configuration;
using AspNetCore.MVC.Restful.Tests.Builders;
using NUnit.Framework;
using Shouldly;

namespace AspNetCore.MVC.Restful.Tests.Configuration
{
    [TestFixture]
    public class DefaultEntitySearchTests
    {
        [Test]
        public void Search_returns_all_results()
        {
            var testEntities = Enumerable.Range(1,10).Select(r => new TestEntity()).ToList();
            
            var search = new DefaultEntitySearch<TestEntity>();

            var queryable = search.Search(testEntities.AsQueryable(), "something");

            queryable.Count().ShouldBe(testEntities.Count);
        }
    }
}