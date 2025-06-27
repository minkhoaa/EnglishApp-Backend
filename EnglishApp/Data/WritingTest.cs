using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EnglishApp.Data;

public class WritingTest
{
    [Key]
    [DatabaseGenerated(databaseGeneratedOption: DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    
    public WritingExamContent ExamContent { get; set; }
    
}

public class WritingExamContent
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public int ExamId { get; set; }
    public string Question { get; set; }
    public string SampleAnswer { get; set; }
    public WritingTest Tests { get; set; } 
}