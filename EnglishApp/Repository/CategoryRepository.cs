using EnglishApp.Dto.Request;
using EnglishApp.Model;
using Microsoft.AspNetCore.Mvc.ApiExplorer;

namespace EnglishApp.Repository
{
    public interface CategoryRepository
    {
        public Task<ApiResponse> AddNewCategoryAsync(AddNewCategoryDto dto);
        public Task<ApiResponse> GetAllCategoriesAsync();
    }
}
