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
        var transcript =  await _context.ExamSections.AsNoTracking().Where(a=>a.ExamId == examId && a.SectionId == sectionId).Select(x=>x.Transcript).FirstOrDefaultAsync();
        var correctAnswers = await _context.ExamOptions
            .AsNoTracking()
            .Where(x => ids.Contains(x.QuestionId) && x.IsCorrect)
            .Select(x => new { x.QuestionId, x.OptionId, x.OptionText })
            .ToListAsync();
        var result = userresult.Select(x =>
        {
            var correct = correctAnswers.FirstOrDefault(a => a.QuestionId == x.QuestionId);
            return new DetailedResult
            {
                    UserAnswerId = x.UserExamResultId,
                    UserAnswer = x.AnswerText,
                    IsCorrect = x.IsCorrect,
                    CorrectOptionId = correct?.OptionId,
                    CorrectAnswer = correct?.OptionText
            };
        }).ToList();
        return Ok(new DetailedUserExamResult(){Transript = transcript, DetailedResults = result});
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

    [HttpGet("/api/getuserelisteningreadinghistory")]
    public async Task<IActionResult> GetUserListeningReadingHistory(int userId)
    {
        var history = await _context.UserExamResults.AsNoTracking()
            .Where(x => x.UserId == userId && x.IsSubmitted)
            .GroupBy(x => new { x.ExamId, x.SectionId })
            .Select(g => new
            {
                ExamId = g.Key.ExamId,
                SectionId = g.Key.SectionId,
                LatestAnsweredAt = g.Max(x => x.AnsweredAt),
                TotalQuestions = g.Count(),
                CorrectAnswers = g.Count(x => x.IsCorrect == true),
                IncorrectAnswers = g.Count(x => x.IsCorrect == false)
            })
            .OrderByDescending(x => x.LatestAnsweredAt)
            .ToListAsync();
        var examIds = history.Select(h => h.ExamId).Distinct().ToList();
        var examDict = await _context.Exams
            .Where(e => examIds.Contains(e.ExamId))
            .ToDictionaryAsync(e => e.ExamId, e => e.Title);

        var sectionIds = history.Select(h => h.SectionId).Distinct().ToList();
        var sectionDict = await _context.ExamSections
            .Where(s => sectionIds.Contains(s.SectionId))
            .ToDictionaryAsync(s => s.SectionId, s => s.Name);
        var result = history.Select(h => new 
        {
            h.ExamId,
            ExamTitle = examDict.ContainsKey(h.ExamId) ? examDict[h.ExamId] : null,
            h.SectionId,
            SectionName = h.SectionId != null && sectionDict.ContainsKey(h.SectionId.Value)
                ? sectionDict[h.SectionId.Value]
                : null, 
            h.LatestAnsweredAt,
            h.TotalQuestions,
            h.CorrectAnswers,
            h.IncorrectAnswers,
            Score = Math.Round((double)h.CorrectAnswers / Math.Max(h.TotalQuestions,1) * 100, 1)
        });

        return Ok(result);
    }
    
}