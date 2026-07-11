using MudBlazor.Services;
using SchaerbeekMunicipality.Application;
using SchaerbeekMunicipality.Infrastructure;
using SchaerbeekMunicipality.Web.Components;
using SchaerbeekMunicipality.Web.DesignSystem.Components.Dialogs;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddMudServices();

builder.Services.AddScoped<IAppDialogService, AppDialogService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
}

app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.UseHealthCheckApiKeyProtection();
app.MapDefaultEndpoints();

app.Run();
