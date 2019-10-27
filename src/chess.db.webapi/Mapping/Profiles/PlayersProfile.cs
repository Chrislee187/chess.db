using System;
using AutoMapper;
using chess.db.webapi.Models;
using chess.db.webapi.ResourceParameters;
using chess.games.db.api.Players;
using chess.games.db.Entities;

namespace chess.db.webapi.Mapping.Profiles
{
    // Used by Automapper convention no direct reference
    // ReSharper disable once UnusedMember.Global
    public class PlayersProfile : Profile
    {
        public PlayersProfile()
        {
            MapPgnPlayers();
            MapPgnPlayerParameters();


            MapPlayers();
            MapPlayerParameters();
        }

        private void MapPlayers()
        {
            CreateMap<Player, PlayerDto>()
                .ForMember(m => m.Lastname, o =>o.MapFrom(i => i.Surname))
                ;
            CreateMap<PlayerCreationDto, Player>()
                .ForMember(m => m.Surname, o => o.MapFrom(i => i.Lastname))
                ;

        }

        private void MapPlayerParameters()
        {
            CreateMap<PlayerResourceParameters, PlayersFilters>()
                .ForMember(m => m.Firstname, o => o.MapFrom(i => i.FirstnameFilter))
                .ForMember(m => m.Middlename, o => o.MapFrom(i => i.MiddlenameFilter))
                .ForMember(m => m.Lastname, o => o.MapFrom(i => i.LastnameFilter))
                ;

            CreateMap<PlayerResourceParameters, PlayersSearchQuery>()
                .ForMember(m => m.QueryText,
                    o => o.MapFrom(i => i.SearchQuery));
        }

        private void MapPgnPlayers()
        {
            CreateMap<PgnPlayer, PgnPlayerDto>()
                .ForMember(p => p.PlayerId,
                    o => o.MapFrom(i => i.Player.Id == Guid.Empty ? (Guid?) null : i.Player.Id))
                ;

        }

        private void MapPgnPlayerParameters()
        {
            CreateMap<PgnPlayerResourceParameters, PgnPlayersFilters>()
                .ForMember(m => m.Name,
                    o => o.MapFrom(i => i.NameFilter));

            CreateMap<PgnPlayerResourceParameters, PgnPlayersSearchQuery>()
                .ForMember(m => m.QueryText,
                    o => o.MapFrom(i => i.SearchQuery));
        }
    }
}