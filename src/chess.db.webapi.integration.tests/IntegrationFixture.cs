using Microsoft.AspNetCore.Mvc.Testing;
using NUnit.Framework;

namespace chess.db.webapi.integration.tests
{
    [SetUpFixture]
    public class IntegrationFixture
    {
        public static WebApplicationFactory<Startup> Factory;
        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            Factory = new WebApplicationFactory<Startup>();
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
        }
    }
}