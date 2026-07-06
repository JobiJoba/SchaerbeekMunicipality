using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SchaerbeekMunicipality.Domain.Certificates;
using SchaerbeekMunicipality.Domain.Documents;
using SchaerbeekMunicipality.Domain.Household;
using SchaerbeekMunicipality.Domain.Identity;
using SchaerbeekMunicipality.Domain.Immigration;
using SchaerbeekMunicipality.Domain.Immigration.Policies;
using SchaerbeekMunicipality.Domain.NationalRegister;
using SchaerbeekMunicipality.Domain.Notifications;
using SchaerbeekMunicipality.Domain.Police;
using SchaerbeekMunicipality.Domain.ReferenceData;
using SchaerbeekMunicipality.Domain.Registration;
using SchaerbeekMunicipality.Infrastructure.Certificates;
using SchaerbeekMunicipality.Infrastructure.Events;
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
        services.AddScoped<IResidencePermitRepository, ResidencePermitRepository>();
        services.AddScoped<IHouseholdRepository, HouseholdRepository>();
        services.AddScoped<IReferenceDataRepository, ReferenceDataRepository>();
        services.AddScoped<INationalRegisterRepository, NationalRegisterRepository>();
        services.AddScoped<IPoliceVerificationRepository, PoliceVerificationRepository>();
        services.AddScoped<ICaseAuditRepository, CaseAuditRepository>();
        services.AddScoped<ICertificateRequestRepository, CertificateRequestRepository>();
        services.AddScoped<IOutboundNotificationRepository, OutboundNotificationRepository>();
        services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();
        services.AddScoped<IRegistrationConfirmedHandler, RegistrationConfirmedLoggingHandler>();
        services.AddScoped<IRegistrationConfirmedHandler, RegistrationConfirmedNotificationHandler>();
        services.AddSingleton<ICertificateRenderer, HtmlCertificateRenderer>();
        services.AddSingleton<IDocumentStorage, LocalFileDocumentStorage>();

        services.AddSingleton<IResidencePolicy, EuCitizenPolicy>();
        services.AddSingleton<IResidencePolicy, NonEuWorkerPolicy>();
        services.AddSingleton<IResidencePolicy, StudentPolicy>();
        services.AddSingleton<ResidencePolicyEvaluator>();

        return services;
    }

    public static async Task InitializeDatabaseAsync(this IServiceProvider services)
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
        else
        {
            await dbContext.Database.MigrateAsync();
        }

        await ReferenceDataSeeder.SeedAsync(dbContext, CancellationToken.None);
        await NationalRegisterSeeder.SeedAsync(dbContext, CancellationToken.None);
    }

    private static bool IsSqlite(string connectionString) =>
        connectionString.Contains("Data Source", StringComparison.OrdinalIgnoreCase)
        || connectionString.Contains(":memory:", StringComparison.OrdinalIgnoreCase);
}
