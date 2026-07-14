using Microsoft.Playwright;

namespace SchaerbeekMunicipality.E2E.Tests.Infrastructure;

internal static class PlaywrightPageExtensions
{
    internal static async Task WaitForBlazorAsync(this IPage page)
    {
        await page.WaitForSelectorAsync("[data-testid='app-shell']", new PageWaitForSelectorOptions
        {
            Timeout = 30_000
        });

        await page.WaitForLoadStateAsync(LoadState.NetworkIdle);
    }

    internal static async Task ClickAndWaitForTextAsync(
        this IPage page,
        string testId,
        string expectedText,
        int timeoutMs = 30_000)
    {
        await page.GetByTestId(testId).ClickAsync();
        await page.GetByText(expectedText).WaitForAsync(new LocatorWaitForOptions
        {
            Timeout = timeoutMs
        });
    }
}