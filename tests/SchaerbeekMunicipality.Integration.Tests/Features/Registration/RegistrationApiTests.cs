using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using SchaerbeekMunicipality.Domain.Registration;
using SchaerbeekMunicipality.Web.Features.Registration.OpenRegistrationCase;
using SchaerbeekMunicipality.Web.Features.Registration.RecordIdentity;

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
}
