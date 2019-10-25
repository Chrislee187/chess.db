using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace chess.games.db.Migrations
{
    public partial class PgnPlayerAddReferenceToPlayer : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "PlayerId",
                table: "PgnPlayers",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PgnPlayers_PlayerId",
                table: "PgnPlayers",
                column: "PlayerId");

            migrationBuilder.AddForeignKey(
                name: "FK_PgnPlayers_Players_PlayerId",
                table: "PgnPlayers",
                column: "PlayerId",
                principalTable: "Players",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PgnPlayers_Players_PlayerId",
                table: "PgnPlayers");

            migrationBuilder.DropIndex(
                name: "IX_PgnPlayers_PlayerId",
                table: "PgnPlayers");

            migrationBuilder.DropColumn(
                name: "PlayerId",
                table: "PgnPlayers");
        }
    }
}
