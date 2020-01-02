using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace chess.games.db.Migrations
{
    public partial class Create_ImportedPgnGameIds_Table : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImportNormalisationComplete",
                table: "PgnGames");

            migrationBuilder.CreateTable(
                name: "ImportedPgnGameIds",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImportedPgnGameIds", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ImportedPgnGameIds");

            migrationBuilder.AddColumn<bool>(
                name: "ImportNormalisationComplete",
                table: "PgnGames",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
