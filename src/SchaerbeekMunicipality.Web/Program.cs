using MudBlazor.Services;
using SchaerbeekMunicipality.Api;
using SchaerbeekMunicipality.Web;
using SchaerbeekMunicipality.Web.Api;
using SchaerbeekMunicipality.Web.Components;
using SchaerbeekMunicipality.Web.DesignSystem.Components.Dialogs;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

if (!builder.Environment.IsDevelopment())
{
    builder.AddMunicipalApiHost();
}

builder.Services.AddWebPresentation();
builder.Services.AddMunicipalApiClients(builder.Environment);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddMudServices();

builder.Services.AddScoped<IAppDialogService, AppDialogService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    await app.InitializeMunicipalApiDatabaseAsync();
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
}

app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);

if (!app.Environment.IsDevelopment())
{
    app.UseMunicipalApiHost();
}

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.UseHealthCheckApiKeyProtection();
app.MapDefaultEndpoints();

app.Run();
