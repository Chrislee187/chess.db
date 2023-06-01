

using Chess.Games.Data;
using Chess.Games.Data.Entities;
using Chess.Games.Data.Repos;

await using (var context = new ChessGamesDbContext())
{
    await context.Database.EnsureCreatedAsync();
}

using var ctx = new ChessGamesDbContext();
var repo = new EventRepo(ctx);

repo.Add(new EventEntity() { Name=$"Test Event at {DateTimeOffset.Now}"});
repo.Save();
GetEvents();

void GetEvents()
{

    var evts = repo.Get().OrderBy(e => e.Name);

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
