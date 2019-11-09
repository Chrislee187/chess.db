﻿using System.Linq;
using AspNetCore.MVC.RESTful.Parameters;
using chess.games.db.Entities;

namespace chess.games.db.api.PgnPlayers
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class GetPgnPlayersSearchQuery : ResourceQuery<PgnPlayer>
    {
        public string QueryText { get; set; }

        public override bool Empty => string.IsNullOrEmpty(QueryText);

        protected override IQueryable<PgnPlayer> ApplyQuery(IQueryable<PgnPlayer> resources) 
            => resources.Where(p => p.Name.Contains(QueryText));
    }
}