using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace chess.games.db.Migrations
{
    public partial class PgnImportQueueCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PgnImportQueue",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Event = table.Column<string>(nullable: false),
                    Site = table.Column<string>(nullable: false),
                    White = table.Column<string>(nullable: false),
                    Black = table.Column<string>(nullable: false),
                    Date = table.Column<string>(nullable: false),
                    Round = table.Column<string>(nullable: false),
                    Result = table.Column<string>(nullable: false),
                    MoveList = table.Column<string>(nullable: false),
                    Eco = table.Column<string>(nullable: true),
                    WhiteElo = table.Column<string>(nullable: true),
                    BlackElo = table.Column<string>(nullable: true),
                    CustomTagsJson = table.Column<string>(nullable: true),
                    ImportNormalisationComplete = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PgnImportQueue", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PgnImportQueue");
        }
    }
}
