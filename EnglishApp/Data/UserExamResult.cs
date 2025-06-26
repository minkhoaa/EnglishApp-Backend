using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EnglishApp.Data;

public class UserExamResult
{
    [Key]
    [DatabaseGenerated(databaseGeneratedOption: DatabaseGeneratedOption.Identity)]
    public int UserExamResultId { get; set; }
    public int UserId { get; set; }
    public int ExamId { get; set; }
    public int? SectionId { get; set; }
    public int QuestionId { get; set; }
    public int? AnswerOptionId { get; set; }    // Đáp án chọn, null nếu tự luận
    public string? AnswerText { get; set; }     // Đáp án điền/chữ
    public string? AnswerJson { get; set; }     // Dạng JSON cho matching, multi-select
    public bool? IsCorrect { get; set; }
    public DateTime? AnsweredAt { get; set; }
    public bool IsSubmitted { get; set; } = false;

    // Navigation properties (tùy bạn có muốn dùng không)
    public User User { get; set; }
    public Exam Exam { get; set; }
    public ExamSection Section { get; set; }
    public ExamQuestion Question { get; set; }
    public ExamOption AnswerOption { get; set; }
}