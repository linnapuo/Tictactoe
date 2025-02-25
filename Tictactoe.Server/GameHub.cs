using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Memory;
using Tictactoe.Engine;

namespace Tictactoe.Server;

public class GameHub(IMemoryCache cache) : Hub
{
    public async Task Create(Create create)
    {
        if (cache.TryGetValue(create.GameId, out Lobby? _))
        {
            throw new HubException("Game already exists");
        }

        var lobby = Funcs.CreateGame(create, Context.ConnectionId);

        cache.Set(create.GameId, lobby);

        await Groups.AddToGroupAsync(Context.ConnectionId, lobby.GameId);

        await SendGamestate(lobby);
    }

    public async Task Join(Join join)
    {
        if (!cache.TryGetValue(join.GameId, out Lobby? lobby))
        {
            throw new HubException("Game does not exist");
        }

        var result = Funcs.JoinGame(lobby!, Context.ConnectionId);

        if (result.IsError)
        {
            throw new HubException(result.ErrorValue);
        }

        lobby = result.ResultValue;

        cache.Set(join.GameId, lobby);

        await Groups.AddToGroupAsync(Context.ConnectionId, lobby.GameId);

        await SendGamestate(lobby);
    }

    public async Task Spectate(Spectate spectate)
    {
        if (!cache.TryGetValue(spectate.GameId, out Lobby? lobby))
        {
            throw new HubException("Game does not exist");
        }

        cache.Set(spectate.GameId, lobby);

        await Groups.AddToGroupAsync(Context.ConnectionId, lobby!.GameId);

        await SendGamestate(lobby);
    }

    public async Task Move(Move move)
    {
        if (!cache.TryGetValue(move.GameId, out Lobby? lobby))
        {
            throw new HubException("Game does not exist");
        }

        var result = Funcs.MakeMove(move, lobby!, Context.ConnectionId);

        if (result.IsError)
        {
            throw new HubException(result.ErrorValue);
        }

        lobby = result.ResultValue;

        cache.Set(move.GameId, lobby);

        await SendGamestate(lobby);
    }

    private async Task SendGamestate(Lobby lobby)
    {
        var gamestate = new GameState(lobby.GameId, lobby.Game.Squares, lobby.Players, lobby.Game.XIsNext);
        await Clients.Group(lobby.GameId).SendAsync(nameof(GameState), gamestate);
    }
}
