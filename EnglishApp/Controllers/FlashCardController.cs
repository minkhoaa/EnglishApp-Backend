using EnglishApp.Data;
using EnglishApp.Dto.Request;
using EnglishApp.Dto.Response;
using EnglishApp.Repository;
using EnglishApp.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EnglishApp.Controllers;
[ApiController]
[Route("api/flashcard")]

public class FlashCardController : ControllerBase
{
    
    private readonly IFlashCardRepository _service;
    private readonly EnglishAppDbContext _context;
    public FlashCardController(IFlashCardRepository service,  EnglishAppDbContext context)
    {
        _service = service;
        _context = context;
    }

    [HttpGet("/api/getallflashcards")]
    public async Task<IActionResult> GetAllAsync()
    {
        try
        {
            var result = await _service.GetAllAsync();
            return Ok(result);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
       
    }

    [HttpPost("/api/addflashcard")]
    public async Task<IActionResult> AddFlashCard(List<FlashCardDto> flashCardDtos, int deckId)
    {
        try
        {
            var result = await _service.AddAsync(flashCardDtos, deckId);
            return Ok(result);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpPut("/api/updateflashcard/{id}")]
        public async Task<IActionResult> UpdateAsync(FlashCardDto flashCardDto, int id)
        {
            var result = await _service.EditAsync(flashCardDto,id);
            
            return (result > 0) ?  Ok(result) : BadRequest();
        }
        
    [HttpDelete("/api/deleteflashcard/{id}")]
    public async Task<IActionResult> DeleteAsync(int id)
    {
        var result = await _service.DeleteAsync(id);
            
        return (result > 0) ?  Ok(result) : BadRequest();
    }

    [HttpGet("/api/getflashcardbyiddeck/{idDeck}")]
    public async Task<IActionResult> GetDeckByIdAsync(int idDeck)
    {
        var result = await _context.FlashCards.AsNoTracking()
            .Where(x => x.DeckId == idDeck)
            .Select(x => new FlashCardResponse()
            {
                FlashcardId = x.FlashcardId,
                FrontText = x.FrontText,
                BackText = x.BackText
            })
            .ToListAsync();
        return Ok(result);
    }
    

}