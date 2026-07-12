using FluentAssertions;
using Microsoft.Playwright;
using SchaerbeekMunicipality.Application.Auth;
using SchaerbeekMunicipality.E2E.Tests.Infrastructure;
using SchaerbeekMunicipality.E2E.Tests.Seeding;

namespace SchaerbeekMunicipality.E2E.Tests.Journeys;

[Collection(E2ECollection.Name)]
[Trait("Category", "E2E")]
public sealed class DocumentRequestE2ETests(MunicipalE2EFixture fixture) : E2ETestBase(fixture)
{
    [Fact]
    public async Task PopulationOfficer_OpensDocumentRequest_FromNationalRegisterSearch()
    {
        await E2ECaseSeeder.EnsureJeanDupontRegisteredAsync(Fixture.ApiFactory);

        var page = await Fixture.NewPageAsOfficerAsync(CurrentOfficer.PopulationOfficerId);
        await page.GotoAsync(DemoOfficerUrls.Population(Fixture.BaseUri, "/identity-documents/requests"));
        await page.WaitForBlazorAsync();

        await page.GetByTestId("new-document-request").ClickAsync();
        await page.GetByRole(AriaRole.Dialog).WaitForAsync(new LocatorWaitForOptions { Timeout = 30_000 });

        await page.GetByTestId("nr-search-family-name").FillAsync("Dupont");
        await page.GetByTestId("nr-search-submit").ClickAsync();

        var openCaseButton = page.GetByRole(AriaRole.Row)
            .Filter(new() { HasText = "Jean" })
            .GetByTestId("open-document-request-case");
        await openCaseButton.WaitForAsync(new LocatorWaitForOptions { Timeout = 30_000 });
        await openCaseButton.ClickAsync();

        await page.WaitForURLAsync("**/identity-documents/requests/**", new PageWaitForURLOptions { Timeout = 30_000 });
        await page.GetByRole(AriaRole.Heading, new() { Name = "Identity document request" })
            .WaitForAsync(new LocatorWaitForOptions { Timeout = 30_000 });
        (await page.GetByRole(AriaRole.Heading, new() { Name = "Identity document request" }).IsVisibleAsync())
            .Should().BeTrue();
    }
}
