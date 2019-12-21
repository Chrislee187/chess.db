using System.Collections.Generic;
using System.Text.Json;
using AutoMapper;
using chess.games.db.Entities;
using chess.games.db.pgnimporter.Extensions;
using chess.games.db.pgnimporter.Mapping;
using NUnit.Framework;
using Shouldly;

namespace chess.games.db.pgnimporter.unit.tests
{
    public class PgnImportQueueMappingTests
    {
        private IMapper _mapper;

        [SetUp]
        public void Setup()
        {
            AutoMapperFactory.MapperConfiguration.AssertConfigurationIsValid();

            _mapper = AutoMapperFactory.Create();
        }

        [Test]
        public void Valid_Pgn_IsMapped()
        {
            var pgnGame = new PgnGameBuilder().Build();

            var actual = _mapper.Map<PgnImportQueue>(pgnGame);

            actual.Site.ShouldBe(pgnGame.Site);
            actual.Event.ShouldBe(pgnGame.Event);
            actual.Result.ShouldBe(pgnGame.Result.RevertToText());
            actual.Round.ShouldBe(pgnGame.Round);
            actual.Black.ShouldBe(pgnGame.Black);
            actual.White.ShouldBe(pgnGame.White);
            actual.Date.ShouldBe(pgnGame.Date.RevertDateToText());
            actual.MoveList.ShouldBe(pgnGame.NormaliseMoveText());
        }

        [Test]
        public void Partial_dates_are_handled()
        {
            var pgnGame = new PgnGameBuilder()
                .WithYear("??")
                .WithMonth("??")
                .WithDay("??")
                .Build();

            var actual = _mapper.Map<PgnImportQueue>(pgnGame);

            actual.Date.ShouldBe("????.??.??");
        }

        [Test]
        public void ECO_and_ELO_values_are_explicitly_mapped()
        {
            var pgnGame = new PgnGameBuilder()
                .WithEco("C03")
                .WithWhiteElo(1111)
                .WithBlackElo(2222)
                .Build();

            var actual = _mapper.Map<PgnImportQueue>(pgnGame);

            actual.Eco.ShouldBe("C03");
            actual.WhiteElo.ShouldBe("1111");
            actual.BlackElo.ShouldBe("2222");
            actual.CustomTagsJson.ShouldBeNull();
        }

        [Test]
        public void ECO_tag_name_is_case_insensitive()
        {
            var pgnGame = new PgnGameBuilder()
                .WithCustomTag("eCo", "D04")
                .Build();

            var actual = _mapper.Map<PgnImportQueue>(pgnGame);

            actual.Eco.ShouldBe("D04");
        }

        [Test]
        public void WhiteElo_tag_name_is_case_insensitive()
        {
            var pgnGame = new PgnGameBuilder()
                .WithCustomTag("wHiTeElO", "1234")
                .Build();

            var actual = _mapper.Map<PgnImportQueue>(pgnGame);

            actual.WhiteElo.ShouldBe("1234");
        }
        [Test]
        public void BlackElo_tag_name_is_case_insensitive()
        {
            var pgnGame = new PgnGameBuilder()
                .WithCustomTag("BlaCkELO", "1234")
                .Build();

            var actual = _mapper.Map<PgnImportQueue>(pgnGame);

            actual.BlackElo.ShouldBe("1234");
        }

        [Test]
        public void CustomTags_are_stored()
        {
            var pgnGame = new PgnGameBuilder()
                .WithCustomTag("Custom", "value")
                .Build();

            var actual = _mapper.Map<PgnImportQueue>(pgnGame);

            var customTags = JsonSerializer.Deserialize<Dictionary<string,string>>(actual.CustomTagsJson);
            customTags.ContainsKey("Custom").ShouldBeTrue();
            customTags["Custom"].ShouldBe("value");
        }

    }
}