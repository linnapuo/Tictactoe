namespace Tictactoe.Server;

public class ApiKeyMiddleware(string apiKey) : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        if (context.Request.Method.Equals("POST", StringComparison.OrdinalIgnoreCase) &&
           (context.Request.Path.StartsWithSegments("/chathub/negotiate") ||
            context.Request.Path.StartsWithSegments("/gamehub/negotiate")))
        {
            var apiKeyHeader = context.Request.Headers["X-Api-Key"];
            if (apiKeyHeader != apiKey)
            {
                context.Response.StatusCode = 403;
                await context.Response.WriteAsync("Invalid apikey");
                return;
            }
        }

        await next.Invoke(context);
    }
}
