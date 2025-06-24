namespace EnglishApp.Dto.Request;

public class ExerciseDto
{
    public int LessonId { get; set; }   
    public string Type { get; set; }

    public string Question { get; set; }
    public string Explanation { get; set; }

    public int SortOrder { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime LastUpdatedAt { get; set; }
}