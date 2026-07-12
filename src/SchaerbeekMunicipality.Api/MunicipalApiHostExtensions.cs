using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SchaerbeekMunicipality.Api.Features.BirthDeclaration;
using SchaerbeekMunicipality.Api.Features.ChangeOfAddress;
using SchaerbeekMunicipality.Api.Features.IdentityDocuments;
using SchaerbeekMunicipality.Api.Features.Registration;
using SchaerbeekMunicipality.Api.Middleware;
using SchaerbeekMunicipality.Application;
using SchaerbeekMunicipality.Application.DemoData;
using SchaerbeekMunicipality.Infrastructure;
using SchaerbeekMunicipality.Infrastructure.Persistence;

namespace SchaerbeekMunicipality.Api;

public static class MunicipalApiHostExtensions
{
    public static IHostApplicationBuilder AddMunicipalApiHost(this IHostApplicationBuilder builder)
    {
        builder.Services.AddInfrastructure(builder.Configuration);
        builder.Services.AddMunicipalDatabaseHealthCheck();
        builder.Services.AddApplication();
        builder.Services.AddValidatorsFromAssembly(typeof(RegistrationEndpoints).Assembly);
        builder.Services.AddOpenApi();

        return builder;
    }

    public static async Task InitializeMunicipalApiDatabaseAsync(this WebApplication app)
    {
        await app.Services.InitializeDatabaseAsync();

        if (app.Configuration.GetValue($"{DemoDataOptions.SectionName}:{nameof(DemoDataOptions.SeedWorkflowCases)}", false))
        {
            await using var scope = app.Services.CreateAsyncScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<MunicipalDbContext>();
            await PopulationRegisterSeeder.SeedAsync(dbContext, CancellationToken.None);
            await DemoWorkflowCaseSeeder.SeedAsync(app.Services);
        }
    }

    public static WebApplication UseMunicipalApiHost(this WebApplication app)
    {
        app.UseDemoOfficerResolution();
        app.UseDomainExceptionHandling();

        app.MapRegistrationEndpoints();
        app.MapBirthDeclarationEndpoints();
        app.MapChangeOfAddressEndpoints();
        app.MapIdentityDocumentsEndpoints();
        app.MapOpenApi();

        return app;
    }
}
