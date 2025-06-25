using EnglishApp.Dto.Request;
using EnglishApp.Repository;
using EnglishApp.Service;
using Microsoft.AspNetCore.Mvc;
namespace EnglishApp.Controllers;
[ApiController]
[Route("api/flashcard")]

public class FlashCardController : ControllerBase
{
    
    private readonly IFlashCardRepository _service;

    public FlashCardController(IFlashCardRepository service)
    {
        _service = service;
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
    public async Task<IActionResult> AddFlashCard(List<FlashCardDto> flashCardDtos)
    {
        try
        {
            var result = await _service.AddAsync(flashCardDtos);
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
}