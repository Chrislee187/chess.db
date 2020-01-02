using chess.games.db.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace chess.games.db.api
{
    public class PersonNameMatcher
    {

        public Player Match(PersonName personName, IReadOnlyList<Player> players)
        {
            if (!players.Any()) return null;

            if (players.Select(p => p.LastName.ToLowerInvariant()).Distinct().Count() != 1)
            {
                throw new Exception("Matching can only be performed on players with the same surname");
            }

            var exact = players.SingleOrDefault(p => MatchAllNames(personName, p));
            if (exact != null)
            {
                return exact;
            }

            if (string.IsNullOrEmpty(personName.Firstname))
            {
                return null;
            }

            var matchingFirstInitial = players
                .Where(p => !string.IsNullOrEmpty(p.Firstname))
                .Where(p => p.Firstname.ToLowerInvariant()[0] == personName.Firstname.ToLowerInvariant()[0])
                .ToList();

            if (matchingFirstInitial.Any())
            {
                if (matchingFirstInitial.Count() == 1)
                {
                    var player = matchingFirstInitial.First();
                    if (personName.Firstname.Length > player.Firstname.Length)
                    {
                        player.Firstname = personName.Firstname;
                    }
                    return player;
                }
                else
                {
                    // TODO: Use middlename to further disambiguate
                    Console.WriteLine();
                    return null;
                }
            }

            return null;
        }

        private static bool MatchAllNames(PersonName personName, Player player)
        {
            return string.Equals(personName.Firstname, player.Firstname, StringComparison.InvariantCultureIgnoreCase)
                   && string.Equals(personName.Middlename, player.OtherNames, StringComparison.InvariantCultureIgnoreCase)
                   && string.Equals(personName.Lastname, player.LastName, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}