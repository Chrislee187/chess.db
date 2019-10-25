using AutoMapper;
using chess.db.webapi.Models;
using chess.games.db.Entities;

namespace chess.db.webapi.Mapping.Profiles
{
    public class PlayersProfile : Profile
    {
        public PlayersProfile()
        {
            CreateMap<Player, PlayerDto>();
        }
    }
}