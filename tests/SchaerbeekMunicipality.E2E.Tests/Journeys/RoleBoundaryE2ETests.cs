using FluentAssertions;
using Microsoft.Playwright;
using SchaerbeekMunicipality.Application.Auth;
using SchaerbeekMunicipality.E2E.Tests.Infrastructure;

namespace SchaerbeekMunicipality.E2E.Tests.Journeys;

[Collection(E2ECollection.Name)]
[Trait("Category", "E2E")]
public sealed class RoleBoundaryE2ETests(MunicipalE2EFixture fixture) : E2ETestBase(fixture)
{
    [Fact]
    public async Task ReceptionOfficer_CannotAccessRegistrationCaseList_IsRedirectedToNewCasePage()
    {
        var page = await Fixture.NewPageAsOfficerAsync(CurrentOfficer.ReceptionOfficerId);
        await page.GotoAsync(DemoOfficerUrls.Reception(Fixture.BaseUri, "/registration/cases"));
        await page.WaitForBlazorAsync();

        await page.WaitForURLAsync("**/registration/new-case**", new PageWaitForURLOptions { Timeout = 30_000 });
        (await page.GetByTestId("create-registration-case").IsVisibleAsync()).Should().BeTrue();
    }

    [Fact]
    public async Task PoliceClerk_CannotOpenRegistrationCase_SeesAccessRestricted()
    {
        var page = await Fixture.NewPageAsOfficerAsync(CurrentOfficer.PoliceClerkId);
        await page.GotoAsync(
            DemoOfficerUrls.WithOfficer(
                Fixture.BaseUri,
                "/registration/new-case",
                CurrentOfficer.PoliceClerkId));
        await page.WaitForBlazorAsync();

        await page.GetByText("Access restricted").WaitForAsync(new LocatorWaitForOptions { Timeout = 30_000 });
    }
}
