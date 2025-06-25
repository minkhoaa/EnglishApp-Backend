using EnglishApp.Data;
using EnglishApp.Dto.Response;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EnglishApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ExamController : ControllerBase
{
    private readonly EnglishAppDbContext _context;

    public ExamController(EnglishAppDbContext context)
    {
        _context = context;
    }

    [HttpGet("/api/fullexamsandquestion/{id}")]
    public async Task<IActionResult> GetExam(int id)
    {
        var result = await _context.Exams.AsNoTracking().Where(x => x.ExamId == id)
            .Select(exam => new ExamDto()
            {
                ExamId = exam.ExamId,
                Title = exam.Title,
                Description = exam.Description,
                Level = exam.Level,
                CreatedAt = exam.CreatedAt,
                sections = exam.Sections.Select(section => new ExamSectionDto()
                    {
                        SectionId = section.SectionId,
                        ExamId = section.ExamId,
                        Name = section.Name,
                        Transcript = section.Transcript,
                        AudioUrl = section.AudioUrl,
                        questions = section.Questions.Select(q => new ExamQuestionDto()
                        {
                            QuestionId = q.QuestionId,
                            SectionId = q.SectionId,
                            QuestionText = q.QuestionText,
                            Type = q.Type,
                            SortOrder = q.SortOrder,
                            CorrectAnswer = q.CorrectAnswer,
                            options = q.Options.Select(opt => new ExamOptionDto()
                            {
                                OptionId = opt.OptionId,
                                QuestionId = opt.QuestionId,
                                OptionText = opt.OptionText,
                                IsCorrect = opt.IsCorrect
                            }).ToList()
                        }).ToList()
                    }).ToList()
            }).FirstOrDefaultAsync();
        if (result == null) return NotFound();
        return Ok(result);


    }

}