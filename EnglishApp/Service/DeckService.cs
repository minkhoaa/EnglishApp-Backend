using EnglishApp.Data;
using EnglishApp.Dto.Request;
using EnglishApp.Repository;
using Microsoft.EntityFrameworkCore;

namespace EnglishApp.Service;

public class DeckService:IDeckRepository
{
    private readonly EnglishAppDbContext _context;

    public DeckService(EnglishAppDbContext context)
    {
        _context = context;
    }
    public async Task<List<Deck>> GetAllDecks()
    {
        var result = await _context.Decks.AsNoTracking().ToListAsync();
        return result;
    }

    public async Task<Deck> GetDeckById(int id)
    {
        var result = await _context.Decks.AsNoTracking().FirstOrDefaultAsync(x=>x.Id == id);
        return result!;
    }

    public async Task<Deck> AddNewDeck(DeckDto dto)
    {
        var newDeck = new Deck()
        {
            Name = dto.Name,
            FlashCardNumber = dto.FlashCardNumber,
        };
        _context.Decks.Add(newDeck);
        await _context.SaveChangesAsync();
        return newDeck; 
    }

    public Task<int> UpdateDeck(DeckDto dto, int id)
    {
        var existingDeck = _context.Decks.AsNoTracking()
            .Where(x => x.Id == id)
            .ExecuteUpdateAsync(setter => setter
                .SetProperty(x => x.FlashCardNumber, dto.FlashCardNumber)
                .SetProperty(x => x.Name, dto.Name)
            );
        return existingDeck;
    }

    public Task<int> DeleteDeck(int id)
    {
        {
            var existingDeck = _context.Decks.AsNoTracking()
                .Where(x => x.Id == id)
                .ExecuteDeleteAsync();
            return existingDeck;
        }
    }
}