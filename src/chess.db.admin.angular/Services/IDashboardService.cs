using System.Threading.Tasks;

namespace chess.db.admin.angular.Services
{
    public interface IDashboardService
    {
        Task<DashboardModel> GetAsync();
        Task<int> GetPlayersCountAsync();
        Task<int> GetGamesCountAsync();
        Task<int> GetEventsCountAsync();
        Task<int> GetSitesCountAsync();
    }
}