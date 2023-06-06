
using Chess.Games.Data;
using Shouldly;

namespace Chess.Data.PGNImporter.Unit.Tests
{
    public class PersonNameTests
    {
        [Theory]
        [InlineData("Doe", null, null, "Doe")]
        [InlineData("John Doe", "John", null, "Doe")]
        [InlineData("John Peter Doe", "John", "Peter", "Doe")]
        [InlineData("John P Doe", "John", "P", "Doe")]
        [InlineData("Doe, John", "John", null, "Doe")]
        [InlineData("Doe,John", "John", null, "Doe")]
        [InlineData("Doe, John Peter", "John", "Peter", "Doe")]
        [InlineData("Peter Doe, John", "John", "Peter", "Doe")]
        [InlineData("Doe, J", "J", null, "Doe")]
        [InlineData("Doe,J.", "J", null, "Doe")]
        [InlineData("John Peter-Doe", "John", null, "Peter-Doe")]
        [InlineData("John-Luc Doe", "John-Luc", null, "Doe")]
        // NOTE: Does not detect reversed order without a comma
        [InlineData("Doe John", "Doe", null, "John")]
        [InlineData("Doe J", "Doe", null, "J")] 
        [InlineData("Doe J.", "Doe", null, "J")]
        public void Test1(string text, string firstName, string middleName, string lastName)
        {
            if (PersonName.TryParse(text, out var personName))
            {
                personName.FirstName.ShouldBe(firstName);
                personName.MiddleName.ShouldBe(middleName);
                personName.LastName.ShouldBe(lastName);
            }
            else
            {
                Assert.Fail($"Failed to parse '{text}'");
            }
        }
    }
}