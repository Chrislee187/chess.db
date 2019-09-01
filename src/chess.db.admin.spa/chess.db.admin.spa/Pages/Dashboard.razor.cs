using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using chess.db.admin.web.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;

namespace chess.db.admin.spa.Pages
{
    public class DashboardComponent : ComponentBase
    {
        [Inject] public IDashboardService DashboardService { get; set; }
        public int GameCount { get; set; }
        public int SiteCount { get; set; }
        public IEnumerable<ItemEntry> Sites { get; set; }

        public const int TopXCount = 10;

        public bool Loading { get; set; } = true;
        protected override async Task OnInitAsync()
        {
            Loading = true;
            GameCount = await DashboardService.GamesCount();

            var games = await DashboardService.GetGamesAsync();
            var sites = games.Select(g => g.Site.Name).ToList();
            Sites = TopX(sites);
            SiteCount = sites.Distinct().Count();

            Loading = false;
//            StateHasChanged();
        }
        protected override async Task OnAfterRenderAsync()
        {
//            base.OnAfterRender();

        }

        public IEnumerable<ItemEntry> TopX(IEnumerable<string> items)
        {
            return items.GroupBy(g => g)
                .OrderByDescending(g => g.Count())
                .Take(TopXCount)
                .Select(g => new ItemEntry { Name = g.Key, Count = g.Count() });

        }
        public class ItemEntry
        {
            public int Count { get; set; }
            public string Name { get; set; }
        }
    }
}