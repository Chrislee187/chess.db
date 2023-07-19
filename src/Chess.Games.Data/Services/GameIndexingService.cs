using Chess.Games.Data.Entities;
using Chess.Games.Data.Repos;

namespace Chess.Games.Data.Services;

public class GameIndexingService : IGameIndexingService
{
    private readonly IGameRepository _gameRepository;

    public GameIndexingService(IGameRepository gameRepository)
    {
        _gameRepository = gameRepository;
    }
    public bool TryAdd(GameEntity game, out GameEntity? existing)
    {
        existing = _gameRepository.FirstOrDefault(g => g.SourceEventText == game.SourceEventText
                                                       && g.SourceSiteText == game.SourceSiteText
                                                       && g.SourceWhitePlayerText == game.SourceWhitePlayerText
                                                       && g.SourceBlackPlayerText == game.SourceBlackPlayerText
                                                       && g.Round == game.Round);
        if (existing != null)
        {    
            return false;
        }

        _gameRepository.Add(game);
        // _gameRepository.Save();
        return true;
    }
}


public interface IGameIndexingService
{
    public bool TryAdd(GameEntity game, out GameEntity? existing);
}