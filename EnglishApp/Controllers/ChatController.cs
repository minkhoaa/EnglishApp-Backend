using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using EnglishApp.Data;
using EnglishApp.Dto.Request;
using EnglishApp.Migrations;
using EnglishApp.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace EnglishApp.Controllers;

[ApiController]
[Route("api/writing")]
public class ChatController : ControllerBase
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    private readonly EnglishAppDbContext _context;

    public ChatController(IHttpClientFactory httpClientFactory, IOptions<OpenRouterSettings> options, EnglishAppDbContext context)
    {
        _httpClient = httpClientFactory.CreateClient();
        _context = context;
        _apiKey = options.Value.ApiKey;
    }

[HttpPost("/writingscore/test")]
public async Task<IActionResult> TestChatWithBot([FromBody] ChatTextRequest userMessage)
{
    string prompt = $"""
    Tôi muốn bạn đóng vai một giám khảo IELTS Writing Task 2, với đề bài như sau:
    {userMessage.DeBai}
    - Hãy đánh giá bài viết dưới đây theo đúng 4 tiêu chí của IELTS:
    - Task Response
    - Coherence and Cohesion
    - Lexical Resource
    - Grammatical Range and Accuracy

    Hãy đưa ra:
    1. Nhận xét chi tiết cho từng tiêu chí
    2. Gợi ý cụ thể để cải thiện bài viết và tăng band điểm
    3. Ước lượng band điểm tổng thể
    4. Gợi ý một đoạn viết lại (optional nếu cần)
    - Nhận xét được được trả về phải hoàn toàn bằng tiếng việt, cách diễn đạt phải dễ hiểu và thân thiện với người dùng
    Bài viết:
    \"\"\"
    {userMessage.BaiLam}
    \"\"\"
    """;

    var requestBody = new
    {
        model = "deepseek/deepseek-chat:free",
        messages = new[]
        {
            new { role = "user", content = prompt }
        }
    };

    var requestJson = JsonSerializer.Serialize(requestBody);
    using var requestContent = new StringContent(requestJson, Encoding.UTF8, "application/json");

    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
    _httpClient.DefaultRequestHeaders.Remove("HTTP-Referer");
    _httpClient.DefaultRequestHeaders.Remove("X-Title");
    _httpClient.DefaultRequestHeaders.Add("HTTP-Referer", "https://localhost");
    _httpClient.DefaultRequestHeaders.Add("X-Title", "IELTS Grader App");

    try
    {
        
        var response = await _httpClient.PostAsync("https://openrouter.ai/api/v1/chat/completions", requestContent);
        var result = await response.Content.ReadAsStringAsync();
        
        // Parse response JSON
        var aiResponse = JsonDocument.Parse(result);
        var content = aiResponse.RootElement
            .GetProperty("choices")[0]
            .GetProperty("message")
            .GetProperty("content")
            .GetString();

        if (string.IsNullOrWhiteSpace(content))
        {
            return StatusCode(500, "Không nhận được phản hồi từ AI.");
        }
        var testInfo = await _context.WritingTests.AsNoTracking().Where(x=>x.Id == userMessage.ExamWritingId).Select(x=>new {x.Title, x.Description}).FirstOrDefaultAsync();
        var history = new UserWritingHistory
        {
            UserId = userMessage.UserId,
            ExamId = userMessage.ExamWritingId,
            UserAnswer = userMessage.BaiLam,
            Title = testInfo.Title,
            Description = testInfo.Description,
            AiFeedback = content, // <-- chỉ lưu phần content này thôi
            SubmittedAt = DateTime.UtcNow
        };
        await _context.UserWritingHistories.AddAsync(history);
        await _context.SaveChangesAsync();

        
        return Content(result, "application/json; charset=utf-8");
    }
    catch (TaskCanceledException)
    {
        return StatusCode(504, "Yêu cầu quá thời gian chờ, thử lại sau.");
    }
    catch (Exception ex)
    {
        return StatusCode(500, $"Lỗi không xác định: {ex.Message}");
    }
}

    [HttpGet("/api/getallexam")]
    public async Task<IActionResult> AllWritingExamTitle()
    {
        var result = await _context.WritingTests.AsNoTracking().Select(x => new { x.Id, x.Title, x.Description })
            .ToListAsync();
        return Ok(result); 
    }

    [HttpGet("/api/getexamcontentbyidexam")]
    public async Task<IActionResult> AllWritingExamContent(int idExam)
    {
        var result = await _context.WritingExamContents.AsNoTracking().Where(x => x.ExamId == idExam)
            .Select(x => new AllWritingExamContent()
            {
                Id = x.Id, ExamId = x.ExamId, Question = x.Question, Title = x.Tests.Title, SampleAnswer = x.SampleAnswer
            })
            .ToListAsync();
        return Ok(result);
    } 
    [HttpPost("/api/addwritingexam")]
    public async Task<IActionResult> AddWritingExam(WritingDto dto)
    {
        var writingtest = new WritingTest
        {
            Title = dto.Title,
            Description = dto.Description,
        };
        await _context.WritingTests.AddAsync(writingtest);
        await _context.SaveChangesAsync();
        var writingcontent = new WritingExamContent()
        {
            ExamId = writingtest.Id,
            Question = dto.Question,
            SampleAnswer = dto.SampleAnswer,
        };
        await _context.WritingExamContents.AddAsync(writingcontent);
        await _context.SaveChangesAsync();
        return Ok(new { writingtest.Id, writingtest.Title, writingtest.Description } );
    }

    [HttpGet("/api/userhistoryexam")]
    public async Task<IActionResult> HistoryExam(int userId)
    {
        var histories = await _context.UserWritingHistories
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.SubmittedAt)
            .Select(x => new
            {
                x.Id,
                x.ExamId,
                x.Title,
                x.Description,
                x.UserAnswer,
                x.AiFeedback,
                x.SubmittedAt
            })
            .ToListAsync();

        return Ok(histories);
    }

}

public class ChatTextRequest
{
    public int UserId { get; set; }
    public int ExamWritingId { get; set; }
    public string DeBai { get; set; }
    public string BaiLam { get; set; }
}


public class AllWritingExamContent
{
    public int Id { get; set; }
    public int ExamId { get; set; }
    public string Title { get; set; }
    public string Question { get; set; }
    public string SampleAnswer { get; set; } 
    
}