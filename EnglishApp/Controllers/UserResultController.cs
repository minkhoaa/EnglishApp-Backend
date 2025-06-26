using System.Diagnostics;
using EnglishApp.Data;
using EnglishApp.Dto.Request;
using EnglishApp.Dto.Response;
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

    [HttpGet("/api/getuserexamresult")]
    public async Task<IActionResult> GetUserExamResult(int userId, int  examId,  int sectionId)
    {
        var userresult = await _context.UserExamResults.AsNoTracking()
            .Include(x=>x.Section).Where(x => x.UserId == userId && x.ExamId == examId)   
            .ToListAsync();
        
        var ids = userresult.Select(x=>x.QuestionId).Distinct().ToList();

        var correctAnswers = await _context.ExamOptions
            .AsNoTracking()
            .Where(x => ids.Contains(x.QuestionId) && x.IsCorrect)
            .Select(x => new { x.QuestionId, x.OptionId, x.OptionText })
            .ToListAsync();
        var result = userresult.Select(x =>
        {
            var correct = correctAnswers.FirstOrDefault(a => a.QuestionId == x.QuestionId);
            return new DetailedResult()
            {
                UserAnswerId = x.UserExamResultId,
                UserAnwser = x.AnswerText,
                IsCorrect = x.IsCorrect,
                CorrectOptionId = correct.OptionId,
                CorrectAnswer = correct.OptionText,
            };
        }).ToList();
        return Ok(result);
    }

    [HttpGet("/api/getexamscore")]
    public async Task<IActionResult> GetUserExamScore(int idUser, int idExam, int sectionId)
    {
        var question = await _context.ExamQuestions.AsNoTracking()
            .Where(x => x.SectionId == sectionId)
            .Select(x => new { x.QuestionId, x.Type })
            .ToListAsync();
        
        var questionIds = question.Select(x => x.QuestionId);
        
        var questionDics = question.ToDictionary(x=>x.QuestionId, x=>x.Type);
        var userResult = await _context.UserExamResults.AsNoTracking()
            .Where(x => x.UserId == idUser && x.ExamId == idExam && x.SectionId == sectionId).ToListAsync();
        int totalQuestion = question.Count();
        int correctAnswer =  userResult.Count(x=>x.IsCorrect ==true);
        int incorrectAnswer =  userResult.Count(x=>x.IsCorrect ==false);
        var typeStats = question.GroupBy(x => x.Type).Select(x =>
        {
            var ids = x.Select(a => a.QuestionId).ToList();
            var total = ids.Count;
            var correctByType = userResult.Count(r => ids.Contains(r.QuestionId) && r.IsCorrect == true);
            return new QuestionTypeStats()
            {
                Type = x.Key,
                Total = total,
                Correct = correctByType,
                Incorrect = total - correctByType,
            };
        }).ToList();
        var dto = new UserExamAnalyse()
        {
            SectionId = sectionId,
            TotalQuestions = totalQuestion,
            CorrectAnswers = correctAnswer,
            IncorrectAnswers = incorrectAnswer,
            PercentCorrect = Math.Round((double)correctAnswer / totalQuestion * 100, 1),
            TypeStats = typeStats
        };
            return Ok(dto);

    }
  
    
}