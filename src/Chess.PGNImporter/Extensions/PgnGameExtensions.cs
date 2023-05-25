using PgnReader;

namespace chess.games.db.pgnimporter.Extensions
{
    public static class PgnGameExtensions
    {
        public static string NormaliseMoveText(this PgnGame game) 
            => NormaliseMoveText(game.MoveText);


        public static string NormaliseMoveText(string moveText) =>
            moveText
                .Replace("\n", " ")
                .Replace("\r", " ")
                .Replace("  ", " ")
                .Replace("{ ", "{")
                .Replace(" }", "}");

    }
}