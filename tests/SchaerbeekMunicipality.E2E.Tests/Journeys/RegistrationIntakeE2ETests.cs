using FluentAssertions;
using Microsoft.Playwright;
using SchaerbeekMunicipality.Application.Auth;
using SchaerbeekMunicipality.E2E.Tests.Infrastructure;

namespace SchaerbeekMunicipality.E2E.Tests.Journeys;

[Collection(E2ECollection.Name)]
[Trait("Category", "E2E")]
public sealed class RegistrationIntakeE2ETests(MunicipalE2EFixture fixture) : E2ETestBase(fixture)
{
    [Fact]
    public async Task ReceptionOfficer_CreatesRegistrationCase_FromNewCasePage()
    {
        var page = await Fixture.NewPageAsOfficerAsync(CurrentOfficer.ReceptionOfficerId);

        await page.GotoAsync(DemoOfficerUrls.Reception(Fixture.BaseUri, "/registration/new-case"));
        await page.WaitForBlazorAsync();

        await page.GetByTestId("create-registration-case").ClickAsync();
        await page.GetByRole(AriaRole.Heading, new() { Name = "Case created" })
            .WaitForAsync(new LocatorWaitForOptions { Timeout = 30_000 });

        (await page.GetByText("The intake file is open").IsVisibleAsync()).Should().BeTrue();
    }
}
