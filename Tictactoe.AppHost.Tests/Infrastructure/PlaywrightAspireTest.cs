using Aspire.Hosting;
using Microsoft.Playwright;

namespace Tictactoe.AppHost.Tests.Infrastructure;

/// <summary>
/// Base class for Playwright tests, providing common functionality and setup for Playwright testing with ASP.NET Core.
/// </summary>
public abstract class PlaywrightAspireTest :
    IClassFixture<AspireFixture>,
    IAsyncLifetime
{
    protected PlaywrightAspireTest(
        AspireFixture aspireFixture,
        string[]? args = null,
        Action<IDistributedApplicationTestingBuilder>? configureBuilder = null)
    {
        _aspireFixture = aspireFixture ?? throw new ArgumentNullException(nameof(aspireFixture));
        _playwrightManager = aspireFixture.PlaywrightManager;
        _args = args;
        _configureBuilder = configureBuilder;
    }

    protected IBrowserContext BrowserContext { get; private set; } = default!;

    private DistributedApplication _app => _aspireFixture.App
        ?? throw new InvalidOperationException("Call ConfigureAsync first");

    private readonly AspireFixture _aspireFixture;
    private readonly PlaywrightManager _playwrightManager;
    private readonly string[]? _args;
    private readonly Action<IDistributedApplicationTestingBuilder>? _configureBuilder;

    private static readonly TimeSpan _defaultTimeout = TimeSpan.FromSeconds(30);

    private async Task<IBrowserContext> GetContextAsync()
    {
        var cancellationToken = new CancellationTokenSource(_defaultTimeout).Token;


        if (_app.GetEndpoint(Resources.ClientResourceName) is null)
            throw new InvalidOperationException($"Service '{Resources.ClientResourceName}' not found in the application endpoints");

        var uri = _app.GetEndpoint(Resources.ClientResourceName);

        await _app.ResourceNotifications
            .WaitForResourceHealthyAsync(Resources.ClientResourceName, cancellationToken)
            .WaitAsync(_defaultTimeout, cancellationToken);

        return await _playwrightManager.Browser.NewContextAsync(new BrowserNewContextOptions
        {
            IgnoreHTTPSErrors = true,
            ColorScheme = ColorScheme.Dark,
            BaseURL = uri.ToString()
        });
    }

    public async ValueTask InitializeAsync()
    {
        await _aspireFixture.ConfigureAsync<Projects.Tictactoe_AppHost>(_args, builder =>
        {
            _configureBuilder?.Invoke(builder);
        });
        BrowserContext = await GetContextAsync();
    }

    public async ValueTask DisposeAsync()
    {
        GC.SuppressFinalize(this);

        await BrowserContext.DisposeAsync();
    }
}
