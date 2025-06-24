using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EnglishApp.Data;
using Microsoft.EntityFrameworkCore;

namespace EnglishApp.Repository
{
   public interface ILessonContentRepository
{
    Task<List<LessonContent>> GetByLessonIdAsync(int lessonId);
    Task<LessonContent?> GetByIdAsync(int id);
    Task<LessonContent> AddAsync(LessonContent c);
    Task UpdateAsync(LessonContent c);
    Task DeleteAsync(int id);
}
public class LessonContentRepository : ILessonContentRepository
{
    private readonly EnglishAppDbContext _db;
    public LessonContentRepository(EnglishAppDbContext db) => _db = db;

    public Task<List<LessonContent>> GetByLessonIdAsync(int lessonId)
        => _db.LessonContents.Where(x => x.LessonId == lessonId).AsNoTracking().ToListAsync();
    public Task<LessonContent?> GetByIdAsync(int id) => _db.LessonContents.FindAsync(id).AsTask();
    public async Task<LessonContent> AddAsync(LessonContent c) { _db.LessonContents.Add(c); await _db.SaveChangesAsync(); return c; }
    public async Task UpdateAsync(LessonContent c) { _db.LessonContents.Update(c); await _db.SaveChangesAsync(); }
    public async Task DeleteAsync(int id) { var e = await _db.LessonContents.FindAsync(id); if(e!=null){_db.LessonContents.Remove(e); await _db.SaveChangesAsync();} }
}

}