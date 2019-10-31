using AspNetCore.MVC.RESTful.Controllers;

namespace chess.games.db.Entities
{
    public class Player : DbEntity, IResourceEntity
    {
        public string Firstname { get; set; }
        public string Surname { get; set; }
        public string Middlenames { get; set; }
    }
}