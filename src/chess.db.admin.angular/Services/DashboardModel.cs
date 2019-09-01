using System.Collections.Generic;
using Microsoft.AspNetCore.Cors.Infrastructure;

namespace chess.db.admin.angular.Services
{
    public class DashboardModel
    {
        public int TotalGames { get; set; }
        public int TotalEvents { get; set; }
        public int TotalPlayers { get; set; }
        public int TotalSites { get; set; }
        public IEnumerable<TopXItem> TopPlayers { get; set; }
    }

    public class TopXItem
    {
        public string Name { get; set; }
        public int Count { get; set; }
    }
    
}