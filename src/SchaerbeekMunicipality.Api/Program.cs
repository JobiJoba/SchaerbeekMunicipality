using FluentValidation;
using SchaerbeekMunicipality.Api.Features.BirthDeclaration;
using SchaerbeekMunicipality.Api.Features.ChangeOfAddress;
using SchaerbeekMunicipality.Api.Features.Registration;
using SchaerbeekMunicipality.Api.Middleware;
using SchaerbeekMunicipality.Application;
using SchaerbeekMunicipality.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddMunicipalDatabaseHealthCheck();
builder.Services.AddApplication();
builder.Services.AddValidatorsFromAssembly(typeof(SchaerbeekMunicipality.Api.Features.Registration.AttachDocument.AttachDocumentRequestValidator).Assembly);

builder.Services.AddOpenApi();

var app = builder.Build();

await app.Services.InitializeDatabaseAsync();

app.UseDemoOfficerResolution();
app.UseDomainExceptionHandling();

app.MapRegistrationEndpoints();
app.MapBirthDeclarationEndpoints();
app.MapChangeOfAddressEndpoints();

app.MapOpenApi();

app.UseHealthCheckApiKeyProtection();
app.MapDefaultEndpoints();

app.Run();
