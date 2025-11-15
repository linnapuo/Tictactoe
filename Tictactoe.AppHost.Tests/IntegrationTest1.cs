using Microsoft.Extensions.Logging;
using Tictactoe.AppHost.Tests.Infrastructure;

namespace Tictactoe.AppHost.Tests;

public class IntegrationTest1
{
    private static readonly TimeSpan _defaultTimeout = TimeSpan.FromSeconds(30);

    [Fact(Timeout = 180_000)]
    public async Task GetWebResourceRootReturnsOkStatusCode()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var appHost = await DistributedApplicationTestingBuilder.CreateAsync<Projects.Tictactoe_AppHost>(cancellationToken);
        appHost.Services.AddLogging(logging =>
        {
            logging.SetMinimumLevel(LogLevel.Debug);
            // Override the logging filters from the app's configuration
            logging.AddFilter(appHost.Environment.ApplicationName, LogLevel.Debug);
            logging.AddFilter("Aspire.", LogLevel.Debug);
            // To output logs to the xUnit.net ITestOutputHelper, consider adding a package from https://www.nuget.org/packages?q=xunit+logging
        });
        appHost.Services.ConfigureHttpClientDefaults(clientBuilder =>
        {
            clientBuilder.AddStandardResilienceHandler();
            clientBuilder.ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
            {
                UseDefaultCredentials = true
            });
        });

        await using var app = await appHost.BuildAsync(cancellationToken).WaitAsync(_defaultTimeout, cancellationToken);
        await app.StartAsync(cancellationToken).WaitAsync(_defaultTimeout, cancellationToken);

        var httpClient = app.CreateHttpClient(Resources.PageResourceName, "http");
        await app.ResourceNotifications.WaitForResourceHealthyAsync(Resources.PageResourceName, cancellationToken).WaitAsync(_defaultTimeout, cancellationToken);
        var response = await httpClient.GetAsync("/", cancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
