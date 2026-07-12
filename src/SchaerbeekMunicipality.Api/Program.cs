using SchaerbeekMunicipality.Api;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.AddMunicipalApiHost();

var app = builder.Build();

await app.InitializeMunicipalApiDatabaseAsync();
app.UseMunicipalApiHost();

app.UseHealthCheckApiKeyProtection();
app.MapDefaultEndpoints();

app.Run();
