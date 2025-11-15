using Aspire.Hosting;

namespace Tictactoe.AppHost.Tests.Infrastructure;

/// <summary>
/// Startup and configure the Aspire application for testing.
/// </summary>
public class AspireFixture : IAsyncLifetime
{
    internal PlaywrightManager PlaywrightManager { get; } = new();

    internal DistributedApplication? App { get; private set; }

    public async Task<DistributedApplication> ConfigureAsync<TEntryPoint>(
            string[]? args = null,
            Action<IDistributedApplicationTestingBuilder>? configureBuilder = null) where TEntryPoint : class
    {
        if (App is not null)
            return App;

        var builder = await DistributedApplicationTestingBuilder.CreateAsync<TEntryPoint>(args ?? []);

        configureBuilder?.Invoke(builder);

        App = await builder.BuildAsync();

        await App.StartAsync();

        return App;
    }

    public async ValueTask InitializeAsync()
    {
        await PlaywrightManager.InitializeAsync();
    }

    public async ValueTask DisposeAsync()
    {
        GC.SuppressFinalize(this);

        await PlaywrightManager.DisposeAsync();

        if (App is not null)
        {
            await App.DisposeAsync();
        }
    }
}
