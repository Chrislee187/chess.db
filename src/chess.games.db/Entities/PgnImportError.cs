using System;

namespace chess.games.db.Entities
{
    public class PgnImportError : DbEntity<Guid>
    {
        public Guid PgnGameId { get; set; }

        public string Error { get; set; }

        public PgnImportError()
        {
            
        }
        public PgnImportError(Guid pgnGameId, string error)
        {
            PgnGameId = pgnGameId;
            Error = error;
        }
    }
}