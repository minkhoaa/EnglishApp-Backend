using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EnglishApp.Data;
using EnglishApp.Repository;

namespace EnglishApp.Service
{
    public interface IExerciseService
    {
        Task<List<Exercise>> GetAllAsync();
        Task<List<Exercise>> GetByLessonIdAsync(int lessonId);
        Task<Exercise?> GetByIdAsync(int id);
        Task<Exercise> AddAsync(Exercise e);
        Task UpdateAsync(Exercise e);
        Task DeleteAsync(int id);
        
        
    }
    public class ExerciseService : IExerciseService
    {
        private readonly IExerciseRepository _repo;
        public ExerciseService(IExerciseRepository repo) => _repo = repo;
        public Task<List<Exercise>> GetAllAsync()
        {
            return _repo.GetAllAsync();
        }

        public Task<List<Exercise>> GetByLessonIdAsync(int lessonId) => _repo.GetByLessonIdAsync(lessonId);
        public Task<Exercise?> GetByIdAsync(int id) => _repo.GetByIdAsync(id);
        public Task<Exercise> AddAsync(Exercise e) => _repo.AddAsync(e);
        public Task UpdateAsync(Exercise e) => _repo.UpdateAsync(e);
        public Task DeleteAsync(int id) => _repo.DeleteAsync(id);
    }

}