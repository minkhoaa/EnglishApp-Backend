using EnglishApp.Dto.Request;

namespace EnglishApp.Dto.Response;

public class ExerciseAndFullOption
{
    public ExerciseDto Exercise { get; set; }
    public List<ExerciseOptionDto> ExerciseOptionList { get; set; }
}