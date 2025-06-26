namespace EnglishApp.Dto.Response;

public class UserExamAnalyse
{
   
        public int SectionId { get; set; }
        public int TotalQuestions { get; set; }
        public int CorrectAnswers { get; set; }
        public int IncorrectAnswers { get; set; }
        public double PercentCorrect { get; set; }
    
        public List<QuestionTypeStats> TypeStats { get; set; }
    }

    public class QuestionTypeStats
    {
        public string Type { get; set; }
        public int Total { get; set; }
        public int Correct { get; set; }
        public int Incorrect { get; set; }
    }
