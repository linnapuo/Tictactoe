using Microsoft.AspNetCore.SignalR.Client;
using Tictactoe.Engine;

namespace TicTacToe.Web.Services;

public class GameClient([FromKeyedServices("GameClient")] HubConnection client, ILogger<GameClient> logger)
{
    public async Task EnsureStartedAsync()
    {
        if (client.State is not HubConnectionState.Disconnected)
        {
            return;
        }

        try
        {
            await client.StartAsync();
        }
        catch (HttpRequestException e)
        {
            logger.LogWarning(e, "GameClient");
        }
    }

    public IAsyncEnumerable<Gamestate> StreamGamestateAsync()
    {
        return client.StreamAsync<Gamestate>("GameState");
    }

    public async Task CreateAsync(string gameId)
    {
        await client.InvokeAsync("Create", new Create(gameId));
    }

    public async Task JoinAsync(string gameId)
    {
        await client.InvokeAsync("Join", new Join(gameId));
    }

    public async Task MoveAsync(string gameId, int square)
    {
        await client.InvokeAsync("Move", new Move(gameId, square));
    }
}
