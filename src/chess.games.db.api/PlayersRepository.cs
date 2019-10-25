using System;
using System.Linq;
using chess.games.db.Entities;

namespace chess.games.db.api
{
    public interface IPlayersRepository
    {
        IQueryable<Player> GetPlayers();
        Player GetPlayer(Guid id);
    }

    public class PlayersRepository : IPlayersRepository
    {
        private readonly ChessGamesDbContext _chessGamesDbContext;

        public PlayersRepository(ChessGamesDbContext chessGamesDbContext)
        {
            _chessGamesDbContext = chessGamesDbContext;
        }

        public IQueryable<Player> GetPlayers() => _chessGamesDbContext.Players;

        public Player GetPlayer(Guid id) => _chessGamesDbContext.Players.Find(id);
    }
}