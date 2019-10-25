using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace chess.games.db.Migrations
{
    public partial class GamesRemoveGuidPgnPlayerReferences : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Games_Players_BlackId",
                table: "Games");

            migrationBuilder.DropForeignKey(
                name: "FK_Games_Players_WhiteId",
                table: "Games");

            migrationBuilder.DropIndex(
                name: "IX_Games_BlackId",
                table: "Games");

            migrationBuilder.DropIndex(
                name: "IX_Games_WhiteId",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "BlackId",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "WhiteId",
                table: "Games");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "BlackId",
                table: "Games",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "WhiteId",
                table: "Games",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Games_BlackId",
                table: "Games",
                column: "BlackId");

            migrationBuilder.CreateIndex(
                name: "IX_Games_WhiteId",
                table: "Games",
                column: "WhiteId");

            migrationBuilder.AddForeignKey(
                name: "FK_Games_Players_BlackId",
                table: "Games",
                column: "BlackId",
                principalTable: "PgnPlayers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Games_Players_WhiteId",
                table: "Games",
                column: "WhiteId",
                principalTable: "PgnPlayers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
