using FluentAssertions;
using Microsoft.Playwright;
using SchaerbeekMunicipality.Application.Auth;
using SchaerbeekMunicipality.E2E.Tests.Infrastructure;
using SchaerbeekMunicipality.E2E.Tests.Seeding;

namespace SchaerbeekMunicipality.E2E.Tests.Journeys;

[Collection(E2ECollection.Name)]
[Trait("Category", "E2E")]
public sealed class RegistrationConfirmE2ETests(MunicipalE2EFixture fixture) : E2ETestBase(fixture)
{
    [Fact]
    public async Task PopulationOfficer_ConfirmsApprovedRegistration_FromCaseDetail()
    {
        var caseId = await E2ECaseSeeder.SeedRegistrationCaseApprovedForConfirmationAsync(Fixture.ApiFactory);

        var page = await Fixture.NewPageAsOfficerAsync(CurrentOfficer.PopulationOfficerId);
        await page.GotoAsync(
            DemoOfficerUrls.Population(
                Fixture.BaseUri,
                $"/registration/cases/{caseId}"));
        await page.WaitForBlazorAsync();

        await page.GetByTestId("confirm-registration").ClickAsync();

        await page.GetByRole(AriaRole.Alert).Filter(new LocatorFilterOptions { HasText = "Person entered in" })
            .WaitForAsync(new LocatorWaitForOptions { Timeout = 30_000 });
        (await page.GetByRole(AriaRole.Alert).Filter(new LocatorFilterOptions { HasText = "Person entered in" })
                .IsVisibleAsync())
            .Should().BeTrue();
    }
}