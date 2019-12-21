using System;
using System.Collections.Generic;
using System.Linq;

namespace chess.games.db.api
{
    public static class StringDictionaryExtensions
    {
        public static bool ContainsKeyInsensitive(this IDictionary<string, string> dict, string key)
            => dict.Any(d => d.Key.Equals(key, StringComparison.InvariantCultureIgnoreCase));
        public static string GetValueInsensitive(this IDictionary<string, string> dict, string key)
            => dict.Single(d => d.Key.Equals(key, StringComparison.InvariantCultureIgnoreCase)).Value;
    }
}