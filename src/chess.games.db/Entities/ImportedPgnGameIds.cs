using System;

namespace chess.games.db.Entities
{
    public class ImportedPgnGameIds : DbEntity<Guid>
    {
        public ImportedPgnGameIds(Guid id)
        {
            Id = id;
        }
    }
}