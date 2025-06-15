using EnglishApp.Data;
using EnglishApp.Dto.Request;
using EnglishApp.Model;
using EnglishApp.Repository;
using EnglishApp.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EnglishApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly CategoryRepository _categoryRepository;
        public CategoryController(CategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }
        [HttpPost("addnewcategory")]
        public async Task<IActionResult> AddNewCategory(AddNewCategoryDto dto)
        {
            var result = await _categoryRepository.AddNewCategoryAsync(dto);
            return Ok(result);
        }
        [HttpGet("getallcategories")]
        [Authorize]
        public async Task<IActionResult> GetAllCategories()
        {
            var result = await _categoryRepository.GetAllCategoriesAsync();
            return Ok(result);
        }

    }
}
