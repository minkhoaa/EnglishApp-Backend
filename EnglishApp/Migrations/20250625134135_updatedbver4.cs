using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EnglishApp.Migrations
{
    /// <inheritdoc />
    public partial class updatedbver4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "favoritedecks",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    DeckId = table.Column<int>(type: "integer", nullable: false),
                    CreateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_favoritedecks", x => new { x.UserId, x.DeckId });
                    table.ForeignKey(
                        name: "FK_favoritedecks_decks_DeckId",
                        column: x => x.DeckId,
                        principalTable: "decks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_favoritedecks_users_UserId",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_favoritedecks_DeckId",
                table: "favoritedecks",
                column: "DeckId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "favoritedecks");
        }
    }
}
