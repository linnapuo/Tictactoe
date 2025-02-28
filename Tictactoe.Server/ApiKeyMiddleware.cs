namespace Tictactoe.Server;

public class ApiKeyMiddleware(string apiKey) : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var apiKeyCookie = context.Request.Cookies["api_key"];

        if (!context.Request.Method.Equals("OPTIONS") && apiKeyCookie != apiKey)
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            context.Response.Headers.ContentType = "application/json";
            await context.Response.WriteAsync("{ \"error\": \"Invalid api_key\" }");
            await context.Response.CompleteAsync();
            return;
        }

        await next.Invoke(context);
    }
}
