using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using SchaerbeekMunicipality.Domain.Documents;
using SchaerbeekMunicipality.Domain.Immigration;
using SchaerbeekMunicipality.Domain.Registration;
using SchaerbeekMunicipality.Web.Features.Registration.AttachDocument;
using SchaerbeekMunicipality.Web.Features.Registration.GetRegistrationCase;
using SchaerbeekMunicipality.Web.Features.Registration.RecordBirthInformation;
using SchaerbeekMunicipality.Web.Features.Registration.RecordIdentity;
using SchaerbeekMunicipality.Web.Features.Registration.ResolveDuplicateInvestigation;
using SchaerbeekMunicipality.Web.Features.Registration.SetResidenceCategory;

namespace SchaerbeekMunicipality.Integration.Tests.Features.Registration;

public sealed class Phase9ExceptionScenarioTests
{
    [Fact]
    public async Task RecordBirthInformation_WithoutCertificate_LeavesBirthEvidenceIncomplete()
    {
        await using var factory = new MunicipalAppFactory();
        await using var scope = factory.Services.CreateAsyncScope();
        var services = scope.ServiceProvider;

        var caseId = await RegistrationTestHelpers.OpenAndClaimCaseAsync(services);
        await services.GetRequiredService<RecordIdentityHandler>().Handle(
            caseId,
            new RecordIdentityRequest("Anna", "Verbeeck", new DateOnly(1992, 4, 2), "Belgian"),
            CancellationToken.None);

        await services.GetRequiredService<RecordBirthInformationHandler>().Handle(
            caseId,
            new RecordBirthInformationRequest("Ghent", "Belgium"),
            CancellationToken.None);

        var detail = await services.GetRequiredService<GetRegistrationCaseHandler>()
            .Handle(caseId, CancellationToken.None);

        detail!.Checklist.BirthEvidenceEstablished.Should().BeFalse();
    }

    [Fact]
    public async Task AttachBirthCertificate_AfterBirthInformation_CompletesBirthEvidence()
    {
        await using var factory = new MunicipalAppFactory();
        await using var scope = factory.Services.CreateAsyncScope();
        var services = scope.ServiceProvider;

        var caseId = await RegistrationTestHelpers.OpenAndClaimCaseAsync(services);
        await services.GetRequiredService<RecordIdentityHandler>().Handle(
            caseId,
            new RecordIdentityRequest("Anna", "Verbeeck", new DateOnly(1992, 4, 2), "Belgian"),
            CancellationToken.None);

        await RegistrationTestHelpers.SatisfyPhase9RequirementsAsync(services, caseId);

        var detail = await services.GetRequiredService<GetRegistrationCaseHandler>()
            .Handle(caseId, CancellationToken.None);

        detail!.Checklist.BirthEvidenceEstablished.Should().BeTrue();
    }

    [Fact]
    public async Task EuCitizen_WithoutPassport_FailsLegalResidenceUntilDocumentAttached()
    {
        await using var factory = new MunicipalAppFactory();
        await using var scope = factory.Services.CreateAsyncScope();
        var services = scope.ServiceProvider;

        var caseId = await RegistrationTestHelpers.OpenAndClaimCaseAsync(services);
        await services.GetRequiredService<RecordIdentityHandler>().Handle(
            caseId,
            new RecordIdentityRequest("Anna", "Verbeeck", new DateOnly(1992, 4, 2), "French"),
            CancellationToken.None);

        await services.GetRequiredService<SetResidenceCategoryHandler>().Handle(
            caseId,
            new SetResidenceCategoryRequest(ResidenceCategory.EuCitizen),
            CancellationToken.None);

        var before = await services.GetRequiredService<GetRegistrationCaseHandler>()
            .Handle(caseId, CancellationToken.None);
        before!.Checklist.LegalResidenceEstablished.Should().BeFalse();

        await using var stream = new MemoryStream([0x25, 0x50, 0x44, 0x46]);
        await services.GetRequiredService<AttachDocumentHandler>().Handle(
            caseId,
            DocumentType.Passport,
            "passport.pdf",
            stream,
            CancellationToken.None);

        var after = await services.GetRequiredService<GetRegistrationCaseHandler>()
            .Handle(caseId, CancellationToken.None);
        after!.Checklist.LegalResidenceEstablished.Should().BeTrue();
    }

    [Fact]
    public async Task ResolveDuplicateInvestigation_ClosesOpenInvestigation()
    {
        await using var factory = new MunicipalAppFactory();
        await using var scope = factory.Services.CreateAsyncScope();
        var services = scope.ServiceProvider;

        var caseId = await RegistrationTestHelpers.OpenAndClaimCaseAsync(services);
        await services.GetRequiredService<RecordIdentityHandler>().Handle(
            caseId,
            new RecordIdentityRequest("Jean", "Dupont", new DateOnly(1985, 6, 12), "Belgian"),
            CancellationToken.None);

        var before = await services.GetRequiredService<GetRegistrationCaseHandler>()
            .Handle(caseId, CancellationToken.None);
        before!.DuplicateInvestigationStatus.Should().Be(DuplicateInvestigationStatus.Open);

        await services.GetRequiredService<ResolveDuplicateInvestigationHandler>().Handle(
            caseId,
            new ResolveDuplicateInvestigationRequest("Confirmed distinct after review."),
            CancellationToken.None);

        var after = await services.GetRequiredService<GetRegistrationCaseHandler>()
            .Handle(caseId, CancellationToken.None);
        after!.DuplicateInvestigationStatus.Should().Be(DuplicateInvestigationStatus.ResolvedDistinct);
        after.Checklist.DuplicateInvestigationResolved.Should().BeTrue();
    }
}
