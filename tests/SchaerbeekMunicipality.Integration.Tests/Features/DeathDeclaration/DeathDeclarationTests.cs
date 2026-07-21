using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using SchaerbeekMunicipality.Application.Auth;
using SchaerbeekMunicipality.Application.Features.DeathDeclaration.ConfirmRadiation;
using SchaerbeekMunicipality.Application.Features.DeathDeclaration.OpenDeathDeclarationCase;
using SchaerbeekMunicipality.Application.Features.DeathDeclaration.RejectDeathDeclaration;
using SchaerbeekMunicipality.Application.Features.Registration.GetReviewDashboard;
using SchaerbeekMunicipality.Domain.DeathDeclaration;
using SchaerbeekMunicipality.Domain.Identity;
using SchaerbeekMunicipality.Infrastructure.Persistence;
using SchaerbeekMunicipality.Integration.Tests.Features.Registration;

namespace SchaerbeekMunicipality.Integration.Tests.Features.DeathDeclaration;

public sealed class DeathDeclarationTests
{
    [Fact]
    public async Task HappyPath_Confirm_MarksPersonDeceasedAndClearsDomicile()
    {
        await using var factory = new MunicipalAppFactory();
        await using var scope = factory.Services.CreateAsyncScope();
        var services = scope.ServiceProvider;

        var personId = await DeathDeclarationTestHelpers.CreateRegisteredPersonAsync(services);
        var caseId = await DeathDeclarationTestHelpers.PrepareCaseReadyForConfirmationAsync(services, personId);

        var confirmed = await services.GetRequiredService<ConfirmRadiationHandler>()
            .Handle(caseId, CancellationToken.None);

        confirmed.Status.Should().Be(nameof(DeathDeclarationCaseStatus.Confirmed));

        var person = await services.GetRequiredService<IPersonRepository>()
            .GetByIdAsync(new PersonId(personId), CancellationToken.None);
        person!.IsDeceased.Should().BeTrue();
        person.DateOfDeath.Should().NotBeNull();
        person.DomicileAddress.Should().BeNull();
    }

    [Fact]
    public async Task Open_ForDeceasedPerson_Throws()
    {
        await using var factory = new MunicipalAppFactory();
        await using var scope = factory.Services.CreateAsyncScope();
        var services = scope.ServiceProvider;

        var personId = await DeathDeclarationTestHelpers.CreateRegisteredPersonAsync(services);
        var caseId = await DeathDeclarationTestHelpers.PrepareCaseReadyForConfirmationAsync(services, personId);
        await services.GetRequiredService<ConfirmRadiationHandler>().Handle(caseId, CancellationToken.None);

        var openHandler = services.GetRequiredService<OpenDeathDeclarationCaseHandler>();
        var act = () => openHandler.Handle(new OpenDeathDeclarationCaseRequest(personId), CancellationToken.None);

        await act.Should().ThrowAsync<InvalidDeathDeclarationTransitionException>();
    }

    [Fact]
    public async Task ReceptionOfficer_CannotConfirm()
    {
        await using var factory = new MunicipalAppFactory();
        await using var scope = factory.Services.CreateAsyncScope();
        var services = scope.ServiceProvider;

        var personId = await DeathDeclarationTestHelpers.CreateRegisteredPersonAsync(services);
        var caseId = await DeathDeclarationTestHelpers.PrepareCaseReadyForConfirmationAsync(services, personId);

        RegistrationTestHelpers.SetRole(services, OfficerRole.ReceptionOfficer);

        var act = () => services.GetRequiredService<ConfirmRadiationHandler>().Handle(caseId, CancellationToken.None);

        await act.Should().ThrowAsync<UnauthorizedAccessException>();
    }

    [Fact]
    public async Task ReceptionOfficer_CannotReject()
    {
        await using var factory = new MunicipalAppFactory();
        await using var scope = factory.Services.CreateAsyncScope();
        var services = scope.ServiceProvider;

        var personId = await DeathDeclarationTestHelpers.CreateRegisteredPersonAsync(services);
        var caseId = await DeathDeclarationTestHelpers.PrepareCaseReadyForConfirmationAsync(services, personId);

        RegistrationTestHelpers.SetRole(services, OfficerRole.ReceptionOfficer);

        var act = () => services.GetRequiredService<RejectDeathDeclarationHandler>().Handle(
            caseId,
            new RejectDeathDeclarationRequest(DeathDeclarationRejectionReason.InsufficientEvidence, "No evidence"),
            CancellationToken.None);

        await act.Should().ThrowAsync<UnauthorizedAccessException>();
    }

    [Fact]
    public async Task Reject_FromIntake_AllowsOpeningNewCaseForSamePerson()
    {
        await using var factory = new MunicipalAppFactory();
        await using var scope = factory.Services.CreateAsyncScope();
        var services = scope.ServiceProvider;
        RegistrationTestHelpers.SetRole(services, OfficerRole.PopulationOfficer);

        var personId = await DeathDeclarationTestHelpers.CreateRegisteredPersonAsync(services);
        var caseId = await DeathDeclarationTestHelpers.OpenAndClaimCaseAsync(services, personId);

        var rejected = await services.GetRequiredService<RejectDeathDeclarationHandler>().Handle(
            caseId,
            new RejectDeathDeclarationRequest(
                DeathDeclarationRejectionReason.OpenedInError,
                "Wrong person selected"),
            CancellationToken.None);

        rejected.Status.Should().Be(nameof(DeathDeclarationCaseStatus.Rejected));

        var reopened = await services.GetRequiredService<OpenDeathDeclarationCaseHandler>()
            .Handle(new OpenDeathDeclarationCaseRequest(personId), CancellationToken.None);

        reopened.CaseId.Should().NotBe(caseId.Value);
    }

    [Fact]
    public async Task ReviewDashboard_IncludesDeathCases()
    {
        await using var factory = new MunicipalAppFactory();
        await using var scope = factory.Services.CreateAsyncScope();
        var services = scope.ServiceProvider;

        var personId = await DeathDeclarationTestHelpers.CreateRegisteredPersonAsync(services);
        var caseId = await DeathDeclarationTestHelpers.PrepareCaseReadyForConfirmationAsync(services, personId);

        RegistrationTestHelpers.SetRole(services, OfficerRole.PopulationOfficer);

        var dashboard = await services.GetRequiredService<GetReviewDashboardHandler>()
            .Handle(CancellationToken.None);

        dashboard.ActionableCases.Should()
            .Contain(row => row.CaseType == ReviewDashboardCaseType.DeathDeclaration
                            && row.CaseId == caseId.Value);
    }
}
