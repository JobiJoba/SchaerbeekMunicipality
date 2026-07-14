using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using SchaerbeekMunicipality.Application.Features.Registration.RecordIdentity;
using SchaerbeekMunicipality.Application.Features.Registration.RecordResidencePermit;
using SchaerbeekMunicipality.Application.Features.Registration.SetResidenceCategory;
using SchaerbeekMunicipality.Domain.Immigration;
using SchaerbeekMunicipality.Domain.Registration;

namespace SchaerbeekMunicipality.Integration.Tests.Features.Registration;

public sealed class ResidenceCategoryTests
{
    [Fact]
    public async Task SetResidenceCategory_EuCitizen_MarksLegalResidenceEstablished()
    {
        await using var factory = new MunicipalAppFactory();
        await using var scope = factory.Services.CreateAsyncScope();

        var caseId = await OpenAndRecordIdentityAsync(scope.ServiceProvider);
        await RegistrationTestHelpers.AttachIdentityDocumentAsync(scope.ServiceProvider, caseId);

        var handler = scope.ServiceProvider.GetRequiredService<SetResidenceCategoryHandler>();
        var result = await handler.Handle(
            caseId,
            new SetResidenceCategoryRequest(ResidenceCategory.EuCitizen),
            CancellationToken.None);

        result.LegalResidenceEstablished.Should().BeTrue();

        var caseRepo = scope.ServiceProvider.GetRequiredService<IRegistrationCaseRepository>();
        var registrationCase = await caseRepo.GetByIdAsync(caseId, CancellationToken.None);
        registrationCase!.ResidenceCategory.Should().Be(ResidenceCategory.EuCitizen);
        registrationCase.Checklist.LegalResidenceEstablished.Should().BeTrue();
    }

    [Fact]
    public async Task SetResidenceCategory_NonEuWorker_WithoutPermit_DoesNotMarkLegalResidence()
    {
        await using var factory = new MunicipalAppFactory();
        await using var scope = factory.Services.CreateAsyncScope();

        var caseId = await OpenAndRecordIdentityAsync(scope.ServiceProvider);

        var handler = scope.ServiceProvider.GetRequiredService<SetResidenceCategoryHandler>();
        var result = await handler.Handle(
            caseId,
            new SetResidenceCategoryRequest(ResidenceCategory.NonEuWorker),
            CancellationToken.None);

        result.LegalResidenceEstablished.Should().BeFalse();
        result.PolicyMessage.Should().Contain("passport");
    }

    [Fact]
    public async Task RecordResidencePermit_NonEuWorker_WithBCard_MarksLegalResidenceEstablished()
    {
        await using var factory = new MunicipalAppFactory();
        await using var scope = factory.Services.CreateAsyncScope();

        var caseId = await OpenAndRecordIdentityAsync(scope.ServiceProvider);
        var categoryHandler = scope.ServiceProvider.GetRequiredService<SetResidenceCategoryHandler>();
        await categoryHandler.Handle(
            caseId,
            new SetResidenceCategoryRequest(ResidenceCategory.NonEuWorker),
            CancellationToken.None);

        await RegistrationTestHelpers.AttachIdentityDocumentAsync(scope.ServiceProvider, caseId);

        var permitHandler = scope.ServiceProvider.GetRequiredService<RecordResidencePermitHandler>();
        var result = await permitHandler.Handle(
            caseId,
            new RecordResidencePermitRequest(
                ResidencePermitType.BCard,
                DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(-1)),
                DateOnly.FromDateTime(DateTime.UtcNow.AddYears(1)),
                "BC-98765",
                "Immigration Office"),
            CancellationToken.None);

        result.LegalResidenceEstablished.Should().BeTrue();

        var permitRepo = scope.ServiceProvider.GetRequiredService<IResidencePermitRepository>();
        var permit = await permitRepo.GetByCaseIdAsync(caseId, CancellationToken.None);
        permit.Should().NotBeNull();
        permit.PermitType.Should().Be(ResidencePermitType.BCard);
    }

    [Fact]
    public async Task SetResidenceCategory_WithoutIdentity_Throws()
    {
        await using var factory = new MunicipalAppFactory();
        await using var scope = factory.Services.CreateAsyncScope();

        var caseId = await RegistrationTestHelpers.OpenAndClaimCaseAsync(scope.ServiceProvider);

        var handler = scope.ServiceProvider.GetRequiredService<SetResidenceCategoryHandler>();
        var act = () => handler.Handle(
            caseId,
            new SetResidenceCategoryRequest(ResidenceCategory.EuCitizen),
            CancellationToken.None);

        await act.Should().ThrowAsync<InvalidRegistrationTransitionException>();
    }

    private static async Task<RegistrationCaseId> OpenAndRecordIdentityAsync(IServiceProvider services)
    {
        var recordHandler = services.GetRequiredService<RecordIdentityHandler>();

        var caseId = await RegistrationTestHelpers.OpenAndClaimCaseAsync(services);
        await recordHandler.Handle(
            caseId,
            new RecordIdentityRequest("Amélie", "Bernard", new DateOnly(1992, 3, 20), "French"),
            CancellationToken.None);

        return caseId;
    }
}