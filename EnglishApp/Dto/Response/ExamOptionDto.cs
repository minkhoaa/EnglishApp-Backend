namespace EnglishApp.Dto.Response;

public class ExamOptionDto
{
    public int OptionId { get; set; }
    public int QuestionId { get; set; }
    public string OptionText { get; set; } = null!;
    public bool IsCorrect { get; set; }
    public int SortOrder { get; set; }
}