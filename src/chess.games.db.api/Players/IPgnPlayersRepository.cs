using AspNetCore.MVC.RESTful.Repositories;
using chess.games.db.Entities;

namespace chess.games.db.api.Players
{
    public interface IPgnPlayersRepository : IResourceRepository<PgnPlayer>
    {
        PgnPlayer Get(string name);
    }
}