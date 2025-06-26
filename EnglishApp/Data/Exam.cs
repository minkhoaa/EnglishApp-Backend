
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace EnglishApp.Data;

public class Exam
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

    public int ExamId { get; set; }
    public string Title { get; set; } = null!;           // Cambridge 17 Test 1
    public string? Description { get; set; }
    
    public string? Image { get; set; }
    public string? Level { get; set; }                   // General/Academic/...
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public int ExamCategoryId { get; set; }
  
    public ExamCategory? Category { get; set; }
    public ICollection<ExamSection>? Sections { get; set; } 
}