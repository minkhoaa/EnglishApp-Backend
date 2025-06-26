using Microsoft.AspNetCore.Identity;

namespace EnglishApp.Data
{
    public class User : IdentityUser<int>
    {
        
        
        public ICollection<UserRole> UserRoles { get; set; } = null!;

        public ICollection<UserExerciseResult> ExerciseResults { get; set; }
        public ICollection<UserLessonProgress> LessonProgresses { get; set; }

        public UserInfo UserInfo { get; set; }
        public ICollection<FavoriteDeck>  FavoriteDecks { get; set; } = null!;
        
        public ICollection<Deck>  Decks { get; set; } = null!;
        
        public ICollection<UserExamResult> UserExamResults { get; set; } = null!;
    }
}
