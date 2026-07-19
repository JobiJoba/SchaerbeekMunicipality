using MudBlazor.Services;
using SchaerbeekMunicipality.Api;
using SchaerbeekMunicipality.Domain.Registration;
using SchaerbeekMunicipality.Web;
using SchaerbeekMunicipality.Web.Api;
using SchaerbeekMunicipality.Web.Api.Registration;
using SchaerbeekMunicipality.Web.Components;
using SchaerbeekMunicipality.Web.DesignSystem.Components.Dialogs;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

if (!builder.Environment.IsDevelopment()) builder.AddMunicipalApiHost();

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
    app.UseExceptionHandler("/Error", true);
}

app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);

if (!app.Environment.IsDevelopment())
{
    app.UseMunicipalApiHost();
}
else
{
    // In development the API runs as a separate Aspire service ("http://api") and is
    // not reachable directly from the browser. Proxy certificate issuance through the
    // web host so the UI can open printable HTML in a new tab.
    app.MapGet(
        "/api/registration/cases/{id:guid}/certificates/residence",
        async (Guid id, IRegistrationApi registrationApi, CancellationToken cancellationToken) =>
        {
            try
            {
                var html = await registrationApi.IssueResidenceCertificateAsync(id, cancellationToken);
                return Results.Content(html, "text/html; charset=utf-8");
            }
            catch (KeyNotFoundException)
            {
                return Results.NotFound();
            }
            catch (InvalidRegistrationTransitionException ex)
            {
                return Results.Problem(
                    ex.Message,
                    statusCode: StatusCodes.Status409Conflict,
                    title: "Cannot issue certificate");
            }
        });

    app.MapGet(
        "/api/registration/cases/{id:guid}/certificates/household-composition",
        async (Guid id, IRegistrationApi registrationApi, CancellationToken cancellationToken) =>
        {
            try
            {
                var html = await registrationApi.IssueHouseholdCompositionCertificateAsync(id, cancellationToken);
                return Results.Content(html, "text/html; charset=utf-8");
            }
            catch (KeyNotFoundException)
            {
                return Results.NotFound();
            }
            catch (InvalidRegistrationTransitionException ex)
            {
                return Results.Problem(
                    ex.Message,
                    statusCode: StatusCodes.Status409Conflict,
                    title: "Cannot issue certificate");
            }
        });
}

app.MapMunicipalDocumentDownloadProxy();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.UseHealthCheckApiKeyProtection();
app.MapDefaultEndpoints();

app.Run();