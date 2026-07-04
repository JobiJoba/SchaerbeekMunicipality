using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SchaerbeekMunicipality.Domain.Documents;
using SchaerbeekMunicipality.Domain.Identity;
using SchaerbeekMunicipality.Domain.Registration;
using SchaerbeekMunicipality.Infrastructure.Persistence;
using SchaerbeekMunicipality.Infrastructure.Persistence.Repositories;
using SchaerbeekMunicipality.Infrastructure.Storage;

namespace SchaerbeekMunicipality.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("schaerbeek")
            ?? "Data Source=:memory:";

        services.AddDbContext<MunicipalDbContext>(options =>
        {
            if (IsSqlite(connectionString))
            {
                options.UseSqlite(connectionString);
            }
            else
            {
                options.UseNpgsql(connectionString)
                    .UseSnakeCaseNamingConvention();
            }
        });

        services.AddScoped<IRegistrationCaseRepository, RegistrationCaseRepository>();
        services.AddScoped<IPersonRepository, PersonRepository>();
        services.AddScoped<IAdministrativeDocumentRepository, AdministrativeDocumentRepository>();
        services.AddSingleton<IDocumentStorage, LocalFileDocumentStorage>();

        return services;
    }

    public static async Task InitializeDatabaseAsync(this IServiceProvider services, IHostEnvironment environment)
    {
        await using var scope = services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<MunicipalDbContext>();
        var connectionString = scope.ServiceProvider
            .GetRequiredService<IConfiguration>()
            .GetConnectionString("schaerbeek");

        if (IsSqlite(connectionString ?? "Data Source=:memory:"))
        {
            await dbContext.Database.EnsureCreatedAsync();
        }
        else if (environment.IsDevelopment())
        {
            await dbContext.Database.MigrateAsync();
        }
    }

    private static bool IsSqlite(string connectionString) =>
        connectionString.Contains("Data Source", StringComparison.OrdinalIgnoreCase)
        || connectionString.Contains(":memory:", StringComparison.OrdinalIgnoreCase);
}
