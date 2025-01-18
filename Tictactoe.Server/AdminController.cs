using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Tictactoe.Engine;

namespace Tictactoe.Server;

[ApiController]
[Route("[controller]/[action]")]
public class AdminController(IMemoryCache cache) : ControllerBase
{
    [HttpGet]
    public IActionResult Lobbies()
    {
        if (cache is not MemoryCache memoryCache)
        {
            return NoContent();
        }

        var lobbies = memoryCache.Keys
            .Where(key => key is string)
            .Select(memoryCache.Get)
            .OfType<Lobby>()
            .Select(x => new { x.gameId })
            .ToList();

        return Ok(lobbies);
    }

    [HttpDelete("{gameId}")]
    public IActionResult Lobbies(string gameId)
    {
        if (cache is not MemoryCache memoryCache)
        {
            return NoContent();
        }

        memoryCache.Remove(gameId);
        return Ok();
    }
}
