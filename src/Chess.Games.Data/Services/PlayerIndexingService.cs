using Chess.Games.Data.Entities;
using Chess.Games.Data.Repos;

namespace Chess.Games.Data.Services;

public class PlayerIndexingService : IPlayerIndexingService
{
    private readonly IPlayerRepository _playerRepository;

    private IDictionary<string, PlayerEntity>? _index;

    public PlayerIndexingService(IPlayerRepository repo)
    {
        _playerRepository = repo;
    }
    public PlayerEntity TryAdd(string player)
    {
        _index = GetIndex();

        if (_index.TryGetValue(player, out var entity))
        {
            return entity;
        }

        entity = new PlayerEntity()
        {
            Name = player
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
    public PlayerEntity TryAdd(string player);
}