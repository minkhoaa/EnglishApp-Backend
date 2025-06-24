using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using EnglishApp.Data;
using EnglishApp.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace EnglishApp.Controllers
{
   [ApiController]
[Route("api/lessons/{lessonId}/contents")]
public class LessonContentController : ControllerBase
{
    private readonly ILessonContentService _svc;
    public LessonContentController(ILessonContentService svc) => _svc = svc;

    [HttpGet]
    public async Task<IActionResult> GetAll(int lessonId)
        => Ok(await _svc.GetByLessonIdAsync(lessonId));

    [HttpGet("/api/lesson-contents/{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var c = await _svc.GetByIdAsync(id);
        return c == null ? NotFound() : Ok(c);
    }

    [HttpPost]
    public async Task<IActionResult> Create(int lessonId, LessonContent dto)
    {
        dto.LessonId = lessonId;
        var c = await _svc.AddAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = c.ContentId }, c);
    }

    [HttpPut("/api/lesson-contents/{id}")]
    public async Task<IActionResult> Update(int id, LessonContent dto)
    {
        if (id != dto.ContentId) return BadRequest();
        await _svc.UpdateAsync(dto);
        return NoContent();
    }

    [HttpDelete("/api/lesson-contents/{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _svc.DeleteAsync(id);
        return NoContent();
    }
}

}