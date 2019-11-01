using AspNetCore.MVC.RESTful.Parameters;

namespace chess.db.webapi.ResourceParameters
{
    public class GetPgnPlayersParameters : CommonResourceParameters
    {
        public string NameFilter { get; set; }
    }
}