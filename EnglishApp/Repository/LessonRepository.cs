using EnglishApp.Dto.Request;
using EnglishApp.Model;

namespace EnglishApp.Repository
{
    public interface LessonRepository
    {
        public Task<ApiResponse> AddNewLessonAsync(AddNewLessonDto dto);
        public Task<ApiResponse> GetAllLessonAsync();
    }
}
