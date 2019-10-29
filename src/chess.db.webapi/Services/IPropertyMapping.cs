using System.Collections.Generic;
using chess.games.db.api.Helpers;

namespace chess.db.webapi.Services
{
    public interface IPropertyMapping
    {
        Dictionary<string, OrderByPropertyMappingValue> MappingDictionary { get; }
    }
}