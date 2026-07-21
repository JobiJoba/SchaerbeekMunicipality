using FluentValidation;
using SchaerbeekMunicipality.Application.Auth;
using SchaerbeekMunicipality.Application.Features.Registration.OpenRegistrationCase;
using SchaerbeekMunicipality.Web.Municipal.Components;

namespace SchaerbeekMunicipality.Web;

public static class DependencyInjection
{
    public static IServiceCollection AddWebPresentation(this IServiceCollection services)
    {
        services.AddScoped<ICurrentOfficer, CurrentOfficer>();
        services.AddScoped<INationalRegisterSearchQueries, NationalRegisterSearchQueries>();
        services.AddValidatorsFromAssemblyContaining<OpenRegistrationCaseValidator>();

        return services;
    }
}