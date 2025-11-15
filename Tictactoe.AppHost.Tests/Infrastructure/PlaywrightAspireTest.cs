using Aspire.Hosting;
using Microsoft.Playwright;

namespace Tictactoe.AppHost.Tests.Infrastructure;

/// <summary>
/// Base class for Playwright tests, providing common functionality and setup for Playwright testing with ASP.NET Core.
/// </summary>
public abstract class PlaywrightAspireTest :
    IClassFixture<AspireFixture>,
    IClassFixture<DatabaseSnapshot>,
    IAsyncLifetime
{
    protected PlaywrightAspireTest(
        AspireFixture aspireFixture,
        DatabaseSnapshot databaseSnapshot,
        string[]? args = null,
        Action<IDistributedApplicationTestingBuilder>? configureBuilder = null)
    {
        _aspireFixture = aspireFixture ?? throw new ArgumentNullException(nameof(aspireFixture));
        _playwrightManager = aspireFixture.PlaywrightManager;
        _databaseSnapshot = databaseSnapshot ?? throw new ArgumentNullException(nameof(databaseSnapshot));
        _args = args;
        _configureBuilder = configureBuilder;
    }

    protected IPage Page { get; private set; } = default!;

    private DistributedApplication _app => _aspireFixture.App
        ?? throw new InvalidOperationException("Call ConfigureAsync first");

    private readonly DatabaseSnapshot _databaseSnapshot;
    private readonly AspireFixture _aspireFixture;
    private readonly PlaywrightManager _playwrightManager;
    private readonly string[]? _args;
    private readonly Action<IDistributedApplicationTestingBuilder>? _configureBuilder;

    private static readonly TimeSpan _defaultTimeout = TimeSpan.FromSeconds(30);

    private IBrowserContext _context = default!;

    private async Task<IPage> GetPageAsync()
    {
        var cancellationToken = new CancellationTokenSource(_defaultTimeout).Token;

        if (_app.GetEndpoint(Resources.PageResourceName) is null)
            throw new InvalidOperationException($"Service '{Resources.PageResourceName}' not found in the application endpoints");

        var uri = _app.GetEndpoint(Resources.PageResourceName);

        await _app.ResourceNotifications
            .WaitForResourceHealthyAsync(Resources.PageResourceName, cancellationToken)
            .WaitAsync(_defaultTimeout, cancellationToken);

        _context = await _playwrightManager.Browser.NewContextAsync(new BrowserNewContextOptions
        {
            IgnoreHTTPSErrors = true,
            ColorScheme = ColorScheme.Dark,
            BaseURL = uri.ToString()
        });

        return await _context.NewPageAsync();
    }

    public async ValueTask InitializeAsync()
    {
        await _aspireFixture.ConfigureAsync<Projects.Tictactoe_AppHost>(_args, builder =>
        {
            var db = builder.Resources.First(r => r.Name == Resources.DbResourceName);
            var rms = builder.Resources.First(r => r.Name == Resources.PageResourceName);

            rms.Annotations.Remove(rms.Annotations.OfType<LaunchProfileAnnotation>().First());
            rms.Annotations.Add(new LaunchProfileAnnotation("Kestrel"));

            _configureBuilder?.Invoke(builder);
        });
        Page = await GetPageAsync();
    }

    public async ValueTask DisposeAsync()
    {
        GC.SuppressFinalize(this);

        await Page.CloseAsync();
        await _context.DisposeAsync();
        await _databaseSnapshot.RestoreSnapshotAsync();
    }
}
