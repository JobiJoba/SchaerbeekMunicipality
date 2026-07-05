using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using SchaerbeekMunicipality.Domain.Address;
using SchaerbeekMunicipality.Domain.Police;
using SchaerbeekMunicipality.Domain.Registration;
using SchaerbeekMunicipality.Web.Features.Registration.DeclareAddress;
using SchaerbeekMunicipality.Web.Features.Registration.GetRegistrationCase;
using SchaerbeekMunicipality.Web.Features.Registration.ListPendingPoliceVerifications;
using SchaerbeekMunicipality.Web.Features.Registration.OpenRegistrationCase;
using SchaerbeekMunicipality.Web.Features.Registration.RecordIdentity;
using SchaerbeekMunicipality.Web.Features.Registration.RecordPoliceResult;
using SchaerbeekMunicipality.Web.Features.Registration.RequestPoliceVerification;

namespace SchaerbeekMunicipality.Integration.Tests.Features.Registration;

public sealed class PoliceVerificationTests
{
    private static async Task<RegistrationCaseId> OpenCaseWithIdentityAndAddressAsync(IServiceProvider services)
    {
        var openHandler = services.GetRequiredService<OpenRegistrationCaseHandler>();
        var recordHandler = services.GetRequiredService<RecordIdentityHandler>();
        var declareHandler = services.GetRequiredService<DeclareAddressHandler>();

        var opened = await openHandler.Handle(
            new OpenRegistrationCaseRequest(VisitReason.FirstRegistration, null),
            CancellationToken.None);

        var caseId = new RegistrationCaseId(opened.CaseId);

        await recordHandler.Handle(
            caseId,
            new RecordIdentityRequest("Sophie", "Lambert", new DateOnly(1988, 6, 12), "Belgian"),
            CancellationToken.None);

        await declareHandler.Handle(
            caseId,
            new DeclareAddressRequest("Chaussée de Louvain", "10", null, "1030", "Schaerbeek"),
            CancellationToken.None);

        return caseId;
    }

    [Fact]
    public async Task RequestPoliceVerification_FullLoop_ConfirmsAddress()
    {
        await using var factory = new MunicipalAppFactory();
        await using var scope = factory.Services.CreateAsyncScope();
        var caseId = await OpenCaseWithIdentityAndAddressAsync(scope.ServiceProvider);

        var requestHandler = scope.ServiceProvider.GetRequiredService<RequestPoliceVerificationHandler>();
        var listHandler = scope.ServiceProvider.GetRequiredService<ListPendingPoliceVerificationsHandler>();
        var recordHandler = scope.ServiceProvider.GetRequiredService<RecordPoliceResultHandler>();
        var caseRepo = scope.ServiceProvider.GetRequiredService<IRegistrationCaseRepository>();

        var request = await requestHandler.Handle(caseId, CancellationToken.None);
        request.AttemptNumber.Should().Be(1);

        var registrationCase = await caseRepo.GetByIdAsync(caseId, CancellationToken.None);
        registrationCase!.Status.Should().Be(RegistrationCaseStatus.AwaitingPoliceVerification);

        var pending = await listHandler.Handle(CancellationToken.None);
        pending.Items.Should().ContainSingle(i => i.CaseId == caseId.Value);

        await recordHandler.Handle(
            PoliceVerificationRequestId.From(request.RequestId),
            new RecordPoliceResultRequest(PoliceVerificationResult.Confirmed, "Person present"),
            CancellationToken.None);

        registrationCase = await caseRepo.GetByIdAsync(caseId, CancellationToken.None);
        registrationCase!.Status.Should().Be(RegistrationCaseStatus.UnderReview);
        registrationCase.Checklist.AddressConfirmed.Should().BeTrue();
    }

    [Fact]
    public async Task RecordPoliceResult_NegativeResult_DoesNotConfirmAddress()
    {
        await using var factory = new MunicipalAppFactory();
        await using var scope = factory.Services.CreateAsyncScope();
        var caseId = await OpenCaseWithIdentityAndAddressAsync(scope.ServiceProvider);

        var requestHandler = scope.ServiceProvider.GetRequiredService<RequestPoliceVerificationHandler>();
        var recordHandler = scope.ServiceProvider.GetRequiredService<RecordPoliceResultHandler>();
        var caseRepo = scope.ServiceProvider.GetRequiredService<IRegistrationCaseRepository>();

        var request = await requestHandler.Handle(caseId, CancellationToken.None);

        await recordHandler.Handle(
            PoliceVerificationRequestId.From(request.RequestId),
            new RecordPoliceResultRequest(PoliceVerificationResult.NotFound, null),
            CancellationToken.None);

        var registrationCase = await caseRepo.GetByIdAsync(caseId, CancellationToken.None);
        registrationCase!.Checklist.AddressConfirmed.Should().BeFalse();
        registrationCase.HasPositivePoliceVerification.Should().BeFalse();
    }

    [Fact]
    public async Task RequestPoliceVerification_Incomplete_AllowsSecondAttempt()
    {
        await using var factory = new MunicipalAppFactory();
        await using var scope = factory.Services.CreateAsyncScope();
        var caseId = await OpenCaseWithIdentityAndAddressAsync(scope.ServiceProvider);

        var requestHandler = scope.ServiceProvider.GetRequiredService<RequestPoliceVerificationHandler>();
        var recordHandler = scope.ServiceProvider.GetRequiredService<RecordPoliceResultHandler>();
        var caseRepo = scope.ServiceProvider.GetRequiredService<IRegistrationCaseRepository>();

        var first = await requestHandler.Handle(caseId, CancellationToken.None);

        await recordHandler.Handle(
            PoliceVerificationRequestId.From(first.RequestId),
            new RecordPoliceResultRequest(PoliceVerificationResult.Incomplete, "Nobody home"),
            CancellationToken.None);

        var second = await requestHandler.Handle(caseId, CancellationToken.None);
        second.AttemptNumber.Should().Be(2);

        var registrationCase = await caseRepo.GetByIdAsync(caseId, CancellationToken.None);
        registrationCase!.Status.Should().Be(RegistrationCaseStatus.AwaitingPoliceVerification);
    }

    [Fact]
    public async Task RequestPoliceVerification_WhilePending_Throws()
    {
        await using var factory = new MunicipalAppFactory();
        await using var scope = factory.Services.CreateAsyncScope();
        var caseId = await OpenCaseWithIdentityAndAddressAsync(scope.ServiceProvider);

        var requestHandler = scope.ServiceProvider.GetRequiredService<RequestPoliceVerificationHandler>();
        await requestHandler.Handle(caseId, CancellationToken.None);

        var act = () => requestHandler.Handle(caseId, CancellationToken.None);

        await act.Should().ThrowAsync<InvalidPoliceVerificationException>();
    }

    [Fact]
    public async Task GetRegistrationCase_AfterPoliceResult_ReturnsHistoryWithNotes()
    {
        await using var factory = new MunicipalAppFactory();
        await using var scope = factory.Services.CreateAsyncScope();
        var caseId = await OpenCaseWithIdentityAndAddressAsync(scope.ServiceProvider);

        var requestHandler = scope.ServiceProvider.GetRequiredService<RequestPoliceVerificationHandler>();
        var recordHandler = scope.ServiceProvider.GetRequiredService<RecordPoliceResultHandler>();
        var getHandler = scope.ServiceProvider.GetRequiredService<GetRegistrationCaseHandler>();

        var request = await requestHandler.Handle(caseId, CancellationToken.None);

        await recordHandler.Handle(
            PoliceVerificationRequestId.From(request.RequestId),
            new RecordPoliceResultRequest(PoliceVerificationResult.NotFound, "Nobody answered the door."),
            CancellationToken.None);

        var detail = await getHandler.Handle(caseId, CancellationToken.None);

        detail.Should().NotBeNull();
        detail!.ActivePoliceVerification.Should().BeNull();
        detail.PoliceVerificationHistory.Should().ContainSingle();
        detail.PoliceVerificationHistory[0].Result.Should().Be(PoliceVerificationResult.NotFound);
        detail.PoliceVerificationHistory[0].OfficerNotes.Should().Be("Nobody answered the door.");
    }
}
