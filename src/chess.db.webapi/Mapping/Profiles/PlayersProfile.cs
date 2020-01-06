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
            MapPlayers();
            MapPlayerParameters();
        }

        private void MapPlayers()
        {
            CreateMap<Player, PlayerDto>()
                .ForMember(m => m.Lastname, o => o.MapFrom(i => i.LastName))
                ;
            CreateMap<PlayerCreationDto, Player>()
                .ForMember(m => m.LastName, o => o.MapFrom(i => i.Lastname))
                ;

            CreateMap<PlayerUpdateDto, Player>()
                .ForMember(m => m.LastName, o => o.MapFrom(i => i.Lastname))
                ;
            CreateMap<Player, PlayerUpdateDto>()
                .ForMember(m => m.Lastname, o => o.MapFrom(i => i.LastName))
                ;
        }

        private void MapPlayerParameters()
        {
            CreateMap<GetPlayersFilters, GetPlayersEntityFilter>()
                .ForMember(m => m.Firstname, o => o.MapFrom(i => i.FirstnameFilter))
                .ForMember(m => m.Middlename, o => o.MapFrom(i => i.MiddlenameFilter))
                .ForMember(m => m.Lastname, o => o.MapFrom(i => i.LastnameFilter))
                ;
            CreateMap<GetPlayersEntityFilter, GetPlayersFilters>()
                .ForMember(m => m.FirstnameFilter, o => o.MapFrom(i => i.Firstname))
                .ForMember(m => m.MiddlenameFilter, o => o.MapFrom(i => i.Middlename))
                .ForMember(m => m.LastnameFilter, o => o.MapFrom(i => i.Lastname))
                ;

        }
    }

    public class GamesProfile : Profile
    {
        public GamesProfile()
        {
            MapGames();
        }

        private void MapGames()
        {
            CreateMap<Game, GameDto>()
                .ForMember(d => d.Black, o => o.MapFrom(s => s.Black.ToString()))
                .ForMember(d => d.BlackId, o => o.MapFrom(s => s.Black.Id))
                .ForMember(d => d.White, o => o.MapFrom(s => s.White.ToString()))
                .ForMember(d => d.WhiteId, o => o.MapFrom(s => s.White.Id))
                .ForMember(d => d.Event, o => o.MapFrom(s => s.Event.Name))
                .ForMember(d => d.EventId, o => o.MapFrom(s => s.Event.Id))
                .ForMember(d => d.Site, o => o.MapFrom(s => s.Site.Name))
                .ForMember(d => d.SiteId, o => o.MapFrom(s => s.Site.Id))
                .ForMember(d => d.Date, o => o.MapFrom(s => s.Date))
                .ForMember(d => d.Moves, o => o.MapFrom(s => s.MoveText))
                .ForMember(d => d.Result, o => o.MapFrom(s => s.Result.RevertToText()))
                .ForMember(d => d.Round, o => o.MapFrom(s => s.Round))
                ;

            CreateMap<Site, SiteDto>();
            CreateMap<Event, EventDto>();
        }
    }

}