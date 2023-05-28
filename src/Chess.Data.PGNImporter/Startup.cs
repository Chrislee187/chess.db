using System.Diagnostics;
using Chess.Games.Data.Entities;
using Chess.Games.Data.Repos;
using Chess.Games.Data.Services;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PgnReader;

namespace Chess.Data.PGNImporter;

public class Startup 
{
    private readonly ILogger<Startup> _logger;
    private IEventIndexingService _eventIndex;
    private readonly ISiteIndexingService _siteIndex;
    private readonly IPlayerIndexingService _playerIndex;
    private readonly IGameIndexingService _gameIndex;
    private IEventRepository _eventRepo;


    private const string PgnText = @"
[Event ""F/S Return Match""]
[Site ""Belgrade, Serbia JUG""]
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
    public Startup(
        ILogger<Startup> logger,
        IEventRepository eventRepo,
        IEventIndexingService eventIndex,
        ISiteIndexingService siteIndex,
        IPlayerIndexingService playerIndex,
        IGameIndexingService gameIndex)
    {
        _eventRepo = eventRepo;
        _eventIndex = eventIndex;
        _siteIndex = siteIndex;
        _playerIndex = playerIndex;
        _gameIndex = gameIndex;
        _logger = logger;
    }

    public void Execute()
    {
        var games = PgnReader.PgnGame.ReadAllGamesFromString(PgnText);

        foreach (var pgnGame in games)
        {
            Console.WriteLine($"{pgnGame.Event} {pgnGame.White} vs {pgnGame.Black} Round {pgnGame.Round}");

            var evt = _eventIndex.TryAdd(pgnGame.Event);
            var site = _siteIndex.TryAdd(pgnGame.Site);
            var black = _playerIndex.TryAdd(pgnGame.Black);
            var white = _playerIndex.TryAdd(pgnGame.White);
            int.TryParse(pgnGame.Round, out int round);

            
            var game = new GameEntity
            {
                SourceEventText = pgnGame.Event,
                Event = evt,
                // EventId = default,
                SourceSiteText = pgnGame.Site,
                Site = site,
                // SiteId = default,
                SourceWhitePlayerText = pgnGame.White,
                White = white,
                // WhiteId = default,
                SourceBlackPlayerText = pgnGame.Black,
                Black = black,
                // BlackId = default,
                Date = default,
                Round = round,
                Result = PgnGameResultToGameResult(pgnGame.Result),
                SourceMoveText = pgnGame.MoveText
            };

            if (_gameIndex.TryAdd(game))
            {
                _eventRepo.Save(); // NOTE: This is the unit-of-work commit call (DbContext.SaveChanges())

            }


            // ImportGame(pgnGame);


            // Check game doesnt already exist
            // Add game to Cosmos
            // If game does exist?
        }

    }

    private GameResult PgnGameResultToGameResult(PgnGameResult pgnGameResult) =>
        pgnGameResult switch
        {
            PgnGameResult.Draw => GameResult.Draw,
            PgnGameResult.WhiteWins => GameResult.WhiteWins,
            PgnGameResult.BlackWins => GameResult.BlackWins,
            _ => GameResult.Unknown
        };


    // private void ImportGame(PgnGame pgnGame)
    // {
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
    // }

}