using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using SchaerbeekMunicipality.Domain.Address;
using SchaerbeekMunicipality.Domain.Certificates;
using SchaerbeekMunicipality.Domain.Household;
using SchaerbeekMunicipality.Domain.Immigration;
using SchaerbeekMunicipality.Domain.Notifications;
using SchaerbeekMunicipality.Domain.Police;
using SchaerbeekMunicipality.Domain.Registration;
using SchaerbeekMunicipality.Web.Features.Registration.ApproveCase;
using SchaerbeekMunicipality.Web.Features.Registration.ConfirmRegistration;
using SchaerbeekMunicipality.Web.Features.Registration.DeclareAddress;
using SchaerbeekMunicipality.Web.Features.Registration.IssueHouseholdComposition;
using SchaerbeekMunicipality.Web.Features.Registration.IssueResidenceCertificate;
using SchaerbeekMunicipality.Web.Features.Registration.ListOutboundNotifications;
using SchaerbeekMunicipality.Web.Features.Registration.OpenRegistrationCase;
using SchaerbeekMunicipality.Web.Features.Registration.RecordHouseholdComposition;
using SchaerbeekMunicipality.Web.Features.Registration.RecordIdentity;
using SchaerbeekMunicipality.Web.Features.Registration.RecordPoliceResult;
using SchaerbeekMunicipality.Web.Features.Registration.RequestPoliceVerification;
using SchaerbeekMunicipality.Web.Features.Registration.SetResidenceCategory;

namespace SchaerbeekMunicipality.Integration.Tests.Features.Registration;

public sealed class CertificatesAndNotificationsTests
{
    private static async Task<RegistrationCaseId> CreateRegisteredCaseAsync(IServiceProvider services)
    {
        var openHandler = services.GetRequiredService<OpenRegistrationCaseHandler>();
        var recordHandler = services.GetRequiredService<RecordIdentityHandler>();
        var categoryHandler = services.GetRequiredService<SetResidenceCategoryHandler>();
        var declareHandler = services.GetRequiredService<DeclareAddressHandler>();
        var householdHandler = services.GetRequiredService<RecordHouseholdCompositionHandler>();
        var requestPoliceHandler = services.GetRequiredService<RequestPoliceVerificationHandler>();
        var recordPoliceHandler = services.GetRequiredService<RecordPoliceResultHandler>();
        var approveHandler = services.GetRequiredService<ApproveCaseHandler>();
        var confirmHandler = services.GetRequiredService<ConfirmRegistrationHandler>();

        var caseId = await RegistrationTestHelpers.OpenAndClaimCaseAsync(services);

        await recordHandler.Handle(
            caseId,
            new RecordIdentityRequest("Sophie", "Lambert", new DateOnly(1988, 6, 12), "Belgian"),
            CancellationToken.None);

        await categoryHandler.Handle(
            caseId,
            new SetResidenceCategoryRequest(ResidenceCategory.EuCitizen),
            CancellationToken.None);

        await declareHandler.Handle(
            caseId,
            new DeclareAddressRequest("Chaussée de Louvain", "10", null, "1030", "Schaerbeek"),
            CancellationToken.None);

        await householdHandler.Handle(
            caseId,
            new RecordHouseholdCompositionRequest(
            [
                new HouseholdMemberRequest("Sophie", "Lambert", new DateOnly(1988, 6, 12), HouseholdMemberRole.Head),
            ]),
            CancellationToken.None);

        var policeRequest = await requestPoliceHandler.Handle(caseId, CancellationToken.None);

        await RegistrationTestHelpers.RecordPoliceResultAsync(
            services,
            policeRequest.RequestId,
            PoliceVerificationResult.Confirmed,
            "Present");

        await approveHandler.Handle(
            caseId,
            new ApproveCaseRequest(RegisterTarget.PopulationRegister),
            CancellationToken.None);

        await confirmHandler.Handle(caseId, CancellationToken.None);

        return caseId;
    }

    [Fact]
    public async Task ConfirmRegistration_CreatesOutboundNotifications()
    {
        await using var factory = new MunicipalAppFactory();
        await using var scope = factory.Services.CreateAsyncScope();
        var caseId = await CreateRegisteredCaseAsync(scope.ServiceProvider);

        var listHandler = scope.ServiceProvider.GetRequiredService<ListOutboundNotificationsHandler>();
        var notifications = await listHandler.Handle(caseId, CancellationToken.None);

        notifications.Items.Should().HaveCount(3);
        notifications.Items.Should().Contain(n =>
            n.Recipient == nameof(OutboundNotificationRecipient.TaxAdministration)
            && n.Message.Contains("tax administration", StringComparison.OrdinalIgnoreCase));
        notifications.Items.Should().OnlyContain(n => n.CaseId == caseId.Value);
    }

    [Fact]
    public async Task IssueResidenceCertificate_ForRegisteredCase_PersistsCertificate()
    {
        await using var factory = new MunicipalAppFactory();
        await using var scope = factory.Services.CreateAsyncScope();
        var caseId = await CreateRegisteredCaseAsync(scope.ServiceProvider);

        var issueHandler = scope.ServiceProvider.GetRequiredService<IssueResidenceCertificateHandler>();
        var certificateRepository = scope.ServiceProvider.GetRequiredService<ICertificateRequestRepository>();

        var result = await issueHandler.Handle(caseId, CancellationToken.None);

        result.ReferenceNumber.Should().StartWith("RC-");
        result.HtmlContent.Should().Contain("Verblijfsattest");
        result.HtmlContent.Should().Contain("Sophie Lambert");

        var certificates = await certificateRepository.ListByCaseIdAsync(caseId, CancellationToken.None);
        certificates.Should().ContainSingle(c => c.CertificateType == CertificateType.ResidenceCertificate);
    }

    [Fact]
    public async Task IssueHouseholdComposition_ForRegisteredCase_PersistsCertificate()
    {
        await using var factory = new MunicipalAppFactory();
        await using var scope = factory.Services.CreateAsyncScope();
        var caseId = await CreateRegisteredCaseAsync(scope.ServiceProvider);

        var issueHandler = scope.ServiceProvider.GetRequiredService<IssueHouseholdCompositionHandler>();
        var result = await issueHandler.Handle(caseId, CancellationToken.None);

        result.ReferenceNumber.Should().StartWith("HC-");
        result.HtmlContent.Should().Contain("Household composition");
        result.HtmlContent.Should().Contain("Sophie Lambert");
    }

    [Fact]
    public async Task IssueResidenceCertificate_WhenNotRegistered_Throws()
    {
        await using var factory = new MunicipalAppFactory();
        await using var scope = factory.Services.CreateAsyncScope();

        var openHandler = scope.ServiceProvider.GetRequiredService<OpenRegistrationCaseHandler>();
        var issueHandler = scope.ServiceProvider.GetRequiredService<IssueResidenceCertificateHandler>();

        var opened = await openHandler.Handle(
            new OpenRegistrationCaseRequest(VisitReason.FirstRegistration, null),
            CancellationToken.None);

        var act = () => issueHandler.Handle(new RegistrationCaseId(opened.CaseId), CancellationToken.None);

        await act.Should().ThrowAsync<InvalidRegistrationTransitionException>();
    }
}
