using Microsoft.EntityFrameworkCore;

namespace chess.games.db
{
    public static class DbContextOptionsBuilderExtensions
    {
        public static DbContextOptionsBuilder UseSqlOrSqlLite(this DbContextOptionsBuilder builder,
            string connectionString)
        {
            return connectionString.Contains("sqlite")
                ? builder.UseSqlite(connectionString)
                : builder.UseSqlServer(connectionString);
        }
    }
}