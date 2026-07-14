using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Playwright;
using SchaerbeekMunicipality.Application.Auth;
using SchaerbeekMunicipality.Application.Features.BirthDeclaration.GetBirthDeclarationCase;
using SchaerbeekMunicipality.Domain.BirthDeclaration;
using SchaerbeekMunicipality.E2E.Tests.Infrastructure;
using SchaerbeekMunicipality.Integration.Tests.Features.BirthDeclaration;
using SchaerbeekMunicipality.Integration.Tests.Features.Registration;

namespace SchaerbeekMunicipality.E2E.Tests.Journeys;

[Collection(E2ECollection.Name)]
[Trait("Category", "E2E")]
public sealed class BirthDeclarationE2ETests(MunicipalE2EFixture fixture) : E2ETestBase(fixture)
{
    [Fact]
    public async Task ReceptionOfficer_OpensBirthDeclarationCase_FromNewCasePage()
    {
        var page = await Fixture.NewPageAsOfficerAsync(CurrentOfficer.ReceptionOfficerId);
        await page.GotoAsync(DemoOfficerUrls.Reception(Fixture.BaseUri, "/registration/new-case"));
        await page.WaitForBlazorAsync();

        await page.GetByText("First registration").ClickAsync();
        await page.Locator(".mud-popover-open .mud-list-item")
            .Filter(new LocatorFilterOptions { HasText = "Birth declaration" })
            .ClickAsync();
        await page.GetByTestId("create-registration-case").ClickAsync();
        await page.GetByRole(AriaRole.Heading, new PageGetByRoleOptions { Name = "Case created" })
            .WaitForAsync(new LocatorWaitForOptions { Timeout = 30_000 });
        (await page.GetByText("The intake file is open").IsVisibleAsync()).Should().BeTrue();
    }

    [Fact]
    public async Task PopulationOfficer_ConfirmsBirthDeclaration_FromCaseDetail()
    {
        await using var scope = Fixture.ApiFactory.Services.CreateAsyncScope();
        RegistrationTestHelpers.SetRole(scope.ServiceProvider, OfficerRole.PopulationOfficer);
        var caseId = await BirthDeclarationTestHelpers.PrepareCaseReadyForConfirmationAsync(scope.ServiceProvider);
        var detail = await scope.ServiceProvider.GetRequiredService<GetBirthDeclarationCaseHandler>()
            .Handle(caseId, CancellationToken.None);
        detail.Should().NotBeNull();
        detail.Status.Should().Be(BirthDeclarationCaseStatus.UnderReview);
        detail.ReadyForConfirmation.Should().BeTrue();
        detail.CanEdit.Should().BeTrue();

        var page = await Fixture.NewPageAsOfficerAsync(CurrentOfficer.PopulationOfficerId);
        await page.GotoAsync(
            DemoOfficerUrls.Population(
                Fixture.BaseUri,
                $"/birth-declarations/{caseId.Value}"));
        await page.WaitForBlazorAsync();
        await page.GetByText("Officer decision").WaitForAsync(new LocatorWaitForOptions { Timeout = 30_000 });

        var confirmButton =
            page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Confirm birth declaration" });
        await confirmButton.WaitForAsync(new LocatorWaitForOptions
            { State = WaitForSelectorState.Visible, Timeout = 30_000 });
        await confirmButton.ClickAsync(new LocatorClickOptions { Timeout = 30_000 });

        await page.GetByRole(AriaRole.Alert).Filter(new LocatorFilterOptions { HasText = "Child NR" })
            .WaitForAsync(new LocatorWaitForOptions { Timeout = 30_000 });
        (await page.GetByText("Birth registered").IsVisibleAsync()).Should().BeTrue();
    }
}