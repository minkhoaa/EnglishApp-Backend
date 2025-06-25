namespace EnglishApp.Dto.Response;

public class FlashCardResponse
{
    public int FlashcardId { get; set; }
    public string FrontText { get; set; } = null!;
    public string BackText { get; set; } = null!;
}