using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace EnglishApp.Migrations
{
    /// <inheritdoc />
    public partial class updatedbver11 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "userexamresults",
                columns: table => new
                {
                    UserExamResultId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    ExamId = table.Column<int>(type: "integer", nullable: false),
                    SectionId = table.Column<int>(type: "integer", nullable: true),
                    QuestionId = table.Column<int>(type: "integer", nullable: false),
                    AnswerOptionId = table.Column<int>(type: "integer", nullable: true),
                    AnswerText = table.Column<string>(type: "text", nullable: true),
                    AnswerJson = table.Column<string>(type: "text", nullable: true),
                    IsCorrect = table.Column<bool>(type: "boolean", nullable: true),
                    AnsweredAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsSubmitted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_userexamresults", x => x.UserExamResultId);
                    table.ForeignKey(
                        name: "FK_userexamresults_examoptions_AnswerOptionId",
                        column: x => x.AnswerOptionId,
                        principalTable: "examoptions",
                        principalColumn: "OptionId");
                    table.ForeignKey(
                        name: "FK_userexamresults_examquestions_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "examquestions",
                        principalColumn: "QuestionId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_userexamresults_exams_ExamId",
                        column: x => x.ExamId,
                        principalTable: "exams",
                        principalColumn: "ExamId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_userexamresults_examsections_SectionId",
                        column: x => x.SectionId,
                        principalTable: "examsections",
                        principalColumn: "SectionId");
                    table.ForeignKey(
                        name: "FK_userexamresults_users_UserId",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_userexamresults_AnswerOptionId",
                table: "userexamresults",
                column: "AnswerOptionId");

            migrationBuilder.CreateIndex(
                name: "IX_userexamresults_ExamId",
                table: "userexamresults",
                column: "ExamId");

            migrationBuilder.CreateIndex(
                name: "IX_userexamresults_QuestionId",
                table: "userexamresults",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_userexamresults_SectionId",
                table: "userexamresults",
                column: "SectionId");

            migrationBuilder.CreateIndex(
                name: "IX_userexamresults_UserId",
                table: "userexamresults",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "userexamresults");
        }
    }
}
