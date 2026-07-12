using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using SchaerbeekMunicipality.Application.Auth;
using SchaerbeekMunicipality.Application.Features.Registration.ApproveCase;
using SchaerbeekMunicipality.Application.Features.Registration.ConfirmRegistration;
using SchaerbeekMunicipality.Application.Features.Registration.DeclareAddress;
using SchaerbeekMunicipality.Application.Features.Registration.GetReviewDashboard;
using SchaerbeekMunicipality.Application.Features.Reporting.GetMunicipalityReport;
using SchaerbeekMunicipality.Application.Features.Registration.ListRegistrationCases;
using SchaerbeekMunicipality.Application.Features.Registration.RecordIdentity;
using SchaerbeekMunicipality.Application.Features.Registration.RequestPoliceVerification;
using SchaerbeekMunicipality.Application.Features.Registration.SetResidenceCategory;
using SchaerbeekMunicipality.Domain.Immigration;
using SchaerbeekMunicipality.Domain.Police;
using SchaerbeekMunicipality.Domain.Registration;
using SchaerbeekMunicipality.Integration.Tests.Features.Registration;

namespace SchaerbeekMunicipality.Integration.Tests.Features.Reporting;

public sealed class MunicipalityReportTests
{
    [Fact]
    public async Task BackOfficeOfficer_CanViewMunicipalityReport()
    {
        await using var factory = new MunicipalAppFactory();
        await using var scope = factory.Services.CreateAsyncScope();
        RegistrationTestHelpers.SetRole(scope.ServiceProvider, OfficerRole.BackOfficeOfficer);

        var handler = scope.ServiceProvider.GetRequiredService<GetMunicipalityReportHandler>();
        var report = await handler.Handle(12, CancellationToken.None);

        report.Months.Should().Be(12);
        report.Summary.Should().NotBeNull();
        report.VolumeSeries.Should().HaveCount(12);
    }

    [Fact]
    public async Task BackOfficeOfficer_CannotViewReviewDashboard()
    {
        await using var factory = new MunicipalAppFactory();
        await using var scope = factory.Services.CreateAsyncScope();
        RegistrationTestHelpers.SetRole(scope.ServiceProvider, OfficerRole.BackOfficeOfficer);

        var handler = scope.ServiceProvider.GetRequiredService<GetReviewDashboardHandler>();
        var act = () => handler.Handle(CancellationToken.None);

        await act.Should().ThrowAsync<UnauthorizedAccessException>();
    }

    [Fact]
    public async Task BackOfficeOfficer_CannotListRegistrationCases()
    {
        await using var factory = new MunicipalAppFactory();
        await using var scope = factory.Services.CreateAsyncScope();
        RegistrationTestHelpers.SetRole(scope.ServiceProvider, OfficerRole.BackOfficeOfficer);

        var handler = scope.ServiceProvider.GetRequiredService<ListRegistrationCasesHandler>();
        var act = () => handler.Handle(CancellationToken.None);

        await act.Should().ThrowAsync<UnauthorizedAccessException>();
    }

    [Theory]
    [InlineData(OfficerRole.ReceptionOfficer)]
    [InlineData(OfficerRole.PoliceClerk)]
    public async Task NonReportingRoles_CannotViewMunicipalityReport(OfficerRole role)
    {
        await using var factory = new MunicipalAppFactory();
        await using var scope = factory.Services.CreateAsyncScope();
        RegistrationTestHelpers.SetRole(scope.ServiceProvider, role);

        var handler = scope.ServiceProvider.GetRequiredService<GetMunicipalityReportHandler>();
        var act = () => handler.Handle(12, CancellationToken.None);

        await act.Should().ThrowAsync<UnauthorizedAccessException>();
    }

    [Fact]
    public async Task PopulationOfficer_CanViewMunicipalityReport()
    {
        await using var factory = new MunicipalAppFactory();
        await using var scope = factory.Services.CreateAsyncScope();
        RegistrationTestHelpers.SetRole(scope.ServiceProvider, OfficerRole.PopulationOfficer);

        var handler = scope.ServiceProvider.GetRequiredService<GetMunicipalityReportHandler>();
        var report = await handler.Handle(12, CancellationToken.None);

        report.Summary.RegistrationsCompleted.Should().BeGreaterThanOrEqualTo(0);
    }

    [Fact]
    public async Task EmptyDatabase_ReturnsZeroCountsWithoutError()
    {
        await using var factory = new MunicipalAppFactory();
        await using var scope = factory.Services.CreateAsyncScope();
        RegistrationTestHelpers.SetRole(scope.ServiceProvider, OfficerRole.BackOfficeOfficer);

        var handler = scope.ServiceProvider.GetRequiredService<GetMunicipalityReportHandler>();
        var report = await handler.Handle(12, CancellationToken.None);

        report.Summary.RegistrationsCompleted.Should().Be(0);
        report.Summary.BirthDeclarationsConfirmed.Should().Be(0);
        report.Summary.AddressChangesConfirmed.Should().Be(0);
        report.Summary.AveragePoliceWaitDays.Should().BeNull();
        report.Summary.AverageIntakeToRegisteredDays.Should().BeNull();
        report.Summary.RejectionRatePercent.Should().Be(0);
        report.Outcomes.Registered.Should().Be(0);
        report.Outcomes.Rejected.Should().Be(0);
        report.Outcomes.Suspended.Should().Be(0);
        report.VolumeSeries.Should().OnlyContain(p =>
            p.Registrations == 0 && p.BirthDeclarations == 0 && p.AddressChanges == 0);
    }

    [Fact]
    public async Task CompletedRegistration_IsCountedInReportSummary()
    {
        await using var factory = new MunicipalAppFactory();
        await using var scope = factory.Services.CreateAsyncScope();
        RegistrationTestHelpers.SetRole(scope.ServiceProvider, OfficerRole.PopulationOfficer);

        var caseId = await CreateCaseReadyForDecisionAsync(scope.ServiceProvider);

        var approveHandler = scope.ServiceProvider.GetRequiredService<ApproveCaseHandler>();
        var confirmHandler = scope.ServiceProvider.GetRequiredService<ConfirmRegistrationHandler>();
        var reportHandler = scope.ServiceProvider.GetRequiredService<GetMunicipalityReportHandler>();

        await approveHandler.Handle(
            caseId,
            new ApproveCaseRequest(RegisterTarget.PopulationRegister),
            CancellationToken.None);

        await confirmHandler.Handle(caseId, CancellationToken.None);

        var report = await reportHandler.Handle(12, CancellationToken.None);

        report.Summary.RegistrationsCompleted.Should().BeGreaterThanOrEqualTo(1);
        report.Outcomes.Registered.Should().BeGreaterThanOrEqualTo(1);
    }

    [Fact]
    public async Task GetMunicipalityReport_Api_AllowsBackOfficeOfficer()
    {
        await using var factory = new MunicipalAppFactory();
        using var client = factory.CreateApiClient();

        var request = new HttpRequestMessage(HttpMethod.Get, "/api/reports/municipality?months=12");
        DemoOfficerTestClient.ApplyOfficerHeaders(request, OfficerRole.BackOfficeOfficer);

        var response = await client.SendAsync(request);

        response.IsSuccessStatusCode.Should().BeTrue();
    }

    [Fact]
    public async Task GetMunicipalityReport_Api_DeniesReceptionOfficer()
    {
        await using var factory = new MunicipalAppFactory();
        using var client = factory.CreateApiClient();

        var request = new HttpRequestMessage(HttpMethod.Get, "/api/reports/municipality?months=12");
        DemoOfficerTestClient.ApplyOfficerHeaders(request, OfficerRole.ReceptionOfficer);

        var response = await client.SendAsync(request);

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.Forbidden);
    }

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
}
