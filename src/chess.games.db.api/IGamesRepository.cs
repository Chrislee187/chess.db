using PgnReader;

namespace chess.games.db.api
{
    public interface IGamesRepository
    {
        long TotalGames { get; }
        int AddImportBatch(PgnGame[] games);
    }
}