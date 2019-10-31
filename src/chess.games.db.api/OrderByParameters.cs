namespace chess.games.db.api
{
    public class OrderByParameters
    {
        public static OrderByParameters Default { get; } = new OrderByParameters();

        public string Clause { get; set; }
    }
}