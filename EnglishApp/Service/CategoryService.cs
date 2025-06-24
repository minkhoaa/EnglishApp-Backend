using EnglishApp.Data;
using EnglishApp.Dto.Request;
using EnglishApp.Model;
using EnglishApp.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;

namespace EnglishApp.Service
{
  public interface ICategoryService
{
    Task<List<Category>> GetAllAsync();
    Task<Category?> GetByIdAsync(int id);
    Task<Category> AddAsync(CategoryDto category);
    Task UpdateAsync(Category category);
    Task DeleteAsync(int id);
}

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _repo;
    public CategoryService(ICategoryRepository repo) => _repo = repo;

    public Task<List<Category>> GetAllAsync() => _repo.GetAllAsync();
    public Task<Category?> GetByIdAsync(int id) => _repo.GetByIdAsync(id);
    public Task<Category> AddAsync(CategoryDto category) => _repo.AddAsync(category);
    public Task UpdateAsync(Category category) => _repo.UpdateAsync(category);
    public Task DeleteAsync(int id) => _repo.DeleteAsync(id);
}

}
