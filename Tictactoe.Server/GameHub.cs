using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Memory;
using Tictactoe.Engine;

namespace Tictactoe.Server;

public class GameHub(IMemoryCache cache) : Hub
{
    public async Task Create(Create create)
    {
        if (cache.TryGetValue(create.gameId, out Lobby? _))
        {
            throw new HubException("Game already exists");
        }

        var lobby = Funcs.CreateGame(create, Context.ConnectionId);

        cache.Set(create.gameId, lobby);

        await Groups.AddToGroupAsync(Context.ConnectionId, lobby.gameId);

        await SendGamestate(lobby);
    }

    public async Task Join(Join join)
    {
        if (!cache.TryGetValue(join.gameId, out Lobby? lobby))
        {
            throw new HubException("Game does not exist");
        }

        var result = Funcs.JoinGame(lobby!, Context.ConnectionId);

        if (result.IsError)
        {
            throw new HubException(result.ErrorValue);
        }

        lobby = result.ResultValue;

        cache.Set(join.gameId, lobby);

        await Groups.AddToGroupAsync(Context.ConnectionId, lobby.gameId);

        await SendGamestate(lobby);
    }

    public async Task Spectate(Spectate spectate)
    {
        if (!cache.TryGetValue(spectate.gameId, out Lobby? lobby))
        {
            throw new HubException("Game does not exist");
        }

        cache.Set(spectate.gameId, lobby);

        await Groups.AddToGroupAsync(Context.ConnectionId, lobby!.gameId);

        await SendGamestate(lobby);
    }

    public async Task Move(Move move)
    {
        if (!cache.TryGetValue(move.gameId, out Lobby? lobby))
        {
            throw new HubException("Game does not exist");
        }

        var result = Funcs.MakeMove(move, lobby!, Context.ConnectionId);

        if (result.IsError)
        {
            throw new HubException(result.ErrorValue);
        }

        lobby = result.ResultValue;

        cache.Set(move.gameId, lobby);

        await SendGamestate(lobby);
    }

    private async Task SendGamestate(Lobby lobby)
    {
        var gamestate = new Gamestate(lobby.gameId, lobby.game.squares, lobby.players, lobby.game.xIsNext);
        await Clients.Group(lobby.gameId).SendAsync(nameof(Gamestate), gamestate);
    }
}
