using Microsoft.Playwright;
using SchaerbeekMunicipality.Integration.Tests;

namespace SchaerbeekMunicipality.E2E.Tests.Infrastructure;

public sealed class MunicipalE2EFixture : IAsyncLifetime
{
    public MunicipalAppFactory ApiFactory { get; private set; } = null!;

    internal MunicipalWebHost WebHost { get; private set; } = null!;

    public IPlaywright Playwright { get; private set; } = null!;

    public IBrowser Browser { get; private set; } = null!;

    public Uri BaseUri { get; private set; } = null!;

    public async Task InitializeAsync()
    {
        ApiFactory = new MunicipalAppFactory();
        _ = ApiFactory.CreateClient();

        WebHost = new MunicipalWebHost();
        await WebHost.StartAsync(ApiFactory);
        BaseUri = WebHost.BaseUri;

        Playwright = await Microsoft.Playwright.Playwright.CreateAsync();
        Browser = await Playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = true,
        });
    }

    public async Task DisposeAsync()
    {
        await Browser.DisposeAsync();
        Playwright.Dispose();
        await WebHost.DisposeAsync();
        await ApiFactory.DisposeAsync();
    }

    public async Task<IPage> NewPageAsOfficerAsync(Guid officerId)
    {
        var page = await Browser.NewPageAsync();
        await page.GotoAsync(DemoOfficerUrls.WithOfficer(BaseUri, "/", officerId));
        await page.WaitForSelectorAsync("[data-testid='app-shell']", new PageWaitForSelectorOptions
        {
            Timeout = 30_000,
        });

        return page;
    }
}
