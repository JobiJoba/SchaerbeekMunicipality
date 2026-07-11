using Microsoft.Extensions.Hosting;
using SchaerbeekMunicipality.Web.Api.BirthDeclaration;
using SchaerbeekMunicipality.Web.Api.ChangeOfAddress;
using SchaerbeekMunicipality.Web.Api.Registration;

namespace SchaerbeekMunicipality.Web.Api;

public static class DependencyInjection
{
    public static IServiceCollection AddMunicipalApiClients(
        this IServiceCollection services,
        IHostEnvironment environment)
    {
        // Prefer plain HTTP in development: Aspire's dev HTTPS cert is often untrusted by HttpClient.
        var apiBaseAddress = environment.IsDevelopment()
            ? "http://api"
            : "https+http://api";

        services.AddHttpClient<IRegistrationApi, RegistrationApiClient>(client =>
            client.BaseAddress = new Uri(apiBaseAddress));

        services.AddHttpClient<IBirthDeclarationApi, BirthDeclarationApiClient>(client =>
            client.BaseAddress = new Uri(apiBaseAddress));

        services.AddHttpClient<IChangeOfAddressApi, ChangeOfAddressApiClient>(client =>
            client.BaseAddress = new Uri(apiBaseAddress));

        return services;
    }
}
