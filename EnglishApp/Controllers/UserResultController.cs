using EnglishApp.Data;
using EnglishApp.Dto.Request;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EnglishApp.Controllers;

[ApiController]
[Route("api/userprogress")]
public class UserResultController : ControllerBase
{
    private readonly EnglishAppDbContext _context;

    public UserResultController(EnglishAppDbContext context)
    {
        _context = context;
    }

    [HttpPost("/api/adduserexamresult")]
    public async Task<IActionResult> AddUserExamResult(List<UserExamResultDto> results)
    {
        foreach (var result in results)
        {
            var correctOptions = await _context.ExamOptions.AsNoTracking()
                .Where(x => x.QuestionId == result.QuestionId && x.IsCorrect)
                .ToListAsync();

            // 2. So sánh optionId
            bool isOptionCorrect = correctOptions.Any(opt => opt.OptionId == result.AnswerOptionId);

            // 3. So sánh answerText
            bool isTextCorrect = correctOptions.Any(opt =>
                !string.IsNullOrWhiteSpace(opt.OptionText) &&
                opt.OptionText.Trim().ToLower() == (result.AnswerText ?? "").Trim().ToLower()
            );

            // 4. Chỉ đúng khi cả hai trường đều đúng (AND)
            bool isCorrect = isOptionCorrect && isTextCorrect;

            
            
            var existing = await _context.UserExamResults
                .FirstOrDefaultAsync(x =>
                    x.UserId == result.UserId &&
                    x.ExamId == result.ExamId &&
                    x.QuestionId == result.QuestionId);
            if (existing!=null)
            {
                existing.AnswerOptionId = result.AnswerOptionId;
                existing.AnswerText = result.AnswerText;
                existing.AnswerJson = result.AnswerJson;
                existing.IsCorrect = isCorrect;
                existing.AnsweredAt = result.AnsweredAt ?? DateTime.UtcNow;
                existing.IsSubmitted = result.IsSubmitted;
            }

            else
            {
                var userResult = new UserExamResult
                {
                    UserId = result.UserId,
                    ExamId = result.ExamId,
                    SectionId = result.SectionId,
                    QuestionId = result.QuestionId,
                    AnswerOptionId = result.AnswerOptionId,
                    AnswerText = result.AnswerText,
                    AnswerJson = result.AnswerJson,
                    IsCorrect = isCorrect ,
                    AnsweredAt = result.AnsweredAt ?? DateTime.UtcNow,
                    IsSubmitted = result.IsSubmitted
                };
                await _context.UserExamResults.AddAsync(userResult);

            }
        }
        await _context.SaveChangesAsync();
        return Ok(new {message = "save successfully"});
    }

    [HttpGet("/api/alluserexamresult")]
    public async Task<IActionResult> GetAllUserExamResult()
    {
        var result = await _context.UserExamResults.AsNoTracking()
            .Include(x=>x.User)
            .Include(x=>x.Exam)
            .Include(x=>x.Question)
            .Include(x=>x.AnswerOption)
            .ToListAsync();
        return Ok(result);
    }
    
}