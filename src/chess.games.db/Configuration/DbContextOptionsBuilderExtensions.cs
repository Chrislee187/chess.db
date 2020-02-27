using Microsoft.EntityFrameworkCore;

namespace chess.games.db.Configuration
{
    public static class DbContextOptionsBuilderExtensions
    {
        public static DbContextOptionsBuilder UseSql(this DbContextOptionsBuilder builder,
            DbServerTypes serverType,
            string connectionString)
        {
            return serverType == DbServerTypes.Sqlite
                ? builder.UseSqlite(connectionString)
                : builder.UseSqlServer(connectionString);
        }
    }
}