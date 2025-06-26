using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EnglishApp.Data;

public class ExamQuestion
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

    public int QuestionId { get; set; }
    public int SectionId { get; set; }
    public string QuestionText { get; set; } = null!;
    public string Type { get; set; } = null!; // 'fill-in-blank', 'multiple-choice'...
    
    public string? SampleImage { get; set; } = null!;
    public int SortOrder { get; set; }
    public ExamSection Section { get; set; } = null!;
    public ICollection<ExamOption>? Options { get; set; }
    public string? CorrectAnswer { get; set; }          
    
    public ICollection<UserExamResult> UserExamResults { get; set; } = null!;

}