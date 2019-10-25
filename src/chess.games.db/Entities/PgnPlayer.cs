namespace chess.games.db.Entities
{
    public class PgnPlayer : DbEntity, IHaveAName
    {
        public string Name { get; set; }
    }
//
//    public class PgnPlayer : DbEntity
//    {
//        public string Firstname { get; set; }
//        public string Surname { get; set; }
//        public string Middlenames { get; set; }
//    }

}