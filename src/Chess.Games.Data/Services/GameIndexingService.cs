using Chess.Games.Data.Entities;
using Chess.Games.Data.Repos;
using EasyEF.Entities;

namespace Chess.Games.Data.Services;

public class GameIndexingService : IGameIndexingService
{
    private readonly IGameRepository _gameRepository;

    private IDictionary<string, GameEntity>? _index = null;

    public GameIndexingService(IGameRepository gameRepository)
    {
        _gameRepository = gameRepository;
    }
    public bool TryAdd(GameEntity game)
    {
        if (_gameRepository.Exists(game.SourceEventText, game.SourceSiteText, game.SourceWhitePlayerText,
                game.SourceBlackPlayerText, game.Round))
        {
            return false;
        }

        _gameRepository.Add(game);

        return true;
    }
}


public interface IGameIndexingService
{
    public bool TryAdd(GameEntity game);
}