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

    public async Task<List<FlashCard>> AddAsync(List<FlashCardDto> dto)
    {
        var entities = dto.Select(x => new FlashCard
        {
            FrontText = x.FrontText,
            BackText = x.BackText,
            CreatedAt = DateTime.UtcNow,
            LastReviewedAt = DateTime.Now,
        }).ToList();
        _context.FlashCards.AddRange(entities);
        await _context.SaveChangesAsync();
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

    public Task<int> DeleteAsync( int id)
    {
        var result = _context.FlashCards.AsNoTracking()
            .Where(x => x.FlashcardId == id)
            .ExecuteDeleteAsync();
        return result; 
    }
}