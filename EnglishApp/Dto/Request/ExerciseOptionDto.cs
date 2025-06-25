namespace EnglishApp.Dto.Request;

public class ExerciseOptionDto
{
    public int OptionId { get; set; }
    public int ExerciseId { get; set; }
    public string OptionText { get; set; }
    public bool IsCorrect { get; set; }
    public int SortOrder { get; set; }
}