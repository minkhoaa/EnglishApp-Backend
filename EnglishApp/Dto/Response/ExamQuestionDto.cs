using EnglishApp.Data;

namespace EnglishApp.Dto.Response;

public class ExamQuestionDto
{
    public int QuestionId { get; set; }
    public int SectionId { get; set; }
    public string QuestionText { get; set; } = null!;
    public string Type { get; set; } = null!;           // 'fill-in-blank', 'multiple-choice'...
    public int SortOrder { get; set; }
    
    public string? CorrectAnswer { get; set; }   
    public List<ExamOptionDto>? options {get; set;}
}