using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EnglishApp.Data;
using Microsoft.EntityFrameworkCore;

namespace EnglishApp.Repository
{
    public interface IExerciseOptionRepository
{
    Task<List<ExerciseOption>> GetByExerciseIdAsync(int exerciseId);
    Task<ExerciseOption?> GetByIdAsync(int id);
    Task<ExerciseOption> AddAsync(ExerciseOption o);
    Task UpdateAsync(ExerciseOption o);
    Task DeleteAsync(int id);
}
public class ExerciseOptionRepository : IExerciseOptionRepository
{
    private readonly EnglishAppDbContext _db;
    public ExerciseOptionRepository(EnglishAppDbContext db) => _db = db;
    
    public Task<List<ExerciseOption>> GetByExerciseIdAsync(int exerciseId)
        => _db.ExerciseOptions.Where(x => x.ExerciseId == exerciseId).AsNoTracking().ToListAsync();
    public Task<ExerciseOption?> GetByIdAsync(int id) => _db.ExerciseOptions.FindAsync(id).AsTask();
    public async Task<ExerciseOption> AddAsync(ExerciseOption o) { _db.ExerciseOptions.Add(o); await _db.SaveChangesAsync(); return o; }
    public async Task UpdateAsync(ExerciseOption o) { _db.ExerciseOptions.Update(o); await _db.SaveChangesAsync(); }
    public async Task DeleteAsync(int id) { var e = await _db.ExerciseOptions.FindAsync(id); if(e!=null){_db.ExerciseOptions.Remove(e); await _db.SaveChangesAsync();} }
}

}