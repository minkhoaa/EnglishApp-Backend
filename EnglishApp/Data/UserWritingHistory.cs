using System.ComponentModel.DataAnnotations;

namespace EnglishApp.Data;

public class UserWritingHistory
{
    [Key]
    public int Id { get; set; }
    public int UserId { get; set; }
    public int ExamId { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string UserAnswer { get; set; } // Bài làm của user
    public string AiFeedback { get; set; } // Nhận xét, đánh giá từ AI (có thể lưu dạng JSON hoặc plain text)
  
    public DateTime SubmittedAt { get; set; }
    
    public User User { get; set; }
}
