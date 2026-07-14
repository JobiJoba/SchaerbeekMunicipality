using FluentValidation;
using SchaerbeekMunicipality.Application.Auth;
using SchaerbeekMunicipality.Application.Features.Registration.OpenRegistrationCase;

namespace SchaerbeekMunicipality.Web;

public static class DependencyInjection
{
    public static IServiceCollection AddWebPresentation(this IServiceCollection services)
    {
        services.AddScoped<ICurrentOfficer, CurrentOfficer>();
        services.AddValidatorsFromAssemblyContaining<OpenRegistrationCaseValidator>();

        return services;
    }
}