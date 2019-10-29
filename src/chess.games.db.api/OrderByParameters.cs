using System.Collections.Generic;
using chess.games.db.api.Helpers;

namespace chess.games.db.api
{
    public class OrderByParameters
    {

        public static OrderByParameters Default { get; } = new OrderByParameters();

        public string Clause { get; set; }

        public IDictionary<string, OrderByPropertyMappingValue> Mappings { get; set; } = new Dictionary<string, OrderByPropertyMappingValue>();
    }
}