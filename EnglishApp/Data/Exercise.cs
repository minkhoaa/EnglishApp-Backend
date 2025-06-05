using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EnglishApp.Data
{
    public class Exercise
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]  
        public int ExerciseId { get; set; }
        public int LessonId { get; set; }   
        public string Type { get; set; }

        public string Question { get; set; }
        public string Explanation { get; set; }

        public int SortOrder { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastUpdatedAt { get; set; }
        public Lesson Lesson { get; set; }
        public ICollection<ExerciseOption> ExerciseOptions { get; set; }
        public ICollection<UserExerciseResult> UserExerciseResults { get; set; }
    }
}
