using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Memory;
using Tictactoe.Engine;

namespace Tictactoe.Server;

public class GameHub : Hub
{
    private IMemoryCache Cache { get; }

    public GameHub(IMemoryCache cache)
    {
        Cache = cache;
    }

    public async Task Create(Create create)
    {
        if (Cache.TryGetValue(create.gameId, out Lobby _))
        {
            throw new HubException("Game already exists");
        }

        var lobby = Funcs.CreateGame(create, Context.ConnectionId);

        Cache.Set(create.gameId, lobby);

        await Groups.AddToGroupAsync(Context.ConnectionId, lobby.name);

        await SendGamestate(lobby);
    }

    public async Task Join(Join join)
    {
        if (!Cache.TryGetValue(join.gameId, out Lobby lobby))
        {
            throw new HubException("Game does not exist");
        }

        var result = Funcs.JoinGame(lobby, Context.ConnectionId);

        if (result.IsError)
        {
            throw new HubException(result.ErrorValue);
        }

        lobby = result.ResultValue;

        Cache.Set(join.gameId, lobby);

        await Groups.AddToGroupAsync(Context.ConnectionId, lobby.name);

        await SendGamestate(lobby);
    }

    public async Task Move(Move move)
    {
        if (!Cache.TryGetValue(move.gameId, out Lobby lobby))
        {
            throw new HubException("Game does not exist");
        }

        var result = Funcs.MakeMove(move, lobby, Context.ConnectionId);

        if (result.IsError)
        {
            throw new HubException(result.ErrorValue);
        }

        lobby = result.ResultValue;

        Cache.Set(move.gameId, lobby);

        await SendGamestate(lobby);
    }

    private async Task SendGamestate(Lobby lobby)
    {
        var gamestate = new Gamestate(lobby.name, lobby.game.squares, lobby.players, lobby.game.xIsNext);

        await Task.Delay(500);

        await Clients.Group(lobby.name).SendAsync(nameof(Gamestate), gamestate);
    }
}