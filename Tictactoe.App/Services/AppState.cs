using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR;
using Microsoft.FluentUI.AspNetCore.Components;
using Tictactoe.App.Models;

namespace Tictactoe.App.Services;

public class AppState : IDisposable
{
    private string? _code;
    private Gamestate? _gamestate;

    private readonly NavigationManager _navigationManager;
    private readonly IToastService _toastService;
    private readonly GameClient _client;
    private readonly ILogger<AppState> _logger;

    private readonly IDisposable? _getGamestate;

    public AppState(
        NavigationManager navigationManager,
        IToastService toastService,
        GameClient client,
        ILogger<AppState> logger)
    {
        _navigationManager = navigationManager;
        _toastService = toastService;
        _client = client;
        _logger = logger;

        _getGamestate = _client.GetGamestate(state =>
        {
            Gamestate = state;
        });
    }

    public async Task InitAsync()
    {
        await _client.EnsureStartedAsync();
    }

    public async Task CreateLobbyAsync(string code)
    {
        try
        {
            await _client.CreateAsync(code);
            Code = code;
            _navigationManager.NavigateTo("/lobby");
        }
        catch (HubException e)
        {
            _toastService.ShowError(e.Message);
            NotifyStateChanged();
        }
    }

    public async Task JoinLobbyAsync(string code)
    {
        try
        {
            await _client.JoinAsync(code);
            Code = code;
            _navigationManager.NavigateTo("/lobby");
        }
        catch (HubException e)
        {
            _toastService.ShowError(e.Message);
            NotifyStateChanged();
        }
    }

    public Task GoToGameAsync()
    {
        _navigationManager.NavigateTo("/game");
        return Task.CompletedTask;
    }

    public async Task MoveAsync(string code, int square)
    {
        try
        {
            await _client.MoveAsync(code, square);
        }
        catch (HubException e)
        {
            _toastService.ShowError(e.Message);
            NotifyStateChanged();
        }
    }

    public void Dispose()
    {
        _logger.LogInformation("Dispose StateManager");
        _getGamestate?.Dispose();
        GC.SuppressFinalize(this);
    }

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

    public string? ConnectionId => _client.ConnectionId;

    public event Action? OnChange;

    private void NotifyStateChanged() => OnChange?.Invoke();
}
