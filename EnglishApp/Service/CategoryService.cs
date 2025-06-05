using EnglishApp.Data;
using EnglishApp.Dto.Request;
using EnglishApp.Model;
using EnglishApp.Repository;
using Microsoft.EntityFrameworkCore;

namespace EnglishApp.Service
{
    public class CategoryService : CategoryRepository
    {
        private readonly EnglishAppDbContext _context; 
        public CategoryService(EnglishAppDbContext context)
        {
            _context = context;
        }
        public async Task<ApiResponse> AddNewCategoryAsync(AddNewCategoryDto dto)
        {
            var category = new Category()
            {
                CategoryName = dto.CategoryName,
                CategoryDescription = dto.CategoryDescription,
            };
            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();  
            return new ApiResponse {Success = true, Message = "Thêm mới category thành công", Data = category};
        }

        public async Task<ApiResponse> GetAllCategoriesAsync()
        {
            return new ApiResponse { Success = true, Message = "Success", Data = await _context.Categories.AsNoTracking().ToListAsync() };
        }
    }
}
