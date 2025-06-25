
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EnglishApp.Data;

public class Exam
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

    public int ExamId { get; set; }
    public string Title { get; set; } = null!;           // Cambridge 17 Test 1
    public string? Description { get; set; }
    public string? Level { get; set; }                   // General/Academic/...
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public ICollection<ExamSection>? Sections { get; set; }
}