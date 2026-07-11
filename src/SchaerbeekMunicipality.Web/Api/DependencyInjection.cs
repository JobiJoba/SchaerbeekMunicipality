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
        RegisterClient<IRegistrationApi, RegistrationApiClient>(services, environment);
        RegisterClient<IBirthDeclarationApi, BirthDeclarationApiClient>(services, environment);
        RegisterClient<IChangeOfAddressApi, ChangeOfAddressApiClient>(services, environment);
        return services;
    }

    private static void RegisterClient<TClient, TImplementation>(
        IServiceCollection services,
        IHostEnvironment environment)
        where TClient : class
        where TImplementation : class, TClient
    {
        services.AddHttpClient<TClient, TImplementation>((serviceProvider, client) =>
            {
                var bridge = serviceProvider.GetService<IMunicipalApiBridge>();
                client.BaseAddress = bridge?.BaseAddress
                    ?? new Uri(environment.IsDevelopment() ? "http://api" : "https+http://api");
            })
            .ConfigurePrimaryHttpMessageHandler(serviceProvider =>
            {
                var bridge = serviceProvider.GetService<IMunicipalApiBridge>();
                return bridge?.CreateHandler() ?? new HttpClientHandler();
            });
    }
}
