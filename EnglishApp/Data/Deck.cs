using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using EnglishApp.Dto.Request;

namespace EnglishApp.Data;

public class Deck
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public string Name { get; set; }
    public int FlashCardNumber { get; set; }
    
    public ICollection<FlashCard>  FlashCards { get; set; }
}