namespace EnglishApp.Dto.Request;

public class UserExamResultDto
{
    public int UserId { get; set; }
    public int ExamId { get; set; }
    public int? SectionId { get; set; }
    public int QuestionId { get; set; }
    public int? AnswerOptionId { get; set; }
    public string? AnswerText { get; set; }
    public string? AnswerJson { get; set; }
    public bool IsSubmitted { get; set; }
    public DateTime? AnsweredAt { get; set; }
}