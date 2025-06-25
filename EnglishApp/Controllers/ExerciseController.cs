using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EnglishApp.Data;
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

            public ExerciseOptionController(IExerciseService service, IExerciseOptionService exerciseOptionService)
            {
                _service = service; 
                _exerciseOptionService = exerciseOptionService;
            }

            [HttpGet("/api/getallexercise")]
            public async Task<List<Exercise>> GetAllAsync()
            {
                return await _service.GetAllAsync();
            }
            
            [HttpGet("/api/exercises/{lessonId}")]
            public async Task<IActionResult> GetAll(int lessonId)
                => Ok(await _service.GetByLessonIdAsync(lessonId));

            [HttpPost]
            public async Task<IActionResult> Create(int exerciseId, ExerciseOption dto)
            {
                dto.ExerciseId = exerciseId;
                var o = await _exerciseOptionService.AddAsync(dto);
                return CreatedAtAction(nameof(GetById), new { exerciseId = exerciseId, id = o.OptionId }, o);
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
