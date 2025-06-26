using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Razor.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System.Security.Policy;
namespace EnglishApp.Data
{
    public class EnglishAppDbContext : IdentityDbContext<
        User,
        Role,
        int,
        UserClaim,
        UserRole,
        UserLogin,
        RoleClaim,
        UserToken
        >
    {
        public EnglishAppDbContext(DbContextOptions<EnglishAppDbContext> options) : base(options) { }
        public DbSet<User> Users { get; set; } 
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UserRoles {  get; set; }
        public DbSet<UserClaim> UserClaims { get; set; }
        public DbSet<UserLogin> UserLogins {  get; set; }
        public DbSet<UserToken> UserTokens {get;set; }
        public DbSet<RoleClaim> RoleClaims {get; set; }
        
        public DbSet<FlashCard> FlashCards {get; set; } 
        public DbSet<Deck> Decks {get; set; }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Lesson> Lessons { get; set; }
        public DbSet<LessonContent> LessonContents { get; set; }
        public DbSet<Exercise> Exercises { get; set; }
        public DbSet<ExerciseOption> ExerciseOptions { get; set; }
        public DbSet<UserLessonProgress> UserLessonProgresses { get; set; }
        public DbSet<UserExerciseResult> UserExerciseResults { get; set; }
        public DbSet<FavoriteDeck>  FavoriteDecks { get; set; } = null!;
        public DbSet<UserInfo> UserInfo { get; set; }

        public DbSet<TempOtp> TempOtps { get; set; }

        public DbSet<ExamCategory>  ExamCategories { get; set; }
        
        public DbSet<Exam> Exams { get; set; }
        public DbSet<ExamSection> ExamSections { get; set; }
        public DbSet<ExamQuestion> ExamQuestions { get; set; }
        public DbSet<ExamOption> ExamOptions { get; set; }
        protected override void OnModelCreating(ModelBuilder builder) 
        {
            base.OnModelCreating(builder);
            builder.Entity<User>().ToTable("users");
            builder.Entity<Role>().ToTable("roles");
            builder.Entity<UserRole>().ToTable("userroles");
            builder.Entity<UserClaim>().ToTable("userclaims");
            builder.Entity<UserLogin>().ToTable("userlogins");
            builder.Entity<UserToken>().ToTable("usertokens");
            builder.Entity<RoleClaim>().ToTable("roleclaims");
            builder.Entity<TempOtp>().ToTable("tempotp");

            builder.Entity<Category>().ToTable("category");
            builder.Entity<Lesson>().ToTable("lesson");
            builder.Entity<LessonContent>().ToTable("lessoncontent");
            builder.Entity<Exercise>().ToTable("exercise");
            builder.Entity<ExerciseOption>().ToTable("exerciseoption");
            builder.Entity<UserLessonProgress>().ToTable("userlessonprogress");
            builder.Entity<UserExerciseResult>().ToTable("userexerciseresult");
            builder.Entity<UserInfo>().ToTable("userinfo");
            builder.Entity<FlashCard>().ToTable("flashcards");
            builder.Entity<Deck>().ToTable("decks");
            builder.Entity<Exam>().ToTable("exams");
            builder.Entity<ExamSection>().ToTable("examsections");
            builder.Entity<ExamQuestion>().ToTable("examquestions");
            builder.Entity<ExamOption>().ToTable("examoptions");
            
            builder.Entity<ExamCategory>().ToTable("examcategory");
            builder.Entity<Exam>()
                .HasOne(x => x.Category)
                .WithMany(x => x.Exams)
                .HasForeignKey(x => x.ExamCategoryId);
            
            builder.Entity<ExamSection>()
                .HasOne(x => x.Exam)
                .WithMany(x => x.Sections)
                .HasForeignKey(x => x.ExamId);

            builder.Entity<ExamQuestion>()
                .HasOne(x => x.Section)
                .WithMany(x => x.Questions)
                .HasForeignKey(x => x.SectionId);

            builder.Entity<ExamOption>()
                .HasOne(x => x.Question)
                .WithMany(x => x.Options)
                .HasForeignKey(x => x.QuestionId);


            builder.Entity<FlashCard>().HasKey(x=>x.FlashcardId);
            builder.Entity<Deck>().HasKey(x=>x.Id);
            builder.Entity<FlashCard>()
                .HasOne(x=>x.Deck)
                .WithMany(x=>x.FlashCards)
                .HasForeignKey(x=>x.DeckId);
            
            
            
            builder.Entity<FavoriteDeck>().ToTable("favoritedecks");
            builder.Entity<FavoriteDeck>().HasKey(x => new
            {
                x.UserId, x.DeckId
            });
            builder.Entity<FavoriteDeck>()
                .HasOne(x=>x.User)
                .WithMany(x=>x.FavoriteDecks)
                .HasForeignKey(x=>x.UserId);
            
            builder.Entity<Deck>()
                .HasOne(x=>x.Owner)
                .WithMany(x=>x.Decks)
                .HasForeignKey(x=>x.OwnerId);
            
            builder.Entity<FavoriteDeck>()
                .HasOne(x=>x.Deck)
                .WithMany(x=>x.FavoriteDecks)
                .HasForeignKey(x=>x.DeckId);
            
            builder.Entity<UserLessonProgress>()
                .HasKey(x => new { x.LessonId, x.UserId});

            builder.Entity<TempOtp>()
                .HasKey(x => x.Email);

            builder.Entity<UserRole>()
                .HasKey(x=> new {x.UserId, x.RoleId});
            builder.Entity<UserRole>()
                .HasOne(x=>x.User)
                .WithMany(a=>a.UserRoles)
                .HasForeignKey(x=>x.UserId)
                .OnDelete(DeleteBehavior.Cascade);


            builder.Entity<UserRole>()
                .HasOne(x=>x.Role)
                .WithMany(a=>a.UserRoles)
                .HasForeignKey(x=>x.RoleId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Lesson>()
                .HasOne(x => x.Category)
                .WithMany(x => x.Lessons)
                .HasForeignKey(x => x.CategoryId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<LessonContent>()
                .HasOne(x => x.Lesson)
                .WithMany(x => x.LessonContents)
                .HasForeignKey(x => x.LessonId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Exercise>()
                .HasOne(x => x.Lesson)
                .WithMany(x => x.Exercises)
                .HasForeignKey(x => x.LessonId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.Entity<UserLessonProgress>()
                .HasOne(x => x.Lesson)
                .WithMany(x => x.UserLessonProgresses)
                .HasForeignKey(x => x.LessonId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.Entity<UserExerciseResult>()
                .HasOne(x=>x.Exercise)
                .WithMany(x=>x.UserExerciseResults)
                .HasForeignKey(x=>x.ExerciseId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.Entity<ExerciseOption>()
                .HasOne(x => x.Exercise)
                .WithMany(x => x.ExerciseOptions)
                .HasForeignKey(x => x.ExerciseId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<UserExerciseResult>()
                .HasOne(x=>x.SelectedOption)
                .WithMany(x=>x.UserExerciseResults)
                .HasForeignKey(x=>x.SelectedOptionId)
                .OnDelete(DeleteBehavior.Cascade);


            builder.Entity<UserExerciseResult>()
                .HasOne(x=>x.User)
                .WithMany(x=>x.ExerciseResults)
                .HasForeignKey(x=>x.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<UserInfo>()
                .HasOne(x=>x.User)
                .WithOne(x=>x.UserInfo)
                .HasForeignKey<UserInfo>(x=>x.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
