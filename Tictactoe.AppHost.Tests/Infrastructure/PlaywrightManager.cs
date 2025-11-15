using Microsoft.Playwright;
using System.Diagnostics;

namespace Tictactoe.AppHost.Tests.Infrastructure;

/// <summary>
/// Configure Playwright for interacting with the browser in tests.
/// </summary>
public class PlaywrightManager : IAsyncLifetime
{
    private static bool _isDebugging => Debugger.IsAttached;
    private static bool _isHeadless => !_isDebugging;

    private IPlaywright? _playwright;

    internal IBrowser Browser { get; set; } = default!;

    public async ValueTask InitializeAsync()
    {
        Assertions.SetDefaultExpectTimeout(10_000);

        _playwright = await Playwright.CreateAsync();

        var options = new BrowserTypeLaunchOptions
        {
            Headless = _isHeadless
        };

        Browser = await _playwright.Chromium.LaunchAsync(options).ConfigureAwait(false);
    }

    public async ValueTask DisposeAsync()
    {
        GC.SuppressFinalize(this);

        await Browser.CloseAsync();
        _playwright?.Dispose();
    }
}