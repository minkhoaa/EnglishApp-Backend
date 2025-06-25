using EnglishApp.Data;
using EnglishApp.Dto.Request;

namespace EnglishApp.Repository;

public interface IFlashCardRepository
{
    Task<List<FlashCard>> GetAllAsync();
    Task<List<FlashCard>> AddAsync(List<FlashCardDto> dto); 
    Task<int> EditAsync(FlashCardDto dto, int id);
    Task<int> DeleteAsync(int id);
}




