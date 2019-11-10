using AspNetCore.MVC.RESTful.Parameters;

namespace chess.db.webapi.ResourceParameters
{
    public class GetPlayersParameters : CommonResourcesGetParameters
    {
        public string FirstnameFilter { get; set; }
        public string MiddlenameFilter { get; set; }
        public string LastnameFilter { get; set; }

    }
}