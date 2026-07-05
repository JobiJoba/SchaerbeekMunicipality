using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using SchaerbeekMunicipality.Domain.Documents;
using SchaerbeekMunicipality.Domain.Immigration;
using SchaerbeekMunicipality.Domain.Registration;
using SchaerbeekMunicipality.Web.Features.Registration.AttachDocument;
using SchaerbeekMunicipality.Web.Features.Registration.CorrectIdentity;
using SchaerbeekMunicipality.Web.Features.Registration.GetRegistrationCase;
using SchaerbeekMunicipality.Web.Features.Registration.OpenRegistrationCase;
using SchaerbeekMunicipality.Web.Features.Registration.RecordIdentity;
using SchaerbeekMunicipality.Web.Features.Registration.RecordResidencePermit;
using SchaerbeekMunicipality.Web.Features.Registration.SetResidenceCategory;

namespace SchaerbeekMunicipality.Integration.Tests.Features.Registration;

public sealed class IntakeCorrectionTests
{
    [Fact]
    public async Task PutIdentity_CorrectsNameAndKeepsChecklist()
    {
        await using var factory = new MunicipalAppFactory();
        using var client = factory.CreateClient();

        var caseId = await CreateCaseWithIdentityAsync(client);

        await client.PostAsJsonAsync(
            $"/api/registration/cases/{caseId}/residence-category",
            new SetResidenceCategoryRequest(ResidenceCategory.EuCitizen));

        var response = await client.PutAsJsonAsync(
            $"/api/registration/cases/{caseId}/identity",
            new RecordIdentityRequest("Jean", "Vermeulen", new DateOnly(1988, 11, 5), "Belgian"));

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<CorrectIdentityResponse>();
        body!.IdentityEstablished.Should().BeTrue();
        body.LegalResidenceEstablished.Should().BeTrue();

        var getResponse = await client.GetAsync($"/api/registration/cases/{caseId}");
        var detail = await getResponse.Content.ReadFromJsonAsync<RegistrationCaseDetailDto>();
        detail!.Person!.GivenName.Should().Be("Jean");
        detail.Checklist.LegalResidenceEstablished.Should().BeTrue();
    }

    [Fact]
    public async Task DeleteDocument_RemovesDocumentFromCase()
    {
        await using var factory = new MunicipalAppFactory();
        await using var scope = factory.Services.CreateAsyncScope();

        var caseId = await OpenAndRecordIdentityAsync(scope.ServiceProvider);
        var attachHandler = scope.ServiceProvider.GetRequiredService<AttachDocumentHandler>();

        await using var stream = new MemoryStream("fake passport scan"u8.ToArray());
        var attached = await attachHandler.Handle(
            caseId,
            DocumentType.Passport,
            "passport.pdf",
            stream,
            CancellationToken.None);

        using var client = factory.CreateClient();
        var deleteResponse = await client.DeleteAsync(
            $"/api/registration/cases/{caseId.Value}/documents/{attached.DocumentId}");

        deleteResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var getHandler = scope.ServiceProvider.GetRequiredService<GetRegistrationCaseHandler>();
        var detail = await getHandler.Handle(caseId, CancellationToken.None);
        detail!.Documents.Should().BeEmpty();
    }

    [Fact]
    public async Task SetResidenceCategory_ChangeToStudentWithStalePermit_FailsPolicyUntilPermitCorrected()
    {
        await using var factory = new MunicipalAppFactory();
        await using var scope = factory.Services.CreateAsyncScope();

        var caseId = await OpenAndRecordIdentityAsync(scope.ServiceProvider);

        var categoryHandler = scope.ServiceProvider.GetRequiredService<SetResidenceCategoryHandler>();
        await categoryHandler.Handle(
            caseId,
            new SetResidenceCategoryRequest(ResidenceCategory.NonEuWorker),
            CancellationToken.None);

        var permitHandler = scope.ServiceProvider.GetRequiredService<RecordResidencePermitHandler>();
        await permitHandler.Handle(
            caseId,
            new RecordResidencePermitRequest(
                ResidencePermitType.ACard,
                DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(-1)),
                DateOnly.FromDateTime(DateTime.UtcNow.AddYears(1)),
                "AC-123",
                "Immigration Office"),
            CancellationToken.None);

        var changeResult = await categoryHandler.Handle(
            caseId,
            new SetResidenceCategoryRequest(ResidenceCategory.Student),
            CancellationToken.None);

        changeResult.LegalResidenceEstablished.Should().BeFalse();
        changeResult.PolicyMessage.Should().NotBeNullOrWhiteSpace();

        var correctedPermit = await permitHandler.Handle(
            caseId,
            new RecordResidencePermitRequest(
                ResidencePermitType.Annex15,
                DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(-1)),
                DateOnly.FromDateTime(DateTime.UtcNow.AddYears(1)),
                "A15-456",
                "Immigration Office"),
            CancellationToken.None);

        correctedPermit.LegalResidenceEstablished.Should().BeTrue();
    }

    [Fact]
    public async Task PutIdentity_WhenApproved_ReturnsConflict()
    {
        await using var factory = new MunicipalAppFactory();
        await using var scope = factory.Services.CreateAsyncScope();

        var caseId = await OpenAndRecordIdentityAsync(scope.ServiceProvider);
        var caseRepo = scope.ServiceProvider.GetRequiredService<IRegistrationCaseRepository>();
        var registrationCase = await caseRepo.GetByIdAsync(caseId, CancellationToken.None);
        typeof(RegistrationCase)
            .GetProperty(nameof(RegistrationCase.Status))!
            .SetValue(registrationCase, RegistrationCaseStatus.Approved);
        await caseRepo.SaveChangesAsync(CancellationToken.None);

        using var client = factory.CreateClient();
        var response = await client.PutAsJsonAsync(
            $"/api/registration/cases/{caseId.Value}/identity",
            new RecordIdentityRequest("Jean", "Vermeulen", new DateOnly(1988, 11, 5), "Belgian"));

        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    private static async Task<Guid> CreateCaseWithIdentityAsync(HttpClient client)
    {
        var createResponse = await client.PostAsJsonAsync(
            "/api/registration/cases",
            new OpenRegistrationCaseRequest(VisitReason.FirstRegistration, null));

        var created = await createResponse.Content.ReadFromJsonAsync<OpenRegistrationCaseResponse>();
        created.Should().NotBeNull();

        await client.PostAsync($"/api/registration/cases/{created!.CaseId}/claim", null);

        await client.PostAsJsonAsync(
            $"/api/registration/cases/{created!.CaseId}/identity",
            new RecordIdentityRequest("Jon", "Vermeulen", new DateOnly(1988, 11, 5), "Belgian"));

        return created.CaseId;
    }

    private static async Task<RegistrationCaseId> OpenAndRecordIdentityAsync(IServiceProvider services)
    {
        var openHandler = services.GetRequiredService<OpenRegistrationCaseHandler>();
        var recordHandler = services.GetRequiredService<RecordIdentityHandler>();

        var caseId = await RegistrationTestHelpers.OpenAndClaimCaseAsync(services);
        await recordHandler.Handle(
            caseId,
            new RecordIdentityRequest("Amélie", "Bernard", new DateOnly(1992, 3, 20), "French"),
            CancellationToken.None);

        return caseId;
    }
}
