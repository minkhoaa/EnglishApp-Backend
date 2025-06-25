using EnglishApp.Data;
using EnglishApp.Dto.Request;

namespace EnglishApp.Repository;

public interface IDeckRepository
{
    public Task<List<Deck>> GetAllDecks(); 
    public Task<Deck> GetDeckById(int id);
    public Task<Deck> AddNewDeck(DeckDto dto, int ownerId);
    public Task<int> UpdateDeck(DeckDto dto, int id);
    public Task<int> DeleteDeck(int id);
}