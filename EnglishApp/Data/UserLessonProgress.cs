using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EnglishApp.Data
{
    public class UserLessonProgress
    {
        public int UserId { get; set; }
        public int LessonId { get; set; }
        public string Status { get; set; }
        public float? Score { get; set; }
        public DateTime? LastAccessed { get; set; }

        public Lesson Lesson { get; set; }
        public User User { get; set; }
    }
}
