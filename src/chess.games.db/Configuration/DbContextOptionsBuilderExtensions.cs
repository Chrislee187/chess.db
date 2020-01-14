using Microsoft.EntityFrameworkCore;

namespace chess.games.db.Configuration
{
    public static class DbContextOptionsBuilderExtensions
    {
        public static DbContextOptionsBuilder UseSql(this DbContextOptionsBuilder builder,
            ConfigurationExtensions.DbServerTypes serverType,
            string connectionString)
        {
            return serverType == ConfigurationExtensions.DbServerTypes.SQLite
                ? builder.UseSqlite(connectionString)
                : builder.UseSqlServer(connectionString);
        }
    }
}