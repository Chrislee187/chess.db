using System;
using System.Linq;
using NUnit.Framework;
using NUnit.Framework.Internal;
using Shouldly;

namespace Tests
{
    public class PgnPlayerNameAnalyserTests
    {
        [SetUp]
        public void Setup()
        {
        }
        [TestCase("King", "", "", "", "King")]
        public void Should_map_single_word_to_last_name(string name, string title, string first, string middle, string last)
        {
            var a = new NameAnalyser();

            var result = a.Analyse(name);

            ShouldMapNameParts(title, first, middle, last, result);
        }

        [TestCase("Martin King", "", "Martin", "", "King")]
        [TestCase("King, Martin", "", "Martin", "", "King")]
        [TestCase("Dr King", "Dr", "", "", "King")]
        [TestCase("Dr. King", "Dr", "", "", "King")]
        [TestCase("M King", "", "M", "", "King")]
        [TestCase("M. King", "", "M", "", "King")]
        [TestCase("King, M", "", "M", "", "King")]
        [TestCase("King, M.", "", "M", "", "King")]
        [TestCase("King, Dr.", "Dr", "", "", "King")]
        [TestCase("King, Dr", "Dr", "", "", "King")]
        public void Should_map_two_word_names(string name, string title, string first, string middle, string last)
        {
            var a = new NameAnalyser();

            var result = a.Analyse(name);

            ShouldMapNameParts(title, first, middle, last, result);
        }

        [TestCase("Dr. Martin Luther King","Dr", "Martin", "Luther", "King")]
        [TestCase("Dr. King, Martin Luther", "Dr", "Martin", "Luther", "King")]
        public void Should_identify_typical_three_part_name_with_titleation(string name, string title, string first, string middle, string last)
        {
            var a = new NameAnalyser();

            var result = a.Analyse(name);

            ShouldMapNameParts(title, first, middle, last, result);
        }

        [TestCase("Dr. King, Martin L.", "Dr", "Martin", "L", "King")]
        [TestCase("Dr. King, M. L.", "Dr", "M", "L", "King")]
        [TestCase("Dr. King, ML", "Dr", "M", "L", "King")]
        [TestCase("Dr. Martin L. King", "Dr", "Martin", "L", "King")]
        [TestCase("Dr. Martin L King", "Dr", "Martin", "L", "King")]
        [TestCase("Dr. M. L. King", "Dr", "M", "L", "King")]
        [TestCase("Dr. M L King", "Dr", "M", "L", "King")]
        [TestCase("Dr. ML King", "Dr", "M", "L", "King")]
        public void Should_identify_initials(string name, string title, string first, string middle, string last)
        {
            var a = new NameAnalyser();
            
            var result = a.Analyse(name);
            
            ShouldMapNameParts(title, first, middle, last, result);
        }


        private static void ShouldMapNameParts(string title, string first, string middle, string last, Name result)
        {
            result.Title.ShouldBe(title);
            result.First.ShouldBe(first);
            result.Middle.ShouldBe(middle);
            result.Last.ShouldBe(last);
        }
    }

    public class NameAnalyser
    {
        public Name Analyse(string name)
        {
            var parts = name.Split(' ');
            Name result;
            switch (parts.Length)
            {
                case 1:

                    result = SingleWordName(parts);

                    break;
                case 2:
                    // Only one 'word' assume its the last name
                    result = TwoWordName(parts);
                    break;
                default:
                    throw new FormatException($"'{name}' has to many parts!");
            }

            return result;
        }

        private Name TwoWordName(string[] parts)
        {
            if (parts.Length != 2) throw new FormatException($"Expected string[1] but got string[{parts.Length}]");

            if(IsTitle(parts[0]))
            {
                // assume Title Lastname
                return new Name() {Title = parts[0], Last = parts[1]};
            }

            if (IsLastName(parts[0]))
            {
                if (IsTitle(parts[1]))
                {
                    return new Name {Title = parts[1], Last = parts[0]};
                }

                return new Name() {First = parts[1], Last = parts[0]};
            }

            /* Two words could be in the forms;
                Martin King
                King, Martin
                Dr King
                Dr. King
                M King
                N. King
                King, M
                King, Dr.
                King, Dr
             */
            return null;
        }

        private Name SingleWordName(string[] parts)
        {
            if(parts.Length != 1) throw new FormatException($"Expected string[1] but got string[{parts.Length}]");
            // Only one 'word' assume its the last name
            return new Name {Last = parts[0]};
        }

        private bool IsLastName(string word) => word.EndsWith(",");
        private bool IsTitle(string word) => new[] {"dr", "mr", "ms", "miss", "mrs", "sir"}.Any(t => t.StartsWith(word.ToLowerInvariant()));
        private string TrimEndPunctuation(string text) => text.TrimEnd('.').TrimEnd(',');
    }

    public class Name
    {
        public string Title { get; set; } = "";
        public string First { get; set; } = "";
        public string Middle { get; set; } = "";
        public string Last { get; set; } = "";
    }
}