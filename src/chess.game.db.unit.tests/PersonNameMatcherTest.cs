using System;
using System.Collections.Generic;
using System.Linq;
using chess.games.db.api;
using chess.games.db.Entities;
using NUnit.Framework;
using Shouldly;

namespace chess.game.db.unit.tests
{
    [TestFixture]
    public class PersonNameMatcherTests
    {
        private PersonNameMatcher _matcher;

        private const string AnyFirstName = "first";
        private const string AnyMiddleName = "middle";
        private static string AnyLastName = "last";
        private static string AnyOtherLastName = "other-last";
        private static readonly PersonName AnyPerson = new PersonName(AnyFirstName, AnyMiddleName, AnyLastName);
        private static readonly Player AnyPlayer = new Player { Firstname = AnyFirstName, LastName = AnyLastName, OtherNames = AnyMiddleName };
        private static readonly Player AnyOtherPlayer = new Player { Firstname = AnyFirstName, LastName = AnyOtherLastName, OtherNames = AnyMiddleName };

        [SetUp]
        public void SetUp()
        {
            _matcher = new PersonNameMatcher();
        }

        [Test]
        public void NoMatch_On_Empty_List()
        {
            _matcher.Match(AnyPerson, new List<Player>()).ShouldBeNull();
        }

        [Test]
        public void Should_match_on_exact()
        {
            _matcher.Match(AnyPerson, new List<Player>(){ AnyPlayer })
                .ShouldBe(AnyPlayer);
        }

        [Test]
        public void Should_throw_when_matching_against_multiple_lastnames()
        {
            Should.Throw<Exception>(() => 
                _matcher.Match(AnyPerson, new List<Player>() {AnyPlayer, AnyOtherPlayer}));
        }

        [TestCase("f", "", "last")]
        [TestCase("F", "", "last")]
        public void Should_match_initials_to_full_names(string first, string middle, string last)
        {
            var player = new Player("first", "middle", "last");

            var personName = new PersonName(first, middle, last);

            _matcher.Match(personName, new List<Player>() { player })
                .ShouldBe(player);
        }

        [TestCase("first", "", "last")]
        [TestCase("First", "", "last")]
        public void Should_update_initials_to_full_names(string first, string middle, string last)
        {
            var player = new Player("f", "middle", "last");

            var personName = new PersonName(first, middle, last);

            _matcher.Match(personName, new List<Player>() { player })
                .ShouldBe(player);

            player.Firstname.ShouldBe(first);
        }

    }

}