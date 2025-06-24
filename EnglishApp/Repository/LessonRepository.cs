using EnglishApp.Data;
using EnglishApp.Dto.Request;
using EnglishApp.Model;
using Microsoft.EntityFrameworkCore;

namespace EnglishApp.Repository
{
    public interface ILessonRepository
    {
        Task<List<Lesson>> GetAllAsync(int? categoryId = null, string? level = null);
        Task<Lesson?> GetByIdAsync(int id);
        Task<List<Lesson>> GetPopularAsync();
        Task<Lesson> AddAsync(Lesson lesson);
        Task UpdateAsync(Lesson lesson);
        Task DeleteAsync(int id);
    }
    public class LessonRepository : ILessonRepository
    {
        private readonly EnglishAppDbContext _db;
        public LessonRepository(EnglishAppDbContext db) => _db = db;

        public Task<List<Lesson>> GetAllAsync(int? categoryId = null, string? level = null)
        {
            var q = _db.Lessons.AsQueryable();
            if (categoryId.HasValue) q = q.AsNoTracking().Where(l => l.CategoryId == categoryId);
            if (!string.IsNullOrEmpty(level)) q = q.Where(l => l.Level == level);
            return q.ToListAsync();
        }
        public Task<Lesson?> GetByIdAsync(int id) => _db.Lessons.FindAsync(id).AsTask();
        public Task<List<Lesson>> GetPopularAsync() => _db.Lessons.OrderByDescending(x => x.CreatedAt).Take(10).ToListAsync(); // ví dụ
        public async Task<Lesson> AddAsync(Lesson l) { _db.Lessons.Add(l); await _db.SaveChangesAsync(); return l; }
        public async Task UpdateAsync(Lesson l) { _db.Lessons.Update(l); await _db.SaveChangesAsync(); }
        public async Task DeleteAsync(int id) { var e = await _db.Lessons.FindAsync(id); if (e != null) { _db.Lessons.Remove(e); await _db.SaveChangesAsync(); } }
    }
}