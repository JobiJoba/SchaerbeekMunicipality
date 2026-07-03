using MudBlazor.Services;
using SchaerbeekMunicipality.Infrastructure;
using SchaerbeekMunicipality.Web.Auth;
using SchaerbeekMunicipality.Web.Components;
using SchaerbeekMunicipality.Web.Features.Registration;
using SchaerbeekMunicipality.Web.Features.Registration.ListRegistrationCases;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddMudServices();

builder.Services.AddScoped<ICurrentOfficer, CurrentOfficer>();
builder.Services.AddScoped<ListRegistrationCasesHandler>();

builder.Services.AddOpenApi();

var app = builder.Build();

await app.Services.InitializeDatabaseAsync(app.Environment);

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
}

app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapRegistrationEndpoints();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapDefaultEndpoints();

app.Run();

public partial class Program;
