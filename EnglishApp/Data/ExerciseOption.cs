using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.Eventing.Reader;

namespace EnglishApp.Data
{
    public class ExerciseOption
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int OptionId { get; set; }
        public int ExerciseId { get; set; }
        public string OptionText { get; set; }
        public bool IsCorrect { get; set; }
        public int SortOrder { get; set; }
        public Exercise Exercise { get; set; }
        public ICollection<UserExerciseResult> UserExerciseResults { get; set; }
    }
}
