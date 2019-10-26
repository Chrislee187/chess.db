namespace chess.db.webapi.ResourceParameters
{
    public class PgnPlayerResourceParameters
    {
        public string NameFilter { get; set; }
        public string SearchQuery { get; set; }
    }

    public class PlayerResourceParameters
    {
        public string FirstnameFilter { get; set; }
        public string MiddlenameFilter { get; set; }
        public string LastnameFilter { get; set; }
        public string SearchQuery { get; set; }
    }
}