using System.Net;
using FluentAssertions;

namespace SchaerbeekMunicipality.Integration.Tests;

public class HealthEndpointTests(MunicipalAppFactory factory) : IClassFixture<MunicipalAppFactory>
{
    [Fact]
    public async Task Health_ReturnsOk()
    {
        var client = factory.CreateClient();

        var response = await client.GetAsync("/health");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}