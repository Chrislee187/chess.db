using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace chess.games.db.Migrations
{
    public partial class Create_PgnImportError_Table : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PgnImportErrors",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    PgnGameId = table.Column<Guid>(nullable: false),
                    Error = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PgnImportErrors", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PgnImportErrors");
        }
    }
}
