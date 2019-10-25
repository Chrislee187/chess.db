using AutoMapper;
using chess.db.webapi.Models;
using chess.db.webapi.ResourceParameters;
using chess.games.db.api.Players;
using chess.games.db.Entities;

namespace chess.db.webapi.Mapping.Profiles
{
    // Used by Automapper convention not a direct reference
    // ReSharper disable once UnusedMember.Global
    public class PlayersProfile : Profile
    {
        public PlayersProfile()
        {
            CreateMap<PgnPlayer, PgnPlayerDto>();

            CreateMap<PgnPlayerResourceParameters, PgnPlayersFilterParams>()
                .ForMember(m => m.Name,
                    o => o.MapFrom(i => i.NameFilter));

            CreateMap<PgnPlayerResourceParameters, PgnPlayersSearchQuery>()
                .ForMember(m => m.QueryText,
                    o => o.MapFrom(i => i.SearchQuery));
        }
    }
}