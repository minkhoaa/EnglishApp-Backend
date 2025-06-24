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
  [ApiController]
[Route("api/categories")]
public class CategoryController : ControllerBase
{
    private readonly ICategoryService _service;
    public CategoryController(ICategoryService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _service.GetAllAsync());

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var cat = await _service.GetByIdAsync(id);
        return cat != null ? Ok(cat) : NotFound();
    }

    [HttpPost]
    public async Task<IActionResult> Create(CategoryDto dto)
    {
        var cat = await _service.AddAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = cat.CategoryId }, cat);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, Category dto)
    {
        if (id != dto.CategoryId) return BadRequest();
        await _service.UpdateAsync(dto);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _service.DeleteAsync(id);
        return NoContent();
    }
}

}
