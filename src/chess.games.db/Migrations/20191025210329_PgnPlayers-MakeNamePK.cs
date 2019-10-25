using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace chess.games.db.Migrations
{
    public partial class PgnPlayersMakeNamePK : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GameImports_Players_BlackId",
                table: "GameImports");

            migrationBuilder.DropForeignKey(
                name: "FK_GameImports_Players_WhiteId",
                table: "GameImports");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PgnPlayers",
                table: "PgnPlayers");

            migrationBuilder.DropIndex(
                name: "IX_GameImports_BlackId",
                table: "GameImports");

            migrationBuilder.DropIndex(
                name: "IX_GameImports_WhiteId",
                table: "GameImports");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "PgnPlayers");

            migrationBuilder.DropColumn(
                name: "BlackId",
                table: "GameImports");

            migrationBuilder.DropColumn(
                name: "WhiteId",
                table: "GameImports");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "PgnPlayers",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BlackName",
                table: "GameImports",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WhiteName",
                table: "GameImports",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_PgnPlayers",
                table: "PgnPlayers",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_GameImports_BlackName",
                table: "GameImports",
                column: "BlackName");

            migrationBuilder.CreateIndex(
                name: "IX_GameImports_WhiteName",
                table: "GameImports",
                column: "WhiteName");

            migrationBuilder.AddForeignKey(
                name: "FK_GameImports_PgnPlayers_BlackName",
                table: "GameImports",
                column: "BlackName",
                principalTable: "PgnPlayers",
                principalColumn: "Name",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_GameImports_PgnPlayers_WhiteName",
                table: "GameImports",
                column: "WhiteName",
                principalTable: "PgnPlayers",
                principalColumn: "Name",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GameImports_PgnPlayers_BlackName",
                table: "GameImports");

            migrationBuilder.DropForeignKey(
                name: "FK_GameImports_PgnPlayers_WhiteName",
                table: "GameImports");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PgnPlayers",
                table: "PgnPlayers");

            migrationBuilder.DropIndex(
                name: "IX_GameImports_BlackName",
                table: "GameImports");

            migrationBuilder.DropIndex(
                name: "IX_GameImports_WhiteName",
                table: "GameImports");

            migrationBuilder.DropColumn(
                name: "BlackName",
                table: "GameImports");

            migrationBuilder.DropColumn(
                name: "WhiteName",
                table: "GameImports");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "PgnPlayers",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "PgnPlayers",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "BlackId",
                table: "GameImports",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "WhiteId",
                table: "GameImports",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_PgnPlayers",
                table: "PgnPlayers",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_GameImports_BlackId",
                table: "GameImports",
                column: "BlackId");

            migrationBuilder.CreateIndex(
                name: "IX_GameImports_WhiteId",
                table: "GameImports",
                column: "WhiteId");

            migrationBuilder.AddForeignKey(
                name: "FK_GameImports_Players_BlackId",
                table: "GameImports",
                column: "BlackId",
                principalTable: "PgnPlayers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_GameImports_Players_WhiteId",
                table: "GameImports",
                column: "WhiteId",
                principalTable: "PgnPlayers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
