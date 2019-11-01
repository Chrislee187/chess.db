using Microsoft.EntityFrameworkCore.Migrations;

namespace chess.games.db.Migrations
{
    public partial class GameAddPgnPlayerBlackAndWhiteColumns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PgnPlayerBlack",
                table: "Games",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PgnPlayerWhite",
                table: "Games",
                nullable: true);

            migrationBuilder.Sql($"UPDATE Games " +
                                 $"SetId PgnPlayerWhite=(SELECT Name from PgnPlayers where Id = WhiteId) "
                                 + @"WHERE PgnPlayerWhite is NULL "
            );
            migrationBuilder.Sql($"UPDATE Games " +
                                 $"SetId PgnPlayerBlack=(SELECT Name from PgnPlayers where Id = BlackId) "
                                 + @"WHERE PgnPlayerBlack is NULL "
            );
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PgnPlayerBlack",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "PgnPlayerWhite",
                table: "Games");
        }
    }
}
