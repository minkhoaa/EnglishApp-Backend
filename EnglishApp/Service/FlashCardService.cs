using EnglishApp.Data;
using EnglishApp.Dto.Request;
using EnglishApp.Repository;
using Microsoft.EntityFrameworkCore;

namespace EnglishApp.Service;

public class FlashCardService : IFlashCardRepository
{
    private readonly EnglishAppDbContext _context;

    public FlashCardService(EnglishAppDbContext context)
    {
        _context = context;
    }


    public async Task<List<FlashCard>> GetAllAsync()
    {
        var result = await _context.FlashCards.AsNoTracking().ToListAsync();
        return result; 
    }

    public async Task<List<FlashCard>> AddAsync(List<FlashCardDto> dto, int deckId)
    {
        var entities = dto.Select(x => new FlashCard
        {
            FrontText = x.FrontText,
            BackText = x.BackText,
            CreatedAt = DateTime.UtcNow,
            LastReviewedAt = DateTime.UtcNow,
            DeckId = deckId,
        }).ToList();
        
        _context.FlashCards.AddRange(entities);
        int effected = await _context.SaveChangesAsync();
        var deck = await _context.Decks.AsNoTracking().Where(x=>x.Id == deckId)
            .ExecuteUpdateAsync(setter => 
                setter.SetProperty(x=>x.FlashCardNumber, x=>x.FlashCardNumber + effected));
        return entities.ToList();
    }

    public async Task<int> EditAsync(FlashCardDto dto, int id)
    {
        var result = await _context.FlashCards.AsNoTracking()
            .Where(x => x.FlashcardId == id)
            .ExecuteUpdateAsync(setter => setter
                .SetProperty(x => x.FrontText, dto.FrontText)
                .SetProperty(x => x.BackText, dto.BackText)
                .SetProperty(x => x.LastReviewedAt, DateTime.UtcNow)
            );
        return result;
    }

    public async Task<int> DeleteAsync( int id)

    {
        try
        {
            var result = await _context.FlashCards.AsNoTracking().FirstOrDefaultAsync(x => x.FlashcardId == id);
            if (result != null)
            {
                await _context.Decks.Where(x => x.Id == result!.DeckId).ExecuteUpdateAsync(setter => setter
                    .SetProperty(a => a.FlashCardNumber, a => a.FlashCardNumber -1 )
                );
                await _context.SaveChangesAsync();
                return await _context.FlashCards.AsNoTracking().Where(x => x.FlashcardId == id).ExecuteDeleteAsync(); ;
            }
            throw new Exception("FlashCard not found"); 
        }
        catch (Exception a)
        {
            Console.WriteLine(a.Message);
            throw;
        }
    }
}