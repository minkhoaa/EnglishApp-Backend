using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EnglishApp.Data;

public class ExamSection
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int SectionId { get; set; }
    public int ExamId { get; set; }
    public string Name { get; set; } = null!;           // Section 1, Section 2,...
    public string? AudioUrl { get; set; }               // Dùng cho Listening
    public string? Transcript { get; set; }             // Public sau khi làm xong
    public int SortOrder { get; set; }
    public Exam Exam { get; set; } = null!;
    public ICollection<ExamQuestion>? Questions { get; set; }
    public ICollection<UserExamResult> UserExamResults { get; set; } = null!;

}