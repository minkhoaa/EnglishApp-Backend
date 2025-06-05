using EnglishApp.Dto.Request;
using EnglishApp.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using System.Security.Policy;

namespace EnglishApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LessonController : ControllerBase
    {
        private readonly LessonRepository _lessonRepository;
        public LessonController(LessonRepository lessonRepository)
        {
            _lessonRepository = lessonRepository;  
        }

        [HttpPost("addnewlesson")]
        public async Task<IActionResult> AddNewLesson(AddNewLessonDto dto)
        {
            var result = await _lessonRepository.AddNewLessonAsync(dto);
            return (result.Success) ? Ok(result) : BadRequest(result);
        }
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAllLessons()
        {
            var result = await _lessonRepository.GetAllLessonAsync();
            return Ok(result);
        }
    }
}
