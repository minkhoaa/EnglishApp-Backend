using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace EnglishApp.Migrations
{
    /// <inheritdoc />
    public partial class addExamcategory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ExamCategoryId",
                table: "exams",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "examcategory",
                columns: table => new
                {
                    ExamCategoryId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Level = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_examcategory", x => x.ExamCategoryId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_exams_ExamCategoryId",
                table: "exams",
                column: "ExamCategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_exams_examcategory_ExamCategoryId",
                table: "exams",
                column: "ExamCategoryId",
                principalTable: "examcategory",
                principalColumn: "ExamCategoryId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_exams_examcategory_ExamCategoryId",
                table: "exams");

            migrationBuilder.DropTable(
                name: "examcategory");

            migrationBuilder.DropIndex(
                name: "IX_exams_ExamCategoryId",
                table: "exams");

            migrationBuilder.DropColumn(
                name: "ExamCategoryId",
                table: "exams");
        }
    }
}
