using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.FluentUI.AspNetCore.Components;
using Tictactoe.Web.Services;
using Tictactoe.Web.Components;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddFluentUIComponents();

builder.Services.AddKeyedScoped("GameClient", (_, _) =>
{
    return new HubConnectionBuilder()
        .WithUrl("https://localhost:7138/gamehub")
        .ConfigureLogging(configure => configure.AddConsole())
        .Build();
});
builder.Services.AddScoped<GameClient>();
builder.Services.AddScoped<GamestateStore>();

builder.Services.AddKeyedScoped("ChatClient", (_, _) =>
{
    return new HubConnectionBuilder()
        .WithUrl("https://localhost:7138/chathub")
        .Build();
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
