using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using SchaerbeekMunicipality.Domain.Identity;
using SchaerbeekMunicipality.Domain.Registration;
using SchaerbeekMunicipality.Web.Features.Registration.OpenRegistrationCase;
using SchaerbeekMunicipality.Web.Features.Registration.RecordIdentity;

namespace SchaerbeekMunicipality.Integration.Tests.Features.Registration;

public sealed class OpenRegistrationCaseTests
{
    [Fact]
    public async Task OpenRegistrationCase_PersistsIntakeCase()
    {
        await using var factory = new MunicipalAppFactory();
        await using var scope = factory.Services.CreateAsyncScope();
        var handler = scope.ServiceProvider.GetRequiredService<OpenRegistrationCaseHandler>();

        var result = await handler.Handle(
            new OpenRegistrationCaseRequest(VisitReason.FirstRegistration, null),
            CancellationToken.None);

        var repo = scope.ServiceProvider.GetRequiredService<IRegistrationCaseRepository>();
        var registrationCase = await repo.GetByIdAsync(
            new RegistrationCaseId(result.CaseId),
            CancellationToken.None);

        registrationCase.Should().NotBeNull();
        registrationCase!.Status.Should().Be(RegistrationCaseStatus.Intake);
        registrationCase.VisitReason.Should().Be(VisitReason.FirstRegistration);
        registrationCase.AssignedOfficerId.Should().BeNull();
        registrationCase.Checklist.IdentityEstablished.Should().BeFalse();
    }
}

public sealed class RecordIdentityTests
{
    [Fact]
    public async Task RecordIdentity_PersistsPersonAndMarksChecklist()
    {
        await using var factory = new MunicipalAppFactory();
        await using var scope = factory.Services.CreateAsyncScope();
        var openHandler = scope.ServiceProvider.GetRequiredService<OpenRegistrationCaseHandler>();
        var recordHandler = scope.ServiceProvider.GetRequiredService<RecordIdentityHandler>();

        var caseId = await RegistrationTestHelpers.OpenAndClaimCaseAsync(scope.ServiceProvider);
        await recordHandler.Handle(
            caseId,
            new RecordIdentityRequest("Amélie", "Bernard", new DateOnly(1992, 3, 20), "French"),
            CancellationToken.None);

        var caseRepo = scope.ServiceProvider.GetRequiredService<IRegistrationCaseRepository>();
        var personRepo = scope.ServiceProvider.GetRequiredService<IPersonRepository>();

        var registrationCase = await caseRepo.GetByIdAsync(caseId, CancellationToken.None);
        registrationCase!.Checklist.IdentityEstablished.Should().BeTrue();
        registrationCase.PersonId.Should().NotBeNull();

        var person = await personRepo.GetByIdAsync(registrationCase.PersonId!.Value, CancellationToken.None);
        person!.FamilyName.Should().Be("Bernard");
    }
}
