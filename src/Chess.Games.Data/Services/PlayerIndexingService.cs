using Chess.Games.Data.Entities;
using Chess.Games.Data.Repos;

namespace Chess.Games.Data.Services;

public class PlayerIndexingService : IPlayerIndexingService
{
    private readonly IPlayerRepository _playerRepository;

    private IDictionary<string, PlayerEntity>? _index = null;

    public PlayerIndexingService(IPlayerRepository eventRepository)
    {
        _playerRepository = eventRepository;
    }
    public PlayerEntity TryAdd(string eventText)
    {
        _index = GetIndex();

        if (_index.TryGetValue(eventText, out var entity))
        {
            return entity;
        }

        entity = new PlayerEntity()
        {
            Name = eventText
        };
        _index.Add(entity.Name, entity);
        _playerRepository.Add(entity);

        return entity;
    }

    private IDictionary<string, PlayerEntity> GetIndex() => _index ??= _playerRepository
        .Get()
        .ToDictionary(e => e.Name);
}



public interface IPlayerIndexingService
{
    public PlayerEntity TryAdd(string eventText);
}