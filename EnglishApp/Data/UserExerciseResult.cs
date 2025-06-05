using System.ComponentModel.DataAnnotations;

namespace EnglishApp.Data
{
    public class UserExerciseResult
    {
        [Key] 
        public int ResultId { get; set; }
        public int UserId { get; set; }
        public int ExerciseId { get; set; }
        public int SelectedOptionId { get; set; }
        public bool IsCorrect { get; set; }
        public DateTime? AnsweredAt { get; set; }

        public Exercise Exercise { get; set; }
        public ExerciseOption SelectedOption { get; set; }

        public User User { get; set; }
    }
}
