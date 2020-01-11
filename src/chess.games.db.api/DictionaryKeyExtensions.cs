using System;
using System.Collections.Generic;
using System.Linq;

namespace chess.games.db.api
{
    public static class DictionaryKeyExtensions
    {
        public static bool ContainsKeyInsensitive<T>(this IDictionary<string, T> dict, string key)
            => dict.Any(d => d.Key.Equals(key, StringComparison.InvariantCultureIgnoreCase));
        public static T ValueInsensitive<T>(this IDictionary<string, T> dict, string key)
            => dict.Single(d => d.Key.Equals(key, StringComparison.InvariantCultureIgnoreCase)).Value;
    }
}