using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EnglishApp.Migrations
{
    /// <inheritdoc />
    public partial class updatedbver6 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "OwnerId",
                table: "decks",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_decks_OwnerId",
                table: "decks",
                column: "OwnerId");

            migrationBuilder.AddForeignKey(
                name: "FK_decks_users_OwnerId",
                table: "decks",
                column: "OwnerId",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_decks_users_OwnerId",
                table: "decks");

            migrationBuilder.DropIndex(
                name: "IX_decks_OwnerId",
                table: "decks");

            migrationBuilder.DropColumn(
                name: "OwnerId",
                table: "decks");
        }
    }
}
