using System;
using System.Collections.Generic;
using chess.games.db.Entities;

namespace chess.games.db.api.Services
{
    public interface IPgnImportService
    {
        event Action<string> Status;
        void ImportGames(string[] pgnFiles);
    }
}