using Chess.Games.Data.Entities;
using Chess.Games.Data.Repos;

namespace Chess.Games.Data.Services;

public class SiteIndexingService : ISiteIndexingService
{
    private readonly ISiteRepository _siteRepository;

    private IDictionary<string, SiteEntity>? _index;

    public SiteIndexingService(ISiteRepository eventRepository)
    {
        _siteRepository = eventRepository;
    }
    public SiteEntity TryAdd(string site)
    {
        _index = GetIndex();

        if (_index.TryGetValue(site, out var entity))
        {
            return entity;
        }

        entity = new SiteEntity()
        {
            Name = site
        };
        _siteRepository.Add(entity);
        _index.Add(entity.Name, entity);

        return entity;
    }

    private IDictionary<string, SiteEntity> GetIndex() => _index ??= _siteRepository
        .Get()
        .ToDictionary(e => e.Name);
}


public interface ISiteIndexingService
{
    public SiteEntity TryAdd(string site);
}