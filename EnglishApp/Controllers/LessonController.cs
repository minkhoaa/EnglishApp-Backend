using EnglishApp.Data;
using EnglishApp.Dto.Request;
using EnglishApp.Repository;
using EnglishApp.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using System.Security.Policy;

namespace EnglishApp.Controllers
{
    [ApiController]
    [Route("api/lessons")]
    public class LessonController : ControllerBase
    {
        private readonly ILessonService _svc;
        public LessonController(ILessonService svc) => _svc = svc;

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int? categoryId, [FromQuery] string? level)
            => Ok(await _svc.GetAllAsync(categoryId, level));

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var l = await _svc.GetByIdAsync(id);
            return l == null ? NotFound() : Ok(l);
        }

        [HttpGet("popular")]
        public async Task<IActionResult> GetPopular() => Ok(await _svc.GetPopularAsync());

        [HttpPost]
        public async Task<IActionResult> Create(Lesson dto)
        {
            var l = await _svc.AddAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = l.LessonId }, l);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Lesson dto)
        {
            if (id != dto.LessonId) return BadRequest();
            await _svc.UpdateAsync(dto);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _svc.DeleteAsync(id);
            return NoContent();
        }
    }

}
