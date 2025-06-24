using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EnglishApp.Data;
using EnglishApp.Repository;

namespace EnglishApp.Service
{
    public interface IExerciseOptionService
    {
        Task<List<ExerciseOption>> GetByExerciseIdAsync(int exerciseId);
        Task<ExerciseOption?> GetByIdAsync(int id);
        Task<ExerciseOption> AddAsync(ExerciseOption o);
        Task UpdateAsync(ExerciseOption o);
        Task DeleteAsync(int id);
    }
    public class ExerciseOptionService : IExerciseOptionService
    {
        private readonly IExerciseOptionRepository _repo;
        public ExerciseOptionService(IExerciseOptionRepository repo) => _repo = repo;
 

        public Task<List<ExerciseOption>> GetByExerciseIdAsync(int exerciseId) => _repo.GetByExerciseIdAsync(exerciseId);
        public Task<ExerciseOption?> GetByIdAsync(int id) => _repo.GetByIdAsync(id);
        public Task<ExerciseOption> AddAsync(ExerciseOption o) => _repo.AddAsync(o);
        public Task UpdateAsync(ExerciseOption o) => _repo.UpdateAsync(o);
        public Task DeleteAsync(int id) => _repo.DeleteAsync(id);
    }
}