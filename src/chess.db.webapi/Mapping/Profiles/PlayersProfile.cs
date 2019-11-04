using System;
using AspNetCore.MVC.RESTful.Parameters;
using AutoMapper;
using chess.db.webapi.Models;
using chess.db.webapi.ResourceParameters;
using chess.games.db.api.PgnPlayers;
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
            
            CreateMap<PlayerUpdateDto, Player>()
                .ForMember(m => m.Surname, o => o.MapFrom(i => i.Lastname))
                ;
            CreateMap<Player, PlayerUpdateDto>()
                .ForMember(m => m.Lastname, o => o.MapFrom(i => i.Surname))
                ;
        }

        private void MapPlayerParameters()
        {
            CreateMap<GetPlayersParameters, GetPlayersFilters>()
                .ForMember(m => m.Firstname, o => o.MapFrom(i => i.FirstnameFilter))
                .ForMember(m => m.Middlename, o => o.MapFrom(i => i.MiddlenameFilter))
                .ForMember(m => m.Lastname, o => o.MapFrom(i => i.LastnameFilter))
                ;
            CreateMap<GetPlayersFilters, GetPlayersParameters>()
                .ForMember(m => m.FirstnameFilter, o => o.MapFrom(i => i.Firstname))
                .ForMember(m => m.MiddlenameFilter, o => o.MapFrom(i => i.Middlename))
                .ForMember(m => m.LastnameFilter, o => o.MapFrom(i => i.Lastname))
                ;
            
            CreateMap<GetPlayersParameters, GetPlayersSearchQuery>()
                .ForMember(m => m.QueryText,
                    o => o.MapFrom(i => i.SearchQuery));
            CreateMap<GetPlayersSearchQuery, GetPlayersParameters>()
                .ForMember(m => m.SearchQuery,
                    o => o.MapFrom(i => i.QueryText));

            CreateMap<GetPlayersParameters, PaginationParameters>()
                .ForMember(m => m.Page, o => o.MapFrom(i => i.PageNumber));
            CreateMap<PaginationParameters, GetPlayersParameters>()
                .ForMember(m => m.PageNumber, o => o.MapFrom(i => i.Page));

            CreateMap<OrderByParameters, GetPlayersParameters>()
                .ForMember(m => m.OrderBy, o => o.MapFrom(i => i.Clause));
            CreateMap<GetPlayersParameters, OrderByParameters>()
                .ForMember(m => m.Clause, o => o.MapFrom(i => i.OrderBy));
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
            CreateMap<GetPgnPlayersParameters, GetPgnPlayersFilters>()
                .ForMember(m => m.Name,
                    o => o.MapFrom(i => i.NameFilter));
            CreateMap<GetPgnPlayersFilters, GetPgnPlayersParameters>()
                .ForMember(m => m.NameFilter,
                    o => o.MapFrom(i => i.Name));

            CreateMap<GetPgnPlayersParameters, GetPgnPlayersSearchQuery>()
                .ForMember(m => m.QueryText,
                    o => o.MapFrom(i => i.SearchQuery));
            CreateMap<GetPgnPlayersSearchQuery, GetPgnPlayersParameters>()
                .ForMember(m => m.SearchQuery, o => o.MapFrom(i => i.QueryText));

            CreateMap<GetPgnPlayersParameters, PaginationParameters>()
                .ForMember(m => m.Page, o => o.MapFrom(i => i.PageNumber));
            CreateMap<PaginationParameters, GetPgnPlayersParameters>()
                .ForMember(m => m.PageNumber, o => o.MapFrom(i => i.Page));

            CreateMap<OrderByParameters, GetPgnPlayersParameters>()
                .ForMember(m => m.OrderBy, o => o.MapFrom(i => i.Clause));
            CreateMap<GetPgnPlayersParameters, OrderByParameters>()
                .ForMember(m => m.Clause, o => o.MapFrom(i => i.OrderBy));
        }
    }
}