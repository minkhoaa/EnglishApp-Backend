using EnglishApp.Data;
using EnglishApp.Dto.Request;
using EnglishApp.Model;
using EnglishApp.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace EnglishApp.Service
{
    public interface ILessonService
    {
        Task<List<Lesson>> GetAllAsync(int? categoryId = null, string? level = null);
        Task<Lesson?> GetByIdAsync(int id);
        Task<List<Lesson>> GetPopularAsync();
        Task<Lesson> AddAsync(Lesson l);
        Task UpdateAsync(Lesson l);
        Task DeleteAsync(int id);
    }
    public class LessonService : ILessonService
    {
        private readonly ILessonRepository _repo;
        public LessonService(ILessonRepository repo) => _repo = repo;
        public Task<List<Lesson>> GetAllAsync(int? categoryId = null, string? level = null) => _repo.GetAllAsync(categoryId, level);
        public Task<Lesson?> GetByIdAsync(int id) => _repo.GetByIdAsync(id);
        public Task<List<Lesson>> GetPopularAsync() => _repo.GetPopularAsync();
        public Task<Lesson> AddAsync(Lesson l) => _repo.AddAsync(l);
        public Task UpdateAsync(Lesson l) => _repo.UpdateAsync(l);
        public Task DeleteAsync(int id) => _repo.DeleteAsync(id);
    }
}