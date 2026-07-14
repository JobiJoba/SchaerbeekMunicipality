using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using SchaerbeekMunicipality.Application.Features.Registration.ApproveCase;
using SchaerbeekMunicipality.Application.Features.Registration.ConfirmRegistration;
using SchaerbeekMunicipality.Application.Features.Registration.DeclareAddress;
using SchaerbeekMunicipality.Application.Features.Registration.GetCaseReviewChecklist;
using SchaerbeekMunicipality.Application.Features.Registration.ListCaseAudit;
using SchaerbeekMunicipality.Application.Features.Registration.RecordIdentity;
using SchaerbeekMunicipality.Application.Features.Registration.RejectCase;
using SchaerbeekMunicipality.Application.Features.Registration.RequestPoliceVerification;
using SchaerbeekMunicipality.Application.Features.Registration.SetResidenceCategory;
using SchaerbeekMunicipality.Domain.Immigration;
using SchaerbeekMunicipality.Domain.Police;
using SchaerbeekMunicipality.Domain.Registration;

namespace SchaerbeekMunicipality.Integration.Tests.Features.Registration;

public sealed class CaseDecisionTests
{
    private static async Task<RegistrationCaseId> CreateCaseReadyForDecisionAsync(IServiceProvider services)
    {
        var recordHandler = services.GetRequiredService<RecordIdentityHandler>();
        var categoryHandler = services.GetRequiredService<SetResidenceCategoryHandler>();
        var declareHandler = services.GetRequiredService<DeclareAddressHandler>();
        var requestPoliceHandler = services.GetRequiredService<RequestPoliceVerificationHandler>();

        var caseId = await RegistrationTestHelpers.OpenAndClaimCaseAsync(services);

        await recordHandler.Handle(
            caseId,
            new RecordIdentityRequest("Sophie", "Lambert", new DateOnly(1988, 6, 12), "Belgian"),
            CancellationToken.None);

        await categoryHandler.Handle(
            caseId,
            new SetResidenceCategoryRequest(ResidenceCategory.EuCitizen),
            CancellationToken.None);

        await RegistrationTestHelpers.SatisfyPhase9RequirementsAsync(services, caseId);

        await declareHandler.Handle(
            caseId,
            new DeclareAddressRequest("Chaussée de Louvain", "10", null, "1030", "Schaerbeek"),
            CancellationToken.None);

        var policeRequest = await requestPoliceHandler.Handle(caseId, CancellationToken.None);

        await RegistrationTestHelpers.RecordPoliceResultAsync(
            services,
            policeRequest.RequestId,
            PoliceVerificationResult.Confirmed,
            "Present");

        return caseId;
    }

    [Fact]
    public async Task FullHappyPath_OpenToRegistered()
    {
        await using var factory = new MunicipalAppFactory();
        await using var scope = factory.Services.CreateAsyncScope();
        var caseId = await CreateCaseReadyForDecisionAsync(scope.ServiceProvider);

        var checklistHandler = scope.ServiceProvider.GetRequiredService<GetCaseReviewChecklistHandler>();
        var approveHandler = scope.ServiceProvider.GetRequiredService<ApproveCaseHandler>();
        var confirmHandler = scope.ServiceProvider.GetRequiredService<ConfirmRegistrationHandler>();
        var caseRepo = scope.ServiceProvider.GetRequiredService<IRegistrationCaseRepository>();

        var checklist = await checklistHandler.Handle(caseId, CancellationToken.None);
        checklist!.IsReadyForApproval.Should().BeTrue();
        checklist.SuggestedRegisterTarget.Should().Be(nameof(RegisterTarget.PopulationRegister));

        await approveHandler.Handle(
            caseId,
            new ApproveCaseRequest(RegisterTarget.PopulationRegister),
            CancellationToken.None);

        var approved = await caseRepo.GetByIdAsync(caseId, CancellationToken.None);
        approved!.Status.Should().Be(RegistrationCaseStatus.Approved);

        var confirmed = await confirmHandler.Handle(caseId, CancellationToken.None);
        confirmed.Status.Should().Be(nameof(RegistrationCaseStatus.Registered));
        confirmed.NationalRegisterNumber.Should().NotBeNullOrWhiteSpace();

        var registered = await caseRepo.GetByIdAsync(caseId, CancellationToken.None);
        registered!.Status.Should().Be(RegistrationCaseStatus.Registered);
    }

    [Fact]
    public async Task RejectCase_PreservesAuditTrail()
    {
        await using var factory = new MunicipalAppFactory();
        await using var scope = factory.Services.CreateAsyncScope();
        var caseId = await CreateCaseReadyForDecisionAsync(scope.ServiceProvider);

        var rejectHandler = scope.ServiceProvider.GetRequiredService<RejectCaseHandler>();
        var auditHandler = scope.ServiceProvider.GetRequiredService<ListCaseAuditHandler>();

        await rejectHandler.Handle(
            caseId,
            new RejectCaseRequest(RejectionReason.AddressNotGenuine, "Police notes"),
            CancellationToken.None);

        var audit = await auditHandler.Handle(caseId, CancellationToken.None);
        audit.Entries.Should().ContainSingle(e => e.Action == nameof(CaseAuditAction.CaseRejected));
    }

    [Fact]
    public async Task Approve_WhenPoliceNegative_Throws()
    {
        await using var factory = new MunicipalAppFactory();
        await using var scope = factory.Services.CreateAsyncScope();

        var recordHandler = scope.ServiceProvider.GetRequiredService<RecordIdentityHandler>();
        var declareHandler = scope.ServiceProvider.GetRequiredService<DeclareAddressHandler>();
        var requestPoliceHandler = scope.ServiceProvider.GetRequiredService<RequestPoliceVerificationHandler>();
        var approveHandler = scope.ServiceProvider.GetRequiredService<ApproveCaseHandler>();

        var caseId = await RegistrationTestHelpers.OpenAndClaimCaseAsync(scope.ServiceProvider);

        await recordHandler.Handle(
            caseId,
            new RecordIdentityRequest("Sophie", "Lambert", new DateOnly(1988, 6, 12), "Belgian"),
            CancellationToken.None);

        await declareHandler.Handle(
            caseId,
            new DeclareAddressRequest("Chaussée de Louvain", "10", null, "1030", "Schaerbeek"),
            CancellationToken.None);

        var policeRequest = await requestPoliceHandler.Handle(caseId, CancellationToken.None);
        await RegistrationTestHelpers.RecordPoliceResultAsync(
            scope.ServiceProvider,
            policeRequest.RequestId,
            PoliceVerificationResult.NotFound);

        var act = () => approveHandler.Handle(
            caseId,
            new ApproveCaseRequest(RegisterTarget.PopulationRegister),
            CancellationToken.None);

        await act.Should().ThrowAsync<InvalidRegistrationTransitionException>();
    }
}