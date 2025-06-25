using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EnglishApp.Migrations
{
    /// <inheritdoc />
    public partial class updatedbver5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "decks",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "decks");
        }
    }
}
