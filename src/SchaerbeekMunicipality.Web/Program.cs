using FluentValidation;
using MudBlazor.Services;
using SchaerbeekMunicipality.Infrastructure;
using SchaerbeekMunicipality.Web.Auth;
using SchaerbeekMunicipality.Web.Components;
using SchaerbeekMunicipality.Web.DesignSystem.Components.Dialogs;
using SchaerbeekMunicipality.Web.Features.Registration;
using SchaerbeekMunicipality.Web.Features.Registration.AttachDocument;
using SchaerbeekMunicipality.Web.Features.Registration.CorrectIdentity;
using SchaerbeekMunicipality.Web.Features.Registration.DeclareAddress;
using SchaerbeekMunicipality.Web.Features.Registration.DownloadDocument;
using SchaerbeekMunicipality.Web.Features.Registration.GetRegistrationCase;
using SchaerbeekMunicipality.Web.Features.Registration.ListRegistrationCases;
using SchaerbeekMunicipality.Web.Features.Registration.OpenRegistrationCase;
using SchaerbeekMunicipality.Web.Features.Registration.RecordCivilStatus;
using SchaerbeekMunicipality.Web.Features.Registration.RecordHouseholdComposition;
using SchaerbeekMunicipality.Web.Features.Registration.RecordHousingSituation;
using SchaerbeekMunicipality.Web.Features.Registration.RecordIdentity;
using SchaerbeekMunicipality.Web.Features.Registration.RecordImmigrationDecision;
using SchaerbeekMunicipality.Web.Features.Registration.RecordResidencePermit;
using SchaerbeekMunicipality.Web.Features.Registration.RemoveDocument;
using SchaerbeekMunicipality.Web.Features.Registration.SearchReferenceData;
using SchaerbeekMunicipality.Web.Features.Registration.SetResidenceCategory;

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
builder.Services.AddScoped<CorrectIdentityHandler>();
builder.Services.AddScoped<AttachDocumentHandler>();
builder.Services.AddScoped<DownloadDocumentHandler>();
builder.Services.AddScoped<RemoveDocumentHandler>();
builder.Services.AddScoped<RegistrationResidenceEvaluator>();
builder.Services.AddScoped<SetResidenceCategoryHandler>();
builder.Services.AddScoped<RecordResidencePermitHandler>();
builder.Services.AddScoped<RecordImmigrationDecisionHandler>();
builder.Services.AddScoped<DeclareAddressHandler>();
builder.Services.AddScoped<RecordHousingSituationHandler>();
builder.Services.AddScoped<RecordHouseholdCompositionHandler>();
builder.Services.AddScoped<RecordCivilStatusHandler>();
builder.Services.AddScoped<SearchMunicipalitiesHandler>();
builder.Services.AddScoped<SearchStreetsHandler>();

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
