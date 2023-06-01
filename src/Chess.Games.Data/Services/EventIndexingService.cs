using Chess.Games.Data.Entities;
using Chess.Games.Data.Repos;

namespace Chess.Games.Data.Services;

public class EventIndexingService : IEventIndexingService
{
    private readonly IEventRepository _eventRepository;

    private IDictionary<string, EventEntity>? _index;

    public EventIndexingService(IEventRepository eventRepository)
    {
        _eventRepository = eventRepository;
    }
    public EventEntity TryAdd(string eventText)
    {
        _index = GetIndex();

        if (_index.TryGetValue(eventText, out var entity))
        {
            return entity;
        }

        entity = new EventEntity()
        {
            Name = eventText
        };
        _eventRepository.Add(entity);
        _index.Add(entity.Name, entity);

        return entity;
    }

    private IDictionary<string, EventEntity> GetIndex() => _index ??= _eventRepository
        .GetAll()
        .ToDictionary(e => e.Name);
}

public interface IEventIndexingService
{
    public EventEntity TryAdd(string eventText);
}