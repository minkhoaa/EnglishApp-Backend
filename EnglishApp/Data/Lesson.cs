using System.CodeDom.Compiler;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EnglishApp.Data
{
    public class Lesson
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int LessonId { get; set; }

        public string Title { get; set; } = string.Empty;
        public string Descripton { get; set; } = string.Empty;

        public string Level { get; set; }

        public int CategoryId { get; set; }

        public string MediaUrl { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }

        public DateTime LastUpdatedAt { get; set; }
        public Category Category { get; set; }
        public ICollection<LessonContent> LessonContents { get; set; }
        public ICollection<Exercise> Exercises { get; set; }
        public ICollection<UserLessonProgress> UserLessonProgresses { get; set; }
    }


}
