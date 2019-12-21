using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace chess.games.db.Entities
{
    public class PgnImport : DbEntity<Guid>
    {
        // The mandatory seven PGN tags
        [Required]
        public string Event { get; set; }
        [Required]
        public string Site { get; set; }
        [Required]
        public string White { get; set; }
        [Required]
        public string Black { get; set; }
        [Required]
        public string Date { get; set; }
        [Required]
        public string Round { get; set; }
        [Required]
        public string Result { get; set; }

        [Required]
        public string MoveList { get; set; }

        // Very commonly used tags
        public string Eco { get; set; }
        public string WhiteElo { get; set; }
        public string BlackElo { get; set; }

        // Any additional tags are stored as Json
        public string CustomTagsJson { get; set; }

        private sealed class PgnDeDupeQueueEqualityComparer : IEqualityComparer<PgnImport>
        {
            public bool Equals(PgnImport x, PgnImport y)
            {
                if (ReferenceEquals(x, y)) return true;
                if (ReferenceEquals(x, null)) return false;
                if (ReferenceEquals(y, null)) return false;
                if (x.GetType() != y.GetType()) return false;
                return x.Event == y.Event && x.Site == y.Site && x.White == y.White && x.Black == y.Black && x.Date == y.Date && x.Round == y.Round && x.Result == y.Result && x.MoveList == y.MoveList && x.Eco == y.Eco && x.WhiteElo == y.WhiteElo && x.BlackElo == y.BlackElo && x.CustomTagsJson == y.CustomTagsJson;
            }

            public int GetHashCode(PgnImport obj)
            {
                unchecked
                {
                    var hashCode = (obj.Event != null ? obj.Event.GetHashCode() : 0);
                    hashCode = (hashCode * 397) ^ (obj.Site != null ? obj.Site.GetHashCode() : 0);
                    hashCode = (hashCode * 397) ^ (obj.White != null ? obj.White.GetHashCode() : 0);
                    hashCode = (hashCode * 397) ^ (obj.Black != null ? obj.Black.GetHashCode() : 0);
                    hashCode = (hashCode * 397) ^ (obj.Date != null ? obj.Date.GetHashCode() : 0);
                    hashCode = (hashCode * 397) ^ (obj.Round != null ? obj.Round.GetHashCode() : 0);
                    hashCode = (hashCode * 397) ^ (obj.Result != null ? obj.Result.GetHashCode() : 0);
                    hashCode = (hashCode * 397) ^ (obj.MoveList != null ? obj.MoveList.GetHashCode() : 0);
                    hashCode = (hashCode * 397) ^ (obj.Eco != null ? obj.Eco.GetHashCode() : 0);
                    hashCode = (hashCode * 397) ^ (obj.WhiteElo != null ? obj.WhiteElo.GetHashCode() : 0);
                    hashCode = (hashCode * 397) ^ (obj.BlackElo != null ? obj.BlackElo.GetHashCode() : 0);
                    hashCode = (hashCode * 397) ^ (obj.CustomTagsJson != null ? obj.CustomTagsJson.GetHashCode() : 0);
                    return hashCode;
                }
            }
        }

        public static IEqualityComparer<PgnImport> PgnDeDupeQueueComparer { get; } = new PgnDeDupeQueueEqualityComparer();
    }
}