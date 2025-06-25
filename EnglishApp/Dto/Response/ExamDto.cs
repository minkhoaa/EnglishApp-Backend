using EnglishApp.Data;

namespace EnglishApp.Dto.Response;

public class ExamDto
{
    public int ExamId { get; set; }
    public string Title { get; set; } = null!;           // Cambridge 17 Test 1
    public string? Description { get; set; }
    public string? Level { get; set; }                   // General/Academic/...
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public List<ExamSectionDto>? sections {get; set;}
}