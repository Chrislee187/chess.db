using chess.games.db.Entities;

namespace chess.games.db.Extensions
{
    public static class EntityExtensions
    {
        public static void LoadChildren<TEntity>(this ChessGamesDbContext context, TEntity entity, string propName)
            where TEntity : class
        {
            context.Entry(entity)
                .Reference(propName)
                .Load();
        }
    }
}