using System.Collections.Generic;
using chess.games.db.Entities;

namespace chess.games.db.api.Players
{
    public interface IPgnPlayersRepository : IResourceRepositoryBase<PgnPlayer>
    {
        PgnPlayer Get(string name);
    }
}