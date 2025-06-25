

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EnglishApp.Data;

public class FlashCard
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int FlashcardId { get; set; }
    public string FrontText { get; set; } = null!;
    public string BackText { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public DateTime? LastReviewedAt { get; set; }
    
    public int DeckId { get; set; }
    public Deck Deck { get; set; } = null!;
    
}