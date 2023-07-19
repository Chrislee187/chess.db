using chess.games.db.api.Services;
using NUnit.Framework;
using Shouldly;

namespace chess.game.db.unit.tests
{
    public class PersonNameTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [TestCase("Doe", null, null, "Doe")]
        [TestCase("John Doe", "John", null, "Doe")]
        [TestCase("John Peter Doe", "John", "Peter", "Doe")]
        [TestCase("John P Doe", "John", "P", "Doe")]
        [TestCase("Doe, John", "John", null, "Doe")]
        [TestCase("Doe,John", "John", null, "Doe")]
        [TestCase("Doe, John Peter", "John", "Peter", "Doe")]
        [TestCase("Peter Doe, John", "John", "Peter", "Doe")]
        [TestCase("Doe, J", "J", null, "Doe")]
        [TestCase("Doe,J.", "J", null, "Doe")]
        [TestCase("John Peter-Doe", "John", null, "Peter-Doe")]
        [TestCase("John-Luc Doe", "John-Luc", null, "Doe")]
        // NOTE: Does not detect reversed order without a comma
        [TestCase("Doe John", "Doe", null, "John")]
        [TestCase("Doe J", "Doe", null, "J")] 
        [TestCase("Doe J.", "Doe", null, "J")]
        public void Test1(string text, string firstName, string middleName, string lastName)
        {
            if (PersonName.TryParse(text, out var personName))
            {
                personName.Firstname.ShouldBe(firstName);
                personName.Middlename.ShouldBe(middleName);
                personName.Lastname.ShouldBe(lastName);
            }
            else
            {
                Assert.Fail($"Failed to parse '{text}'");
            }
        }
    }
}