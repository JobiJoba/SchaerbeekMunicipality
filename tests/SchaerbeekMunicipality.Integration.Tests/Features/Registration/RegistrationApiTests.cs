using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using SchaerbeekMunicipality.Domain.Immigration;
using SchaerbeekMunicipality.Domain.Registration;
using SchaerbeekMunicipality.Web.Features.Registration.OpenRegistrationCase;
using SchaerbeekMunicipality.Web.Features.Registration.RecordIdentity;
using SchaerbeekMunicipality.Web.Features.Registration.RecordResidencePermit;
using SchaerbeekMunicipality.Web.Features.Registration.SetResidenceCategory;

namespace SchaerbeekMunicipality.Integration.Tests.Features.Registration;

public sealed class RegistrationApiTests
{
    [Fact]
    public async Task PostCases_ValidRequest_ReturnsCreated()
    {
        await using var factory = new MunicipalAppFactory();
        using var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync(
            "/api/registration/cases",
            new OpenRegistrationCaseRequest(VisitReason.FirstRegistration, null));

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task PostCasesIdentity_ValidRequest_ReturnsOk()
    {
        await using var factory = new MunicipalAppFactory();
        using var client = factory.CreateClient();

        var createResponse = await client.PostAsJsonAsync(
            "/api/registration/cases",
            new OpenRegistrationCaseRequest(VisitReason.FirstRegistration, null));

        var created = await createResponse.Content.ReadFromJsonAsync<OpenRegistrationCaseResponse>();
        created.Should().NotBeNull();

        var identityResponse = await client.PostAsJsonAsync(
            $"/api/registration/cases/{created!.CaseId}/identity",
            new RecordIdentityRequest("Luc", "Vermeulen", new DateOnly(1988, 11, 5), "Belgian"));

        identityResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task PostResidenceCategory_EuCitizen_ReturnsOkWithLegalResidenceEstablished()
    {
        await using var factory = new MunicipalAppFactory();
        using var client = factory.CreateClient();

        var caseId = await CreateCaseWithIdentityAsync(client);

        var response = await client.PostAsJsonAsync(
            $"/api/registration/cases/{caseId}/residence-category",
            new SetResidenceCategoryRequest(ResidenceCategory.EuCitizen));

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<SetResidenceCategoryResponse>();
        body!.LegalResidenceEstablished.Should().BeTrue();
    }

    [Fact]
    public async Task PostResidencePermit_NonEuWorker_ReturnsOk()
    {
        await using var factory = new MunicipalAppFactory();
        using var client = factory.CreateClient();

        var caseId = await CreateCaseWithIdentityAsync(client);

        await client.PostAsJsonAsync(
            $"/api/registration/cases/{caseId}/residence-category",
            new SetResidenceCategoryRequest(ResidenceCategory.NonEuWorker));

        var response = await client.PostAsJsonAsync(
            $"/api/registration/cases/{caseId}/residence-permit",
            new RecordResidencePermitRequest(
                ResidencePermitType.BCard,
                DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(-1)),
                DateOnly.FromDateTime(DateTime.UtcNow.AddYears(1)),
                "BC-123",
                "Immigration Office"));

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<RecordResidencePermitResponse>();
        body!.LegalResidenceEstablished.Should().BeTrue();
    }

    private static async Task<Guid> CreateCaseWithIdentityAsync(HttpClient client)
    {
        var createResponse = await client.PostAsJsonAsync(
            "/api/registration/cases",
            new OpenRegistrationCaseRequest(VisitReason.FirstRegistration, null));

        var created = await createResponse.Content.ReadFromJsonAsync<OpenRegistrationCaseResponse>();
        created.Should().NotBeNull();

        await client.PostAsJsonAsync(
            $"/api/registration/cases/{created!.CaseId}/identity",
            new RecordIdentityRequest("Luc", "Vermeulen", new DateOnly(1988, 11, 5), "Belgian"));

        return created.CaseId;
    }
}
