using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Tictactoe.Engine;

namespace Tictactoe.Server;

[ApiController]
[Route("[controller]")]
public class LobbiesController(IMemoryCache cache) : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
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
}
