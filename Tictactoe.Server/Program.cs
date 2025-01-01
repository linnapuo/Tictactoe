using Microsoft.AspNetCore.SignalR;
using Tictactoe.Server;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddOpenApi("v1");
builder.Services.AddCors();
builder.Services.AddSignalR(options =>
{
    options.AddFilter<ExceptionFilter>();
});
builder.Services.AddMemoryCache();
builder.Services.AddControllers();

var app = builder.Build();

app.MapDefaultEndpoints();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi("/openapi/v1.json");
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "v1");
    });
}

app.UseHttpsRedirection();

app.UseCors(builder =>
{
    builder.WithOrigins("http://localhost:3000")
        .AllowAnyHeader()
        .WithMethods("GET", "POST")
        .AllowCredentials();
});

app.MapHub<GameHub>("/gamehub");
app.MapHub<ChatHub>("/chathub");
app.MapControllers();

app.Run();
