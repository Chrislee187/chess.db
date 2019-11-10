using AspNetCore.MVC.RESTful.Parameters;

namespace chess.db.webapi.ResourceParameters
{
    public class GetPgnPlayersParameters : CommonResourcesGetParameters
    {
        public string NameFilter { get; set; }
    }
}