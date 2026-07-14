using SchaerbeekMunicipality.Web.Api.BirthDeclaration;
using SchaerbeekMunicipality.Web.Api.ChangeOfAddress;
using SchaerbeekMunicipality.Web.Api.IdentityDocuments;
using SchaerbeekMunicipality.Web.Api.PersonFile;
using SchaerbeekMunicipality.Web.Api.Registration;
using SchaerbeekMunicipality.Web.Api.Reporting;
using SchaerbeekMunicipality.Web.Api.RegisterAmendment;

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
        RegisterClient<IRegisterAmendmentApi, RegisterAmendmentApiClient>(services, environment);
        RegisterClient<IIdentityDocumentsApi, IdentityDocumentsApiClient>(services, environment);
        RegisterClient<IReportingApi, ReportingApiClient>(services, environment);
        RegisterClient<IPersonFileApi, PersonFileApiClient>(services, environment);
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
                client.BaseAddress = ResolveApiBaseAddress(serviceProvider, environment);
            })
            .ConfigurePrimaryHttpMessageHandler(serviceProvider =>
            {
                var bridge = serviceProvider.GetService<IMunicipalApiBridge>();
                return bridge?.CreateHandler() ?? new HttpClientHandler();
            });
    }

    private static Uri ResolveApiBaseAddress(IServiceProvider serviceProvider, IHostEnvironment environment)
    {
        var bridge = serviceProvider.GetService<IMunicipalApiBridge>();
        if (bridge is not null) return bridge.BaseAddress;

        if (environment.IsDevelopment()) return new Uri("http://api");

        var configuration = serviceProvider.GetRequiredService<IConfiguration>();
        var configuredBaseAddress = configuration["MunicipalApi:BaseAddress"];
        return configuredBaseAddress is not null
            ? new Uri(configuredBaseAddress)
            : new Uri("http://127.0.0.1:8080");
    }
}