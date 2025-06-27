namespace EnglishApp.Dto.Response;

public class DetailedUserExamResult
{
    public string Transript {get; set;} 
    public List<DetailedResult> DetailedResults { get; set; } 
    
}

public class DetailedResult
{
    public int UserAnswerId { get; set; }
    public string? UserAnswer  {get; set;}
    public bool? IsCorrect {get; set;} 
    
    
    public int? CorrectOptionId {get; set;}
    public string? CorrectAnswer {get; set;}

}