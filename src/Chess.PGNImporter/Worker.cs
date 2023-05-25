using Microsoft.Extensions.Logging;
using PgnReader;

namespace Chess.PGNImporter;

public class Worker
{
    private readonly ILogger<Worker> _logger;


    private const string PgnText = @"
[Event ""F/S Return Match""]
[SiteEntity ""Belgrade, Serbia JUG""]
[Date ""1992.11.04""]
[Round ""29""]
[White ""Fischer, Robert J.""]
[Black ""Spassky, Boris V.""]
[Result ""1/2-1/2""]

1. e4 e5 2. Nf3 Nc6 3. Bb5 a6 {This opening is called the Ruy Lopez.}
4. Ba4 Nf6 5. O-O Be7 6. Re1 b5 7. Bb3 d6 8. c3 O-O 9. h3 Nb8 10. d4 Nbd7
11. c4 c6 12. cxb5 axb5 13. Nc3 Bb7 14. Bg5 b4 15. Nb1 h6 16. Bh4 c5 17. dxe5
Nxe4 18. Bxe7 Qxe7 19. exd6 Qf6 20. Nbd2 Nxd6 21. Nc4 Nxc4 22. Bxc4 Nb6
23. Ne5 Rae8 24. Bxf7+ Rxf7 25. Nxf7 Rxe1+ 26. Qxe1 Kxf7 27. Qe3 Qg5 28. Qxg5
hxg5 29. b3 Ke6 30. a3 Kd6 31. axb4 cxb4 32. Ra5 Nd5 33. f3 Bc8 34. Kf2 Bf5
35. Ra7 g6 36. Ra6+ Kc5 37. Ke1 Nf4 38. g3 Nxh3 39. Kd2 Kb5 40. Rd6 Kc5 41. Ra6
Nf2 42. g4 Bd3 43. Re6 1/2-1/2
";
    public Worker(ILogger<Worker> logger)
    {
        _logger = logger;
    }

    public void Execute()
    {
        var games = PgnReader.PgnGame.ReadAllGamesFromString(PgnText);

        foreach (var pgnGame in games)
        {
            ImportGame(pgnGame);


            // Check game doesnt already exist
            // Add game to Cosmos
            // If game does exist?
        }

        // Guid has 16 bytes
        // 4-2-2-2-6



        // read file/path from command line
        // read PGN
        // convert to PGON (PiGeON)
        // * each is a seperate PiGeON object
        // * stored in a separate Cosmos document
        // * Basic schema is that of PGN file
        // * use "extensions" child object for additional data
        //      * Encoded move string
        //          * A single string contain the moves in a normalised D2D4 format for easy comparison
        //              * Might be more efficient to encode somehow and not just use a string
        //      * SanMoves array [ {
        //                          "move": 1,
        //                          "white": {
        //                                  "from": "d2",
        //                                      "to": "d4"
        //                                  "pre-boardstate" : "xxx",
        //                                  "post-boardstate" : "xxx",
        //                                  "piece" : "prbnkq"
        //                          },
        //                          "black": { "from": "d7", "to": "d5" },
        //                      }
        //      *               ]
        //      * PgnMoves array [ {
        //                          "move": 1,
        //                          "white": { "d4" },
        //                          "black": { "d5" },
        //                      }
        //      *               ]
    }

    private void ImportGame(PgnGame pgnGame)
    {
        // var pgonGame = new PgonGame(pgnGame);
        //
        // IChessIndexRepository index = new SqlChessIndexRepo(_services.GetRequiredService<ChessMatchDbContext>());
        // if (!index.TryGetEvent(pgonGame.Event, out var evt))
        // {
        //     evt = index.AddEvent(pgonGame.Event);
        // }
        //
        // pgonGame.Event = evt.Name;
        // pgonGame.EventUid = evt.Uid;
    }
}