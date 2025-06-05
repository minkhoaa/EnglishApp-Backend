using EnglishApp.Data;
using EnglishApp.Dto.Request;
using EnglishApp.Model;
using EnglishApp.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace EnglishApp.Service
{
    public class LessonService : LessonRepository
    {
        private readonly EnglishAppDbContext _context;
        public LessonService(EnglishAppDbContext context)
        {
            _context = context;
        }
        public async Task<ApiResponse> AddNewLessonAsync(AddNewLessonDto dto)
        {
            var category = await _context.Categories.AsNoTracking().AnyAsync(x=>x.CategoryId == dto.CategoryId);
            if (!category)
            {
                return new ApiResponse
                {
                    Success = false,
                    Message = "Category chưa tồn tại"
                };
            }
            try
            {
                var lesson = new Lesson
                {
                    Title = dto.Title,
                    Descripton = dto.Descripton,
                    Level = dto.Level,
                    CategoryId = dto.CategoryId,
                    MediaUrl = dto.MediaUrl,
                    CreatedAt = DateTime.UtcNow,
                    LastUpdatedAt = DateTime.UtcNow,
                };
                await _context.Lessons.AddAsync(lesson);
                await _context.SaveChangesAsync();
                return new ApiResponse
                {
                    Success = true,
                    Message = "Tạo mới bài học thành công",
                    Data = lesson
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse
                {
                    Success = false,
                    Message = "Không thể thêm mới bài học: " + ex.Message,
                };
            }

    
        }

        public async Task<ApiResponse> GetAllLessonAsync()
        {
           return new ApiResponse { Success = true, Message = "Thành công", Data = await _context.Lessons.AsNoTracking().ToListAsync() };
        }
    }
}
