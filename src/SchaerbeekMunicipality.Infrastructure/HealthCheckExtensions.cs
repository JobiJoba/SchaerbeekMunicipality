using Microsoft.Extensions.DependencyInjection;
using SchaerbeekMunicipality.Infrastructure.Persistence;

namespace SchaerbeekMunicipality.Infrastructure;

public static class HealthCheckExtensions
{
    public static IServiceCollection AddMunicipalDatabaseHealthCheck(this IServiceCollection services)
    {
        services.AddHealthChecks()
            .AddDbContextCheck<MunicipalDbContext>(
                name: "database",
                tags: ["ready"]);

        return services;
    }
}
