using Microsoft.AspNetCore.Components;
using Tictactoe.App.Models;

namespace Tictactoe.App.Services;

public class AppState(
    NavigationManager navigationManager,
    GameClient client,
    ILogger<AppState> logger) : IDisposable
{
    private IDisposable? _getGamestate;

    public async Task InitAsync()
    {
        await client.EnsureStartedAsync();
        _getGamestate ??= client.GetGamestate(state =>
        {
            logger.LogInformation("State changed");
            Gamestate = state;
        });
    }

    public async Task GoToLobbyAsync(string code)
    {
        await client.CreateAsync(code);
        Code = code;
        navigationManager.NavigateTo("/lobby");
    }

    public async Task GoToGameAsync()
    {
        await Task.CompletedTask;
        navigationManager.NavigateTo("/game");
    }

    public void Dispose()
    {
        logger.LogInformation("Dispose StateManager");
        _getGamestate?.Dispose();
        GC.SuppressFinalize(this);
    }

    private string? _code;
    private Gamestate? _gamestate;

    public string? Code
    {
        get => _code;
        private set
        {
            _code = value;
            NotifyStateChanged();
        }
    }

    public Gamestate? Gamestate
    {
        get => _gamestate;
        private set
        {
            _gamestate = value;
            NotifyStateChanged();
        }
    }

    public event Action? OnChange;

    private void NotifyStateChanged() => OnChange?.Invoke();
}
