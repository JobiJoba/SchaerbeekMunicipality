using FluentValidation;
using MudBlazor.Services;
using SchaerbeekMunicipality.Infrastructure;
using SchaerbeekMunicipality.Web.Auth;
using SchaerbeekMunicipality.Web.Components;
using SchaerbeekMunicipality.Web.DesignSystem.Components.Dialogs;
using SchaerbeekMunicipality.Web.Features.Registration;
using SchaerbeekMunicipality.Web.Features.Registration.AttachDocument;
using SchaerbeekMunicipality.Web.Features.Registration.GetRegistrationCase;
using SchaerbeekMunicipality.Web.Features.Registration.ListRegistrationCases;
using SchaerbeekMunicipality.Web.Features.Registration.OpenRegistrationCase;
using SchaerbeekMunicipality.Web.Features.Registration.RecordIdentity;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddMudServices();

builder.Services.AddScoped<IAppDialogService, AppDialogService>();

builder.Services.AddValidatorsFromAssemblyContaining<OpenRegistrationCaseValidator>();

builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddScoped<ICurrentOfficer, CurrentOfficer>();
builder.Services.AddScoped<ListRegistrationCasesHandler>();
builder.Services.AddScoped<OpenRegistrationCaseHandler>();
builder.Services.AddScoped<GetRegistrationCaseHandler>();
builder.Services.AddScoped<RecordIdentityHandler>();
builder.Services.AddScoped<AttachDocumentHandler>();

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
