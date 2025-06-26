using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EnglishApp.Data;

public class ExamCategory
{
    [Key]
    [DatabaseGenerated(databaseGeneratedOption:DatabaseGeneratedOption.Identity)]
    public int ExamCategoryId { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? Level { get; set; }
    public ICollection<Exam>? Exams { get; set; }
}