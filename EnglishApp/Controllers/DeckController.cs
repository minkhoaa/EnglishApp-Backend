using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using EnglishApp.Data;
using EnglishApp.Dto.Request;
using EnglishApp.Dto.Response;
using EnglishApp.Repository;
using EnglishApp.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection.KeyManagement.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


[ApiController]
[Route("api/deck/{controller}")]
public class DeckController :  ControllerBase
{
    private readonly IDeckRepository _deckRepository;
    private readonly EnglishAppDbContext _context;

    public DeckController(IDeckRepository deckRepository, EnglishAppDbContext  context)
    {
        _deckRepository = deckRepository;
        _context = context;
    }

    [HttpGet("/api/getallpublicdeck")]
    public async Task<IActionResult> GetAllDecks()
    {
        try
        {
            return Ok(await _deckRepository.GetAllDecks());
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
        
    }

    [HttpGet("/api/deck/{id}")]
    public async Task<IActionResult> GetDeck(int id)
    {
        try
        {
            return Ok(await _deckRepository.GetDeckById(id));
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpPost("/api/adddeck")]
    public async Task<IActionResult> CreateDeck([FromBody] DeckDto Deck, int ownerId)
    {
        try
        {
            var result = await _deckRepository.AddNewDeck(dto: Deck,  ownerId: ownerId);
            return Ok(result);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpPut("/api/editdeck/{id}")]
    public async Task<IActionResult> UpdateDeck([FromBody] DeckDto Deck, int id)
    {
        var result = await _deckRepository.UpdateDeck(dto: Deck, id: id);
        return (result > 0) ? Ok(result) : NotFound();  
    }

    [HttpPost("/api/savedeck")]
    [Authorize]
    public async Task<IActionResult> SaveDeck(int deckID)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier) ?.Value;
        var existedDeck = await _context.FavoriteDecks.AsNoTracking()
            .FirstOrDefaultAsync(x=> x.UserId.ToString() == userId && x.DeckId == deckID );
        if (existedDeck == null)
        {
            await _context.FavoriteDecks.AddAsync(new FavoriteDeck()
            {
                UserId = int.Parse(userId!),
                DeckId = deckID,
                CreateTime =  DateTime.UtcNow
            });
            await _context.SaveChangesAsync();
            return Ok("Saved");
        }
        else
        {
            await _context.FavoriteDecks.AsNoTracking()
                .Where(x => x.UserId.ToString() == userId && x.DeckId == deckID).ExecuteDeleteAsync();
        }
        return Ok("Unsaved");
    }

    [HttpGet("/api/allsaveddecks")]

    public async Task<IActionResult> GetAllSavedDecks(int userId)
    {
      
        var result = await _context.FavoriteDecks.AsNoTracking()
            .Where(x=>x.UserId == userId)
            .Select(x=> new DeckResponse()
            {
                Id = x.DeckId, 
                Name = x.Deck.Name,
                FlashCardNumber = x.Deck.FlashCardNumber
            })
            .ToListAsync();
        
        return Ok(result);
    }

    [HttpGet("/api/getowndecks")]

    public async Task<IActionResult> GetAllOwnDecks(int ownerId)
    {
        var result = await _context.Decks.AsNoTracking()
            .Where(x => x.OwnerId == ownerId).ToListAsync();
        return Ok(result);
    }
    [HttpDelete("/api/deleteDeck/{id}")]
    public async Task<IActionResult> DeleteDeck(int id) {
        try
        {
            var result =  await _context.Decks.AsNoTracking().Where(x => x.Id == id).
                ExecuteDeleteAsync();
            return Ok(result); 
        }
        catch (Exception e)
        {
            return BadRequest(e.Message); 
        }
     
    }
}