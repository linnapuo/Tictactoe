using Microsoft.AspNetCore.SignalR;

namespace Tictactoe.Server;

public record ChatMessage(string Name, string Message);

public class ChatHub : Hub
{
    public async Task Send(string message)
    {
        await Clients.Others.SendAsync("Receive", new ChatMessage(Context.ConnectionId, message));
    }
}
