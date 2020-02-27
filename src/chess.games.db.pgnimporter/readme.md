# PGN Importer

This little tool will import any number of PGN games from PGN files recursively found under a specified folder.

The games will be imported to the database specified in the `appsettings.json`, `DbServerType` & `ChessDB` properties.

See [chess.game.db](https://github.com/Chrislee187/chess.db/tree/master/src/chess.games.db) readme for more inforation on the DB configuration settings.
## Example Usage
Build with `dotnet build` and run the compiled `chess.games.db.exe`, or use 
`dotnet run .\chess.games.db.pgnimporter -- .\SampleFiles`

You should see output similar to;
```
Connecting to SQLite chess database...
  Checking for pending migrations...
  No pending migrations
Starting import from: .\SampleFiles...
[14:24:54 INF] .\SampleFiles
Beginning import of 1 PGN files at: 27/02/2020 14:24:54
File                                                    #      +   dupe elpsed  p/sec
(1/1) sample.pgn                                      169    169      0  525ms    169

1 total files processed, 169 games created per second.
Initialising validation process...

218 players left to analyse.
169 total games left to analyse...

Validating    10 game(s) for (1/218) 'Barendregt, Johan Teunis'         +0=10?0 - 84ms avg. per game
Validating     9 game(s) for (2/218) 'Ujtelky, Maximilian'      +0=9?0 - 73ms avg. per game
Validating     9 game(s) for (3/218) 'Robatsch, Karl'   +0=9?0 - 76ms avg. per game
Validating     6 game(s) for (4/218) 'Botvinnik, Mikhail'       +0=6?0 - 77ms avg. per game
...
```

5