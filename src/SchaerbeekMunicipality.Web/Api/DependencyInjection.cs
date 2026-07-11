using SchaerbeekMunicipality.Web.Api.BirthDeclaration;
using SchaerbeekMunicipality.Web.Api.ChangeOfAddress;
using SchaerbeekMunicipality.Web.Api.Registration;

namespace SchaerbeekMunicipality.Web.Api;

public static class DependencyInjection
{
    private const string ApiBaseAddress = "https+http://api";

    public static IServiceCollection AddMunicipalApiClients(this IServiceCollection services)
    {
        services.AddTransient<OfficerForwardingHandler>();

        services.AddHttpClient<IRegistrationApi, RegistrationApiClient>(client =>
            client.BaseAddress = new Uri(ApiBaseAddress))
            .AddHttpMessageHandler<OfficerForwardingHandler>();

        services.AddHttpClient<IBirthDeclarationApi, BirthDeclarationApiClient>(client =>
            client.BaseAddress = new Uri(ApiBaseAddress))
            .AddHttpMessageHandler<OfficerForwardingHandler>();

        services.AddHttpClient<IChangeOfAddressApi, ChangeOfAddressApiClient>(client =>
            client.BaseAddress = new Uri(ApiBaseAddress))
            .AddHttpMessageHandler<OfficerForwardingHandler>();

        return services;
    }
}
