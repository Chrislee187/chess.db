﻿using System.Linq;
using AspNetCore.MVC.RESTful.Configuration;
using chess.games.db.Entities;

namespace chess.games.db.api.Players
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class GetPlayersEntitySearch : IEntitySearch<Player>
    {
        public IQueryable<Player> Search(IQueryable<Player> entities, string searchText)
        {
            return entities.Where(p => p.Firstname.Contains(searchText)
                                    || p.Middlenames.Contains(searchText)
                                    || p.Surname.Contains(searchText));
        }
    }
}