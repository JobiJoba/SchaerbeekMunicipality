using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SchaerbeekMunicipality.Api.Middleware;
using SchaerbeekMunicipality.Application.Auth;
using SchaerbeekMunicipality.Infrastructure.Persistence;

namespace SchaerbeekMunicipality.Integration.Tests;

public sealed class MunicipalAppFactory : WebApplicationFactory<SchaerbeekMunicipality.Api.Program>
{
    private SqliteConnection? _connection;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");

        builder.UseSetting("ConnectionStrings:schaerbeek", "Data Source=:memory:");

        builder.ConfigureServices(services =>
        {
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<MunicipalDbContext>));

            if (descriptor is not null)
            {
                services.Remove(descriptor);
            }

            _connection = new SqliteConnection("Data Source=:memory:");
            _connection.Open();

            services.AddDbContext<MunicipalDbContext>(options =>
                options.UseSqlite(_connection));
        });
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _connection?.Dispose();
        }

        base.Dispose(disposing);
    }
}

public static class DemoOfficerTestClient
{
    public static HttpClient Create(MunicipalAppFactory factory) =>
        factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false,
        });

    public static void ApplyDefaultOfficerHeaders(HttpRequestMessage request)
    {
        request.Headers.Add(DemoOfficerHeaders.OfficerId, CurrentOfficer.PopulationOfficerId.ToString());
        request.Headers.Add(DemoOfficerHeaders.OfficerRole, OfficerRole.PopulationOfficer.ToString());
        request.Headers.Add(DemoOfficerHeaders.OfficerName, DemoOfficers.Marie.DisplayName);
    }

    public static async Task<HttpResponseMessage> SendAsync(
        HttpClient client,
        HttpRequestMessage request,
        OfficerRole role = OfficerRole.PopulationOfficer)
    {
        ApplyOfficerHeaders(request, role);
        return await client.SendAsync(request);
    }

    public static void ApplyOfficerHeaders(HttpRequestMessage request, OfficerRole role)
    {
        var officer = role switch
        {
            OfficerRole.ReceptionOfficer => DemoOfficers.Jean,
            OfficerRole.PoliceClerk => DemoOfficers.Luc,
            _ => DemoOfficers.Marie,
        };

        request.Headers.Add(DemoOfficerHeaders.OfficerId, officer.Id.ToString());
        request.Headers.Add(DemoOfficerHeaders.OfficerRole, officer.Role.ToString());
        request.Headers.Add(DemoOfficerHeaders.OfficerName, officer.DisplayName);
    }
}
