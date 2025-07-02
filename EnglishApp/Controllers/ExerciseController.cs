using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EnglishApp.Data;
using EnglishApp.Dto.Request;
using EnglishApp.Service;
using Microsoft.AspNetCore.Mvc;

namespace EnglishApp.Controllers
{
    [ApiController]
    [Route("api/exercises")]
        public class ExerciseOptionController : ControllerBase
        {
            private readonly IExerciseService _service;
            private readonly IExerciseOptionService _exerciseOptionService;
            private readonly EnglishAppDbContext _context;

            public ExerciseOptionController(IExerciseService service, IExerciseOptionService exerciseOptionService, EnglishAppDbContext context)
            {
                _service = service; 
                _exerciseOptionService = exerciseOptionService;
                _context = context;
            }

            [HttpGet("/api/getallexercise")]
            public async Task<List<Exercise>> GetAllAsync()
            {
                return await _service.GetAllAsync();
            }
            
            [HttpGet("/api/exercises/{lessonId}")]
            public async Task<IActionResult> GetAll(int lessonId)
                => Ok(await _service.GetByLessonIdAsync(lessonId));

            [HttpPost("/api/addcexercise")]
            public async Task<IActionResult> Create(int LessonId,[FromBody] AddExerciseDto dto)
            {
               

                // Bước 1: tạo và lưu Exercise
                var exercise = new Exercise
                {
                    LessonId = LessonId,
                    Type = dto.Exercise.Type,
                    Question = dto.Exercise.Question,
                    Explanation = dto.Exercise.Explanation,
                    SortOrder = dto.Exercise.SortOrder,
                    CreatedAt = DateTime.UtcNow,
                    LastUpdatedAt = DateTime.UtcNow
                };

                _context.Exercises.Add(exercise);
                await _context.SaveChangesAsync(); // Bây giờ exercise.ExerciseId đã có giá trị thật

// Bước 2: gán ExerciseId cho từng option
                var options = dto.Options.Select(x => new ExerciseOption
                {
                    ExerciseId = exercise.ExerciseId, // ✅ đây là chỗ EF cần biết
                    OptionText = x.OptionText,
                    IsCorrect = x.IsCorrect,
                    SortOrder = x.SortOrder
                }).ToList();

                _context.ExerciseOptions.AddRange(options);
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    exercise.ExerciseId, 
                    exercise.Question,
                    options = exercise.ExerciseOptions.Select(x=> new {x.OptionId, x.OptionText, x.IsCorrect}).ToList()
                });
            }

            [HttpGet("/api/exercise-options/{id}")]
            public async Task<IActionResult> GetById(int id)
            {
                var o = await _exerciseOptionService.GetByIdAsync(id);
                return o == null ? NotFound() : Ok(o);
            }

            [HttpPut("/api/exercise-options/{id}")]
            public async Task<IActionResult> Update(int id, ExerciseOption dto)
            {
                if (id != dto.OptionId) return BadRequest();
                await _exerciseOptionService.UpdateAsync(dto);
                return NoContent();
            }

            [HttpDelete("/api/exercise-options/{id}")]
            public async Task<IActionResult> Delete(int id)
            {
                await _exerciseOptionService.DeleteAsync(id);
                return NoContent();
            }

            [HttpGet("/api/exercise-options/getallexerciseandoption")]
            public async Task<IActionResult> GetByLessonId(int lessonId)
            {
                var result = await _exerciseOptionService.GetAllExerciseAndFullOptionByLessonAsync(lessonId);
                return Ok(result);
            }
        }
    }
public class AddExerciseDto
{
    public AddNewExerciseDto Exercise { get; set; }
    public List<AddNewExerciseOptionDto> Options { get; set; }
}

public class AddNewExerciseDto
{
    public string Type { get; set; }
    public string Question { get; set; }
    public string Explanation { get; set; }
    public int SortOrder { get; set; }
}

public class AddNewExerciseOptionDto
{
    public string OptionText { get; set; }
    public bool IsCorrect { get; set; }
    public int SortOrder { get; set; }
}
