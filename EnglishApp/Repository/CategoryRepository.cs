using System.ComponentModel;
using EnglishApp.Data;
using EnglishApp.Dto.Request;
using EnglishApp.Model;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.EntityFrameworkCore;

namespace EnglishApp.Repository
{
    public interface ICategoryRepository
{
    Task<List<Category>> GetAllAsync();
    Task<Category?> GetByIdAsync(int id);
    Task<Category> AddAsync(CategoryDto category);
    Task<ApiResponse> UpdateAsync(Category category);
    Task DeleteAsync(int id);
}

public class CategoryRepository(EnglishAppDbContext db) : ICategoryRepository
{
    public Task<List<Category>> GetAllAsync() => db.Categories.AsNoTracking().ToListAsync();

    public Task<Category?> GetByIdAsync(int id) => db.Categories.FindAsync(id).AsTask();

    public async Task<Category> AddAsync(CategoryDto category)
    {
        var result = new Category()
        {
            CategoryDescription = category.CategoryDescription,
            CategoryName = category.CategoryName,
        };
        db.Categories.Add(result);
        await db.SaveChangesAsync();
        return result;
    }



    public async Task<ApiResponse> UpdateAsync(Category category)
    {
        var effectedRow = await db.Categories.Where(x => x.CategoryId == category.CategoryId)
            .ExecuteUpdateAsync(setter => setter
                .SetProperty(x => x.CategoryDescription, category.CategoryDescription)
                .SetProperty(x => x.CategoryName, category.CategoryName)
            );
        return (effectedRow > 0) ? new ApiResponse() {Success = true, Message ="Update Success",Data = effectedRow} 
            : new ApiResponse(){Success = false, Message = "Failed to update category",Data = null};
    }

    public async Task DeleteAsync(int id)
    {
        var c = await db.Categories.FindAsync(id);
        if (c != null)
        {
            db.Categories.Remove(c);
            await db.SaveChangesAsync();
        }
    }
}

}
