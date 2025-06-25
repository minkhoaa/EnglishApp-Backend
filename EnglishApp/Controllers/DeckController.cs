using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EnglishApp.Data;
using EnglishApp.Dto.Request;
using EnglishApp.Repository;
using EnglishApp.Service;
using Microsoft.AspNetCore.Mvc;


[ApiController]
[Route("api/deck/{controller}")]
public class DeckController :  ControllerBase
{
    private readonly IDeckRepository _deckRepository;

    public DeckController(IDeckRepository deckRepository)
    {
        _deckRepository = deckRepository;
    }

    [HttpGet("/api/getall")]
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
    public async Task<IActionResult> CreateDeck([FromBody] DeckDto Deck)
    {
        try
        {
            var result = await _deckRepository.AddNewDeck(dto: Deck);
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

    [HttpDelete("api/deletedeck/{id}")]
    public async Task<IActionResult> DeleteDeck(int id)
    {
        var result = await _deckRepository.DeleteDeck(id);
        return (result > 0) ? Ok(result) : NotFound();
    }
}