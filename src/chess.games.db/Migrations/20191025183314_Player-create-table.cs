using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace chess.games.db.Migrations
{
    public partial class PlayerCreateTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameIndex("PK_Players", "PK_PgnPlayers", "PgnPlayers");


            migrationBuilder.CreateTable(
                name: "Players",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Firstname = table.Column<string>(nullable: true),
                    Surname = table.Column<string>(nullable: true),
                    Middlenames = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Players", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Players");
        }
    }
}
