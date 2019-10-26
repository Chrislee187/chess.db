using System;
using System.Linq;
using chess.games.db.Entities;

namespace chess.games.db.api.Players
{
    public class PlayersRepository : RepositoryBase, IPlayersRepository
    {
        private readonly ChessGamesDbContext _chessGamesDbContext;

        public PlayersRepository(ChessGamesDbContext chessGamesDbContext) 
            => _chessGamesDbContext = chessGamesDbContext;

        public IQueryable<Player> GetPlayers()
            => _chessGamesDbContext.Players;

        public IQueryable<Player> GetPlayers(
            PlayersFilters filters,
            PlayersSearchQuery query) 
            => Reduce(GetPlayers(), filters, query);

        public Player GetPlayer(Guid id)
            => _chessGamesDbContext.Players.Find(id);


    }
}