using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EnglishApp.Data;
using EnglishApp.Repository;

namespace EnglishApp.Service
{
   public interface ILessonContentService
{
    Task<List<LessonContent>> GetByLessonIdAsync(int lessonId);
    Task<LessonContent?> GetByIdAsync(int id);
    Task<LessonContent> AddAsync(LessonContent c);
    Task UpdateAsync(LessonContent c);
    Task DeleteAsync(int id);
}
public class LessonContentService : ILessonContentService
{
    private readonly ILessonContentRepository _repo;
    public LessonContentService(ILessonContentRepository repo) => _repo = repo;
    public Task<List<LessonContent>> GetByLessonIdAsync(int lessonId) => _repo.GetByLessonIdAsync(lessonId);
    public Task<LessonContent?> GetByIdAsync(int id) => _repo.GetByIdAsync(id);
    public Task<LessonContent> AddAsync(LessonContent c) => _repo.AddAsync(c);
    public Task UpdateAsync(LessonContent c) => _repo.UpdateAsync(c);
    public Task DeleteAsync(int id) => _repo.DeleteAsync(id);
}

}