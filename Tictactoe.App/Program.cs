using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.FluentUI.AspNetCore.Components;
using Tictactoe.App;
using Tictactoe.App.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

//builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddFluentUIComponents();

builder.Services.AddKeyedScoped("GameClient", (_, _) =>
{
    return new HubConnectionBuilder()
        .WithUrl("https://localhost:7138/gamehub")
        .ConfigureLogging(b => b.SetMinimumLevel(LogLevel.Debug))
        .Build();
});

builder.Services.AddScoped<GameClient>();
builder.Services.AddScoped<AppState>();

await builder.Build().RunAsync();
