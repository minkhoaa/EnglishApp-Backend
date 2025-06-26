using EnglishApp.Data;

namespace EnglishApp.Dto.Request;

public class CreateExamDto
{
    public string Title { get; set; }
    public string? Description { get; set; }
    public string? Level { get; set; }
    public string? Image { get; set; }
    
    public int CategoryId { get; set; } 
  
    public List<CreateExamSectionDto> Sections { get; set; }
}

public class CreateExamSectionDto
{
    public string Name { get; set; }
    public string? AudioUrl { get; set; }
    public string? Transcript { get; set; }
    public int SortOrder { get; set; }
    public List<CreateExamQuestionDto> Questions { get; set; }
}

public class CreateExamQuestionDto
{
    public string QuestionText { get; set; }
    public string Type { get; set; }
    public string? SampleImage { get; set; } = "";
    public int SortOrder { get; set; }
    public string? CorrectAnswer { get; set; }
    public List<CreateExamOptionDto> Options { get; set; }
}

public class CreateExamOptionDto
{
    public string OptionText { get; set; }
    public bool IsCorrect { get; set; }
    public int SortOrder { get; set; }
}
