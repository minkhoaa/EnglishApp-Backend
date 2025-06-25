using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EnglishApp.Data;
using EnglishApp.Dto.Request;
using EnglishApp.Dto.Response;
using EnglishApp.Repository;
using Microsoft.EntityFrameworkCore;

namespace EnglishApp.Service
{
    public interface IExerciseOptionService
    {
        Task<List<ExerciseOption>> GetByExerciseIdAsync(int exerciseId);
        Task<ExerciseOption?> GetByIdAsync(int id);
        Task<ExerciseOption> AddAsync(ExerciseOption o);
        Task UpdateAsync(ExerciseOption o);
        Task DeleteAsync(int id);
        
        //Extend
        Task<List<ExerciseAndFullOption>> GetAllExerciseAndFullOptionByLessonAsync(int lessonId);
    }
    public class ExerciseOptionService : IExerciseOptionService
    {
        private readonly IExerciseOptionRepository _repo;
        private readonly EnglishAppDbContext _context;
        private readonly IExerciseRepository _exerciseRepository;
        public ExerciseOptionService(IExerciseOptionRepository repo, EnglishAppDbContext context,  IExerciseRepository exerciseRepository)
        {
            _repo = repo; 
            _context = context;
            _exerciseRepository = exerciseRepository;
        }


        public Task<List<ExerciseOption>> GetByExerciseIdAsync(int exerciseId) => _repo.GetByExerciseIdAsync(exerciseId);
        public Task<ExerciseOption?> GetByIdAsync(int id) => _repo.GetByIdAsync(id);
        public Task<ExerciseOption> AddAsync(ExerciseOption o) => _repo.AddAsync(o);
        public Task UpdateAsync(ExerciseOption o) => _repo.UpdateAsync(o);
        public Task DeleteAsync(int id) => _repo.DeleteAsync(id);
       

        //Extend Service
         public async Task<List<ExerciseAndFullOption>> GetAllExerciseAndFullOptionByLessonAsync(int  lessonId)
         {
             var result = await _context.Exercises
                 .AsNoTracking()
                 .Where(x => x.LessonId == lessonId)
                 .Select(x => new ExerciseAndFullOption
                 {
                     Exercise = new ExerciseDto
                     {
                         LessonId = x.LessonId, CreatedAt = x.CreatedAt,
                         Explanation = x.Explanation, LastUpdatedAt = x.LastUpdatedAt, Question = x.Question,
                         SortOrder = x.SortOrder, Type = x.Type
                     },
                     ExerciseOptionList = x.ExerciseOptions.Select(x => new ExerciseOptionDto()
                     {
                         OptionId = x.OptionId, ExerciseId = x.ExerciseId, SortOrder = x.SortOrder,
                         IsCorrect = x.IsCorrect, OptionText = x.OptionText,
                     }).ToList()
                 }).ToListAsync();
             return result;
         }
        
        
    }
}