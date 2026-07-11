using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using SchaerbeekMunicipality.Domain.Registration;
using SchaerbeekMunicipality.Application.Auth;
using SchaerbeekMunicipality.Application.Features.BirthDeclaration.OpenBirthDeclarationCase;
using SchaerbeekMunicipality.Application.Features.Registration.ClaimRegistrationCase;
using SchaerbeekMunicipality.Application.Features.Registration.GetRegistrationCase;
using SchaerbeekMunicipality.Application.Features.Registration.ListCaseAudit;
using SchaerbeekMunicipality.Application.Features.Registration.ListRegistrationCases;
using SchaerbeekMunicipality.Application.Features.Registration.GetReviewDashboard;
using SchaerbeekMunicipality.Application.Features.Registration.OpenRegistrationCase;
using SchaerbeekMunicipality.Application.Features.Registration.RecordIdentity;
using SchaerbeekMunicipality.Application.Features.Registration.ReleaseCaseLock;
using SchaerbeekMunicipality.Application.Features.Registration;

namespace SchaerbeekMunicipality.Integration.Tests.Features.Registration;

public sealed class RoleBoundariesAndCaseLockingTests
{
    [Fact]
    public async Task ReceptionOfficer_CanOpenCase_ButCannotListOrView()
    {
        await using var factory = new MunicipalAppFactory();
        await using var scope = factory.Services.CreateAsyncScope();
        RegistrationTestHelpers.SetRole(scope.ServiceProvider, OfficerRole.ReceptionOfficer);

        var openHandler = scope.ServiceProvider.GetRequiredService<OpenRegistrationCaseHandler>();
        var listHandler = scope.ServiceProvider.GetRequiredService<ListRegistrationCasesHandler>();
        var getHandler = scope.ServiceProvider.GetRequiredService<GetRegistrationCaseHandler>();

        var opened = await openHandler.Handle(
            new OpenRegistrationCaseRequest(VisitReason.FirstRegistration, null),
            CancellationToken.None);

        var listAct = () => listHandler.Handle(CancellationToken.None);
        await listAct.Should().ThrowAsync<UnauthorizedAccessException>();

        var getAct = () => getHandler.Handle(new RegistrationCaseId(opened.CaseId), CancellationToken.None);
        await getAct.Should().ThrowAsync<UnauthorizedAccessException>();
    }

    [Fact]
    public async Task PopulationOfficer_ClaimsUnassignedCase_AndRecordsAudit()
    {
        await using var factory = new MunicipalAppFactory();
        await using var scope = factory.Services.CreateAsyncScope();
        RegistrationTestHelpers.SetRole(scope.ServiceProvider, OfficerRole.ReceptionOfficer);

        var openHandler = scope.ServiceProvider.GetRequiredService<OpenRegistrationCaseHandler>();
        var opened = await openHandler.Handle(
            new OpenRegistrationCaseRequest(VisitReason.FirstRegistration, null),
            CancellationToken.None);

        RegistrationTestHelpers.SetRole(scope.ServiceProvider, OfficerRole.PopulationOfficer);

        var claimHandler = scope.ServiceProvider.GetRequiredService<ClaimRegistrationCaseHandler>();
        var claim = await claimHandler.Handle(new RegistrationCaseId(opened.CaseId), CancellationToken.None);

        claim.NewlyClaimed.Should().BeTrue();
        claim.CanEdit.Should().BeTrue();
        claim.AssignedOfficerId.Should().Be(CurrentOfficer.PopulationOfficerId);

        var auditHandler = scope.ServiceProvider.GetRequiredService<ListCaseAuditHandler>();
        var audit = await auditHandler.Handle(new RegistrationCaseId(opened.CaseId), CancellationToken.None);

        audit.Entries.Select(e => e.Action).Should().Contain("CaseOpened");
        audit.Entries.Select(e => e.Action).Should().Contain("CaseAssigned");
    }

    [Fact]
    public async Task SecondPopulationOfficer_GetsReadOnlyCaseDetail()
    {
        await using var factory = new MunicipalAppFactory();
        await using var scope = factory.Services.CreateAsyncScope();

        RegistrationTestHelpers.SetRole(scope.ServiceProvider, OfficerRole.ReceptionOfficer);
        var openHandler = scope.ServiceProvider.GetRequiredService<OpenRegistrationCaseHandler>();
        var opened = await openHandler.Handle(
            new OpenRegistrationCaseRequest(VisitReason.FirstRegistration, null),
            CancellationToken.None);
        var caseId = new RegistrationCaseId(opened.CaseId);

        RegistrationTestHelpers.SetRole(scope.ServiceProvider, OfficerRole.PopulationOfficer);
        await scope.ServiceProvider.GetRequiredService<ClaimRegistrationCaseHandler>()
            .Handle(caseId, CancellationToken.None);

        RegistrationTestHelpers.SelectPopulationOfficer(scope.ServiceProvider, secondary: true);

        var claimAct = () => scope.ServiceProvider.GetRequiredService<ClaimRegistrationCaseHandler>()
            .Handle(caseId, CancellationToken.None);
        await claimAct.Should().ThrowAsync<InvalidRegistrationTransitionException>();

        var detail = await scope.ServiceProvider.GetRequiredService<GetRegistrationCaseHandler>()
            .Handle(caseId, CancellationToken.None);

        detail!.CanEdit.Should().BeFalse();
        detail.IsReadOnlyDueToLock.Should().BeTrue();
    }

    [Fact]
    public async Task LockHolder_CanEdit_AfterRelease_AnotherOfficerCanClaim()
    {
        await using var factory = new MunicipalAppFactory();
        await using var scope = factory.Services.CreateAsyncScope();

        var caseId = await RegistrationTestHelpers.OpenAndClaimCaseAsync(scope.ServiceProvider);

        var recordHandler = scope.ServiceProvider.GetRequiredService<RecordIdentityHandler>();
        await recordHandler.Handle(
            caseId,
            new RecordIdentityRequest("Marie", "Curie", new DateOnly(1990, 1, 1), "Belgian"),
            CancellationToken.None);

        await scope.ServiceProvider.GetRequiredService<ReleaseCaseLockHandler>()
            .Handle(caseId, CancellationToken.None);

        RegistrationTestHelpers.SelectPopulationOfficer(scope.ServiceProvider, secondary: true);

        var claim = await scope.ServiceProvider.GetRequiredService<ClaimRegistrationCaseHandler>()
            .Handle(caseId, CancellationToken.None);

        claim.NewlyClaimed.Should().BeTrue();
        claim.CanEdit.Should().BeTrue();
    }

    [Fact]
    public async Task PoliceClerk_CannotListCases()
    {
        await using var factory = new MunicipalAppFactory();
        await using var scope = factory.Services.CreateAsyncScope();

        RegistrationTestHelpers.SetRole(scope.ServiceProvider, OfficerRole.PoliceClerk);

        var listAct = () => scope.ServiceProvider.GetRequiredService<ListRegistrationCasesHandler>()
            .Handle(CancellationToken.None);
        await listAct.Should().ThrowAsync<UnauthorizedAccessException>();
    }

    [Fact]
    public async Task ReleaseLock_DoesNotImmediatelyReclaimForSameOfficer()
    {
        await using var factory = new MunicipalAppFactory();
        await using var scope = factory.Services.CreateAsyncScope();

        var caseId = await RegistrationTestHelpers.OpenAndClaimCaseAsync(scope.ServiceProvider);
        var releaseHandler = scope.ServiceProvider.GetRequiredService<ReleaseCaseLockHandler>();
        var claimHandler = scope.ServiceProvider.GetRequiredService<ClaimRegistrationCaseHandler>();
        var getHandler = scope.ServiceProvider.GetRequiredService<GetRegistrationCaseHandler>();

        await releaseHandler.Handle(caseId, CancellationToken.None);

        var autoClaim = await claimHandler.TryAutoClaimAsync(caseId, CancellationToken.None);
        autoClaim.Should().BeNull();

        var detail = await getHandler.Handle(caseId, CancellationToken.None);
        detail!.LockedByOfficerId.Should().BeNull();
        detail.CanEdit.Should().BeFalse();
    }

    [Fact]
    public async Task MutatingHandler_RejectsWhenLockedToAnotherOfficer()
    {
        await using var factory = new MunicipalAppFactory();
        await using var scope = factory.Services.CreateAsyncScope();

        RegistrationTestHelpers.SetRole(scope.ServiceProvider, OfficerRole.ReceptionOfficer);
        var opened = await scope.ServiceProvider.GetRequiredService<OpenRegistrationCaseHandler>()
            .Handle(new OpenRegistrationCaseRequest(VisitReason.FirstRegistration, null), CancellationToken.None);
        var caseId = new RegistrationCaseId(opened.CaseId);

        RegistrationTestHelpers.SetRole(scope.ServiceProvider, OfficerRole.PopulationOfficer);
        await scope.ServiceProvider.GetRequiredService<ClaimRegistrationCaseHandler>()
            .Handle(caseId, CancellationToken.None);

        RegistrationTestHelpers.SelectPopulationOfficer(scope.ServiceProvider, secondary: true);

        var act = () => scope.ServiceProvider.GetRequiredService<RecordIdentityHandler>()
            .Handle(
                caseId,
                new RecordIdentityRequest("Jean", "Dupont", new DateOnly(1985, 5, 5), "French"),
                CancellationToken.None);

        await act.Should().ThrowAsync<InvalidRegistrationTransitionException>();
    }

    [Fact]
    public async Task UnassignedCase_AppearsOnReviewDashboard_AsNeedingAttention()
    {
        await using var factory = new MunicipalAppFactory();
        await using var scope = factory.Services.CreateAsyncScope();

        RegistrationTestHelpers.SetRole(scope.ServiceProvider, OfficerRole.ReceptionOfficer);
        var opened = await scope.ServiceProvider.GetRequiredService<OpenRegistrationCaseHandler>()
            .Handle(new OpenRegistrationCaseRequest(VisitReason.FirstRegistration, null), CancellationToken.None);

        RegistrationTestHelpers.SetRole(scope.ServiceProvider, OfficerRole.PopulationOfficer);
        var dashboard = await scope.ServiceProvider
            .GetRequiredService<GetReviewDashboardHandler>()
            .Handle(CancellationToken.None);

        dashboard.Statistics.Should().Contain(s => s.Label == "Unassigned" && s.Value == 1);
        dashboard.ActionableCases.Should().ContainSingle(c =>
            c.CaseId == opened.CaseId &&
            c.CaseType == ReviewDashboardCaseType.Registration &&
            c.Summary == "Unassigned — awaiting intake");
    }

    [Fact]
    public async Task UnassignedBirthDeclaration_AppearsOnReviewDashboard_AsNeedingAttention()
    {
        await using var factory = new MunicipalAppFactory();
        await using var scope = factory.Services.CreateAsyncScope();

        RegistrationTestHelpers.SetRole(scope.ServiceProvider, OfficerRole.ReceptionOfficer);
        var opened = await scope.ServiceProvider.GetRequiredService<OpenBirthDeclarationCaseHandler>()
            .Handle(CancellationToken.None);

        RegistrationTestHelpers.SetRole(scope.ServiceProvider, OfficerRole.PopulationOfficer);
        var dashboard = await scope.ServiceProvider
            .GetRequiredService<GetReviewDashboardHandler>()
            .Handle(CancellationToken.None);

        dashboard.Statistics.Should().Contain(s => s.Label == "Birth unassigned" && s.Value == 1);
        dashboard.ActionableCases.Should().ContainSingle(c =>
            c.CaseId == opened.CaseId &&
            c.CaseType == ReviewDashboardCaseType.BirthDeclaration &&
            c.Procedure == "Birth declaration" &&
            c.Summary == "Unassigned — awaiting intake");
    }
}
