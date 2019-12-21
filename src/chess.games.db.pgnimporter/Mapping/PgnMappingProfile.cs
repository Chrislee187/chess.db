using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using AutoMapper;
using chess.games.db.api;
using chess.games.db.Entities;
using chess.games.db.pgnimporter.Extensions;
using PgnGame = PgnReader.PgnGame;

namespace chess.games.db.pgnimporter.Mapping
{
    // ReSharper disable once UnusedMember.Global - Discovered dynamically by AutoMapper
    public class PgnMappingProfile : Profile
    {
        private static readonly List<string> MandatoryTags = new List<string>(
            new[] { "event", "site", "round", "date", "black", "white", "result" });

        public PgnMappingProfile()
        {
            CreateMap<PgnGame, PgnImport>().ConvertUsing((g, q) => MapToPgnDeDupeQueue(g));
        }

        private PgnImport MapToPgnDeDupeQueue(PgnGame pgnGame)
        {
            var game = new PgnImport
            {
                Event = pgnGame.Event,
                Black = pgnGame.Black,
                Site = pgnGame.Site,
                White = pgnGame.White,
                Round = pgnGame.Round,
                Date = pgnGame.Date.RevertDateToText(),
                Result = pgnGame.Result.RevertToText(),
                MoveList = NormaliseMoveText(pgnGame.MoveText)
            };

            var usedTags = new List<string>(MandatoryTags);

            var pairs = pgnGame.TagPairs.ToDictionary(k => k.Name, v => v.Value);

            void ParseCustomTag(string tag, Action<string> valueSetter)
            {
                if (pairs.ContainsKeyInsensitive(tag))
                {
                    usedTags.Add(tag.ToLowerInvariant());
                    var ecoValue = pairs.GetValueInsensitive(tag);
                    if (!string.IsNullOrWhiteSpace(ecoValue))
                    {
                        valueSetter(ecoValue);
                    }
                }
            }

            ParseCustomTag("ECO", v => game.Eco = v);
            ParseCustomTag("WhiteELO", v => game.WhiteElo = v);
            ParseCustomTag("BlackELO", v => game.BlackElo = v);

            var customTags = pgnGame.TagPairs
                .Where(t => !usedTags.Contains(t.Name.ToLowerInvariant()))
                .ToDictionary(k => k.Name, v => v.Value);

            game.CustomTagsJson = customTags.Any() 
                ? JsonSerializer.Serialize(customTags) 
                : null;

            return game;
        }

        private static string NormaliseMoveText(string moveText)
        {
            return moveText
                .Replace("\n", " ")
                .Replace("\r", " ")
                .Replace("  ", " ")
                .Replace("{ ", "{")
                .Replace(" }", "}");
        }
    }
}