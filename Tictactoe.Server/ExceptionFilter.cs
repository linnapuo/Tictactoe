using Microsoft.AspNetCore.SignalR;

namespace Tictactoe.Server;

public class ExceptionFilter : IHubFilter
{
    public ValueTask<object?> InvokeMethodAsync(HubInvocationContext invocationContext, Func<HubInvocationContext, ValueTask<object?>> next)
    {
        try
        {
            return next(invocationContext);
        }
        catch (Exception e) when (e is not HubException)
        {
            throw new HubException("Unknown error");
        }
    }
}
