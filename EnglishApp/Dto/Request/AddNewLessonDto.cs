namespace EnglishApp.Dto.Request
{
    public class AddNewLessonDto
    {
        public string Title { get; set; } = string.Empty;
        public string Descripton { get; set; } = string.Empty;

        public string Level { get; set; } = string.Empty;

        public int CategoryId { get; set; }

        public string MediaUrl { get; set; }

    
    }
}
