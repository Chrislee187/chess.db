using Chess.Games.Data.Entities;
using EasyEF.Repos;

namespace Chess.Games.Data.Repos;

public class GameRepo : EasyEfRepositoryBase<GameEntity>, IGameRepository
{
    public GameRepo(ChessGamesDbContext dbContext) : base(dbContext)
    {
    }

    public bool Exists(string eventText, string siteText, string whitePlayer, string blackPlayer, int round)
    {
        return FirstOrDefault(g => g.SourceEventText == eventText
                   && g.SourceSiteText == siteText
                   && g.SourceWhitePlayerText == whitePlayer
                   && g.SourceBlackPlayerText == blackPlayer
                   && g.Round == round
        ) != null;
    }

}
public interface IGameRepository : ILinqRepository<GameEntity>
{
    bool Exists(string eventText, string siteText, string whitePlayer, string blackPlayer, int round);
}