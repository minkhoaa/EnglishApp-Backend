using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EnglishApp.Data;

public class ExamOption
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

    public int OptionId { get; set; }
    public int QuestionId { get; set; }
    public string OptionText { get; set; } = null!;
    public bool IsCorrect { get; set; }
    public int SortOrder { get; set; }
    public ExamQuestion Question { get; set; } = null!; 
    
    public ICollection<UserExamResult> UserExamResults { get; set; } = null!;

}