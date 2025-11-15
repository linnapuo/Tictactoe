using Microsoft.Playwright;
using Tictactoe.AppHost.Tests.Infrastructure;

namespace Tictactoe.AppHost.Tests;

public class PageTest(AspireFixture aspireFixture, string[]? args = null)
    : PlaywrightAspireTest(aspireFixture, args, configureBuilder: builder =>
    {
    })
{
    [Fact]
    public async Task Test()
    {
        var page1 = await BrowserContext.NewPageAsync();
        var page2 = await BrowserContext.NewPageAsync();
        await page1.GotoAsync("/");
        await page2.GotoAsync("/");

        await Task.Delay(10000, TestContext.Current.CancellationToken);

        await page1.GetByRole(AriaRole.Button, new() { Name = "Create" }).ClickAsync();
        await page1.GetByRole(AriaRole.Textbox, new() { Name = "Code" }).FillAsync("game1");
        await page1.GetByRole(AriaRole.Button, new() { Name = "ENTER" }).ClickAsync();

        await page2.GetByRole(AriaRole.Button, new() { Name = "Join" }).ClickAsync();
        await page2.GetByRole(AriaRole.Textbox, new() { Name = "Code" }).FillAsync("game1");
        await page2.GetByRole(AriaRole.Button, new() { Name = "ENTER" }).ClickAsync();

    }
}
