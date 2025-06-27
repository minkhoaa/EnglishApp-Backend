using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EnglishApp.Migrations
{
    /// <inheritdoc />
    public partial class dbv99 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "userwritinghistory",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "userwritinghistory",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "userwritinghistory");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "userwritinghistory");
        }
    }
}
