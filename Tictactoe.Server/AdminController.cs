using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Tictactoe.Engine;

namespace Tictactoe.Server;

[ApiController]
public class AdminController(IMemoryCache cache) : ControllerBase
{
    [HttpGet("/admin/lobbies")]
    public IActionResult GetLobbies()
    {
        if (cache is not MemoryCache memoryCache)
        {
            return NoContent();
        }

        var lobbies = memoryCache.Keys
            .Where(key => key is string)
            .Select(memoryCache.Get)
            .OfType<Lobby>()
            .ToList();

        return Ok(lobbies);
    }
}
