using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EnglishApp.Migrations
{
    /// <inheritdoc />
    public partial class updatedbVer3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Image",
                table: "category",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Image",
                table: "category");
        }
    }
}
