

using Chess.Games.Data;

await using (ChessGamesDbContext context = new ChessGamesDbContext())
{
    await context.Database.EnsureCreatedAsync();
}

GetEvents();

void GetEvents()
{
    using var ctx = new ChessGamesDbContext();
    var evts = ctx.Events.ToList();

    foreach (var eventEntity in evts)
    {
        Console.WriteLine(eventEntity.Name);
    }
}

// namespace Chess.PGNImporter;
//
// internal class Program
// {
//
//
//     static void Main(string[] args)
//     {
//
//     }
//
//
//     /* BOard state
// * whether a square has a piece or not can be simply encoded in 64 bits (a long)
// * a piece and it's colour can encoded in 4 bits
// * so... 32 pieces can be encoded in a guid
// *
// * {
// *      "boardstate" : {
// *          "occupied": (long),
// *          "pieces": (guid),
// *          "white-to-move": (bool),
// *          "white-has-castled: (bool),
// *          "black-has-castled: (bool),
// *      }
// * }
// */
//
// }
