using System;

namespace chess.games.db.api.Services
{
    public interface IPgnImportService
    {
        event Action<string> Status;
        void ImportGames(string[] pgnFiles);
        void ProcessUnvalidatedGames();
    }
}