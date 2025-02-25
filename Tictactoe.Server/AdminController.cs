using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Tictactoe.Engine;

namespace Tictactoe.Server;

[ApiController]
[Route("[controller]/[action]")]
public class AdminController(IMemoryCache cache) : ControllerBase
{
    [HttpGet]
    [Authorize]
    public IActionResult Lobbies()
    {
        var request = HttpContext.User;

        if (cache is not MemoryCache memoryCache)
        {
            return NoContent();
        }

        var lobbies = memoryCache.Keys
            .Where(key => key is string)
            .Select(memoryCache.Get)
            .OfType<Lobby>()
            .Select(x => new { x.GameId })
            .ToList();

        return Ok(lobbies);
    }

    [HttpDelete("{id}")]
    [Authorize]
    public IActionResult Lobbies(string id)
    {
        if (cache is not MemoryCache memoryCache)
        {
            return NoContent();
        }

        memoryCache.Remove(id);
        return Ok();
    }
}
