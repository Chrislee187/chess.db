using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace chess.games.db.Entities
{
    public class Game : DbEntity<Guid>
    {
        public Event Event { get; set; }
        public Site Site { get; set; }
        public Player White { get; set; }
        public Player Black { get; set; }

        [MaxLength(30)]
        public string Date { get; set; }

        public string Round { get; set; }
        public GameResult Result { get; set; }

        public string MoveText { get; set; }

        /// <summary>
        /// Chess Opening code
        /// https://en.wikipedia.org/wiki/Encyclopaedia_of_Chess_Openings
        /// </summary>
        public string Eco { get; set; }

        public int? WhiteElo { get; set; }
        public int? BlackElo { get; set; }

        protected bool Equals(Game other)
        {
            return Equals(Event, other.Event) && Equals(Site, other.Site) && Equals(White, other.White) && Equals(Black, other.Black) && Date == other.Date && Round == other.Round && Result == other.Result && MoveText == other.MoveText && Eco == other.Eco && WhiteElo == other.WhiteElo && BlackElo == other.BlackElo;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Game) obj);
        }

        [SuppressMessage("ReSharper", "NonReadonlyMemberInGetHashCode")]
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (Event != null ? Event.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Site != null ? Site.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (White != null ? White.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Black != null ? Black.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Date != null ? Date.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Round != null ? Round.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (int) Result;
                hashCode = (hashCode * 397) ^ (MoveText != null ? MoveText.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Eco != null ? Eco.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ WhiteElo.GetHashCode();
                hashCode = (hashCode * 397) ^ BlackElo.GetHashCode();
                return hashCode;
            }
        }

        private sealed class GameEqualityComparer : IEqualityComparer<Game>
        {
            public bool Equals(Game x, Game y)
            {
                if (ReferenceEquals(x, y)) return true;
                if (ReferenceEquals(x, null)) return false;
                if (ReferenceEquals(y, null)) return false;
                if (x.GetType() != y.GetType()) return false;
                return Equals(x.Event, y.Event) 
                       && Equals(x.Site, y.Site) 
                       && Equals(x.White, y.White) 
                       && Equals(x.Black, y.Black) 
                       && x.Date == y.Date 
                       && x.Round == y.Round 
                       && x.Result == y.Result 
                       && x.MoveText == y.MoveText 
                       && x.Eco == y.Eco 
                       && x.WhiteElo == y.WhiteElo 
                       && x.BlackElo == y.BlackElo;

            }

            public int GetHashCode(Game obj)
            {
                unchecked
                {
                    var hashCode = (obj.Event != null ? obj.Event.GetHashCode() : 0);
                    hashCode = (hashCode * 397) ^ (obj.Site != null ? obj.Site.GetHashCode() : 0);
                    hashCode = (hashCode * 397) ^ (obj.White != null ? obj.White.GetHashCode() : 0);
                    hashCode = (hashCode * 397) ^ (obj.Black != null ? obj.Black.GetHashCode() : 0);
                    hashCode = (hashCode * 397) ^ (obj.Date != null ? obj.Date.GetHashCode() : 0);
                    hashCode = (hashCode * 397) ^ (obj.Round != null ? obj.Round.GetHashCode() : 0);
                    hashCode = (hashCode * 397) ^ (int) obj.Result;
                    hashCode = (hashCode * 397) ^ (obj.MoveText != null ? obj.MoveText.GetHashCode() : 0);
                    hashCode = (hashCode * 397) ^ (obj.Eco != null ? obj.Eco.GetHashCode() : 0);
                    hashCode = (hashCode * 397) ^ obj.WhiteElo.GetHashCode();
                    hashCode = (hashCode * 397) ^ obj.BlackElo.GetHashCode();
                    return hashCode;
                }
            }
        }

        public static IEqualityComparer<Game> GameComparer { get; } = new GameEqualityComparer();
    }
}