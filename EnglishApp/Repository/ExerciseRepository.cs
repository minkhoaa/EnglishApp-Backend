using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EnglishApp.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;

namespace EnglishApp.Repository
{
    public interface IExerciseRepository
{
    Task<List<Exercise>> GetAllAsync();
    Task<List<Exercise>> GetByLessonIdAsync(int lessonId);
    Task<Exercise?> GetByIdAsync(int id);
    Task<Exercise> AddAsync(Exercise e);
    Task UpdateAsync(Exercise e);
    Task DeleteAsync(int id);
}
public class ExerciseRepository : IExerciseRepository
{
    private readonly EnglishAppDbContext _db;

    public ExerciseRepository(EnglishAppDbContext db)
    {
        _db = db; 
    }

    public async Task<List<Exercise>> GetAllAsync()
    {
        return await _db.Exercises.ToListAsync();
    }

    public Task<List<Exercise>> GetByLessonIdAsync(int lessonId)
        => _db.Exercises.Where(x => x.LessonId == lessonId).AsNoTracking().ToListAsync();
    public Task<Exercise?> GetByIdAsync(int id) => _db.Exercises.FindAsync(id).AsTask();
    public async Task<Exercise> AddAsync(Exercise e) { _db.Exercises.Add(e); await _db.SaveChangesAsync(); return e; }
    public async Task UpdateAsync(Exercise e) { _db.Exercises.Update(e); await _db.SaveChangesAsync(); }
    public async Task DeleteAsync(int id) { var ee = await _db.Exercises.FindAsync(id); if(ee!=null){_db.Exercises.Remove(ee); await _db.SaveChangesAsync();} }
}

}