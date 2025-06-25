using EnglishApp.Data;

namespace EnglishApp.Dto.Response;

public class ExamSectionDto
{
    public int SectionId { get; set; }
    public int ExamId { get; set; }
    public string Name { get; set; } = null!;           // Section 1, Section 2,...
    public string? AudioUrl { get; set; }               // Dùng cho Listening
    public string? Transcript { get; set; }             // Public sau khi làm xong
    public int SortOrder { get; set; }
    public List<ExamQuestionDto>? questions {get; set;}
}