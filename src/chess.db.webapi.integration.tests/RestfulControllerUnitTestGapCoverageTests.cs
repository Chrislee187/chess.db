using System.Threading.Tasks;
using NUnit.Framework;
using Shouldly;

namespace chess.db.webapi.integration.tests
{
    public class RestfulControllerUnitTestGapCoverageTests
    {
        [Test]
        public async Task Collections_contains_pagination_header()
        {
            // Arrange
            var client = IntegrationFixture.Factory.CreateClient();

            // Act
            var response = await client.GetAsync("/api/events");

            response.EnsureSuccessStatusCode();

            var pagination = response.PaginationValues();

            pagination.CurrentPage.ShouldBe(1);
            pagination.PageSize.ShouldBe(20);
            pagination.PreviousPage.ShouldBeEmpty();
            pagination.NextPage.ShouldNotBeEmpty();
            pagination.TotalCount.ShouldBeGreaterThan(0);
            pagination.TotalPages.ShouldBeGreaterThan(0);
        }

        [Test]
        public void TODO_Add_to_collection_returns_location_header()
        {
            // TODO: DO NOT ADD ANYTHING TO REAL DB
            Assert.Inconclusive("TODO");
        }

        [Test]
        public void TODO_options_returns_allow_header()
        {
            // TODO: DO NOT ADD ANYTHING TO REAL DB
            Assert.Inconclusive("TODO");
        }
    }
}