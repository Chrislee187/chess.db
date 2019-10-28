using System.Collections.Generic;
using chess.games.db.Entities;

namespace chess.games.db.api.Players
{
    public interface IPgnPlayersRepository : IRepositoryBase<PgnPlayer>
    {
        IEnumerable<PgnPlayer> Get(
            PgnPlayersFilters filters, 
            PgnPlayersSearchQuery searchQuery
            );

        PgnPlayer Get(string name);
    }
}