using AspNetCore.MVC.RESTful.Parameters;

namespace chess.db.webapi.ResourceParameters
{
    public class GetPgnPlayersParameters : CommonResourceParameters
    {
        public string NameFilter { get; set; }
    }

    public class GetPlayersParameters : CommonResourceParameters
    {
        public string FirstnameFilter { get; set; }
        public string MiddlenameFilter { get; set; }
        public string LastnameFilter { get; set; }

    }
}