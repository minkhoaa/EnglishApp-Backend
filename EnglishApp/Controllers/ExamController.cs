using EnglishApp.Data;
using EnglishApp.Dto.Request;
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

    [HttpPost("/api/addexamcategory")]
    public async Task<IActionResult> AddExamCategory(ExamCategoryDto dto)
    {
        try
        {
            var category = new ExamCategory()
            {
                Name = dto.Name,
                Description = dto.Description,
                Level = dto.Level,
            };
            _context.ExamCategories.Add(category);
            await _context.SaveChangesAsync();
            return Ok(category);
        }
        catch (Exception exception)
        {
            return BadRequest(exception.Message); 
        }
}

    [HttpGet("/api/allcategories")]
    public async Task<IActionResult> GetAllCategories()
    {
        try
        {
            var result = await _context.ExamCategories.AsNoTracking().Select(x => new ExamCategoryResponse()
            {
                ExamCategoryId = x.ExamCategoryId,
                Name = x.Name,
                Description = x.Description,
                Level = x.Level,
            }).ToListAsync();
            return Ok(result);

        } catch (Exception exception)
        {
            return BadRequest(exception.Message);
        }
    }

    [HttpGet("/api/allexams")]
    public async Task<IActionResult> GetAllExams()
    {
        try
        {
            var result = await _context.Exams.AsNoTracking().Select(exam => new ExamDto()
            {
                ExamId = exam.ExamId,
                Title = exam.Title,
                Description = exam.Description,
                Level = exam.Level,
                CreatedAt = exam.CreatedAt,
            }).ToListAsync();
            return Ok(result);
        }
        catch (Exception exception)
        {
            return BadRequest(exception.Message);
        }
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
    [HttpGet("/api/exams/GetSectionsByExamId/{examId}")]
    public async Task<IActionResult> GetSectionsByExamId(int examId)
    {
        var sections = await _context.ExamSections
            .AsNoTracking()
            .Where(s => s.ExamId == examId)
            .Select(section => new ExamSectionDto
            {
                SectionId = section.SectionId,
                ExamId = section.ExamId,
                Name = section.Name,
                Transcript = section.Transcript,
                AudioUrl = section.AudioUrl,
                SortOrder = section.SortOrder
            })
            .ToListAsync();

        if (sections == null || !sections.Any()) return NotFound();
        return Ok(sections);
    }
    [HttpGet("/api/sections/GetQuestionsBySectionId/{sectionId}")]
    public async Task<IActionResult> GetQuestionsBySectionId(int sectionId)
    {
        var questions = await _context.ExamQuestions
            .AsNoTracking()
            .Where(q => q.SectionId == sectionId)
            .Select(q => new ExamQuestionDto
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
            })
            .ToListAsync();

        if (questions == null || !questions.Any()) return NotFound();
        return Ok(questions);
    }

    [HttpGet("/api/questions/GetOptionsByQuestionId/{questionId}")]
    public async Task<IActionResult> GetOptionsByQuestionId(int questionId)
    {
        var options = await _context.ExamOptions
            .AsNoTracking()
            .Where(opt => opt.QuestionId == questionId)
            .Select(opt => new ExamOptionDto
            {
                OptionId = opt.OptionId,
                QuestionId = opt.QuestionId,
                OptionText = opt.OptionText,
                IsCorrect = opt.IsCorrect,
                SortOrder = opt.SortOrder
            })
            .ToListAsync();

        if (options == null || !options.Any()) return NotFound();
        return Ok(options);
    }

    
    [HttpPost("/api/addfullexamsandquestion")]
    public async Task<IActionResult> AddExam([FromBody] CreateExamDto examDto)
    {
        var exam = new Exam
        {
            Title = examDto.Title,
            Description = examDto.Description,
            Level = examDto.Level,
            Image = examDto.Image,
            CreatedAt = DateTime.UtcNow,
            ExamCategoryId = examDto.CategoryId,
            Sections = new List<ExamSection>()
        };

        foreach (var sectionDto in examDto.Sections)
        {
            var section = new ExamSection
            {
                Name = sectionDto.Name,
                AudioUrl = sectionDto.AudioUrl,
                Transcript = sectionDto.Transcript,
                SortOrder = sectionDto.SortOrder,
                Questions = new List<ExamQuestion>()
            };

            foreach (var questionDto in sectionDto.Questions)
            {
                var question = new ExamQuestion
                {
                    QuestionText = questionDto.QuestionText,
                    Type = questionDto.Type,
                    SortOrder = questionDto.SortOrder,
                    SampleImage = questionDto.SampleImage ?? "",
                    CorrectAnswer = questionDto.CorrectAnswer,
                    Options = new List<ExamOption>()
                };

                if (questionDto.Options != null)
                {
                    foreach (var optDto in questionDto.Options)
                    {
                        var option = new ExamOption
                        {
                            OptionText = optDto.OptionText,
                            IsCorrect = optDto.IsCorrect,
                            SortOrder = optDto.SortOrder
                        };
                        question.Options.Add(option); // gắn qua navigation
                    }
                }

                section.Questions.Add(question); // gắn qua navigation
            }

            exam.Sections.Add(section); // gắn qua navigation
        }

        await _context.Exams.AddAsync(exam); // chỉ cần add exam là đủ
        await _context.SaveChangesAsync();   // EF sẽ tự lưu toàn bộ quan hệ con

        return CreatedAtAction(nameof(GetExam), new { id = exam.ExamId }, new { exam.ExamId });
    }

}