using FluentValidation;
using MudBlazor.Services;
using SchaerbeekMunicipality.Infrastructure;
using SchaerbeekMunicipality.Web.Auth;
using SchaerbeekMunicipality.Web.Components;
using SchaerbeekMunicipality.Web.DesignSystem.Components.Dialogs;
using SchaerbeekMunicipality.Web.Features.Registration;
using SchaerbeekMunicipality.Web.Features.Registration.ClaimRegistrationCase;
using SchaerbeekMunicipality.Web.Features.Registration.ReleaseCaseLock;
using SchaerbeekMunicipality.Web.Features.Registration.AttachDocument;
using SchaerbeekMunicipality.Web.Features.Registration.CorrectIdentity;
using SchaerbeekMunicipality.Web.Features.Registration.DeclareAddress;
using SchaerbeekMunicipality.Web.Features.Registration.DownloadDocument;
using SchaerbeekMunicipality.Web.Features.Registration.GetRegistrationCase;
using SchaerbeekMunicipality.Web.Features.Registration.ConvertBisNumber;
using SchaerbeekMunicipality.Web.Features.Registration.LinkExistingPerson;
using SchaerbeekMunicipality.Web.Features.Registration.ListRegistrationCases;
using SchaerbeekMunicipality.Web.Features.Registration.OpenRegistrationCase;
using SchaerbeekMunicipality.Web.Features.Registration.RecordCivilStatus;
using SchaerbeekMunicipality.Web.Features.Registration.RecordHouseholdComposition;
using SchaerbeekMunicipality.Web.Features.Registration.RecordHousingSituation;
using SchaerbeekMunicipality.Web.Features.Registration.RecordIdentity;
using SchaerbeekMunicipality.Web.Features.Registration.RecordBirthInformation;
using SchaerbeekMunicipality.Web.Features.Registration.ResolveDuplicateInvestigation;
using SchaerbeekMunicipality.Web.Features.Registration.RecordImmigrationDecision;
using SchaerbeekMunicipality.Web.Features.Registration.RecordResidencePermit;
using SchaerbeekMunicipality.Web.Features.Registration.RemoveDocument;
using SchaerbeekMunicipality.Web.Features.Registration.SearchReferenceData;
using SchaerbeekMunicipality.Web.Features.Registration.SearchNationalRegister;
using SchaerbeekMunicipality.Web.Features.Registration.ListPendingPoliceVerifications;
using SchaerbeekMunicipality.Web.Features.Registration.RecordPoliceResult;
using SchaerbeekMunicipality.Web.Features.Registration.RequestPoliceVerification;
using SchaerbeekMunicipality.Web.Features.Registration.ApproveCase;
using SchaerbeekMunicipality.Web.Features.Registration.ConfirmRegistration;
using SchaerbeekMunicipality.Web.Features.Registration.GetCaseReviewChecklist;
using SchaerbeekMunicipality.Web.Features.Registration.GetReviewDashboard;
using SchaerbeekMunicipality.Web.Features.Registration.ListCaseAudit;
using SchaerbeekMunicipality.Web.Features.Registration.RejectCase;
using SchaerbeekMunicipality.Web.Features.Registration.ResumeCase;
using SchaerbeekMunicipality.Web.Features.Registration.SuspendCase;
using SchaerbeekMunicipality.Web.Features.Registration.IssueResidenceCertificate;
using SchaerbeekMunicipality.Web.Features.Registration.IssueHouseholdComposition;
using SchaerbeekMunicipality.Web.Features.Registration.ListOutboundNotifications;
using SchaerbeekMunicipality.Web.Features.Registration.SetResidenceCategory;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddMunicipalDatabaseHealthCheck();

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddMudServices();

builder.Services.AddScoped<IAppDialogService, AppDialogService>();

builder.Services.AddValidatorsFromAssemblyContaining<OpenRegistrationCaseValidator>();

builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddScoped<RegistrationCaseAuthorization>();
builder.Services.AddScoped<RegistrationCaseGuard>();
builder.Services.AddScoped<ICurrentOfficer, CurrentOfficer>();
builder.Services.AddScoped<ListRegistrationCasesHandler>();
builder.Services.AddScoped<OpenRegistrationCaseHandler>();
builder.Services.AddScoped<GetRegistrationCaseHandler>();
builder.Services.AddScoped<ClaimRegistrationCaseHandler>();
builder.Services.AddScoped<ReleaseCaseLockHandler>();
builder.Services.AddScoped<RecordIdentityHandler>();
builder.Services.AddScoped<LinkExistingPersonHandler>();
builder.Services.AddScoped<ConvertBisNumberHandler>();
builder.Services.AddScoped<SearchNationalRegisterHandler>();
builder.Services.AddScoped<CorrectIdentityHandler>();
builder.Services.AddScoped<AttachDocumentHandler>();
builder.Services.AddScoped<DownloadDocumentHandler>();
builder.Services.AddScoped<RemoveDocumentHandler>();
builder.Services.AddScoped<RegistrationResidenceEvaluator>();
builder.Services.AddScoped<RegistrationExceptionEvaluator>();
builder.Services.AddScoped<SetResidenceCategoryHandler>();
builder.Services.AddScoped<RecordResidencePermitHandler>();
builder.Services.AddScoped<RecordImmigrationDecisionHandler>();
builder.Services.AddScoped<DeclareAddressHandler>();
builder.Services.AddScoped<RecordHousingSituationHandler>();
builder.Services.AddScoped<RecordHouseholdCompositionHandler>();
builder.Services.AddScoped<RecordCivilStatusHandler>();
builder.Services.AddScoped<RecordBirthInformationHandler>();
builder.Services.AddScoped<ResolveDuplicateInvestigationHandler>();
builder.Services.AddScoped<RequestPoliceVerificationHandler>();
builder.Services.AddScoped<ListPendingPoliceVerificationsHandler>();
builder.Services.AddScoped<RecordPoliceResultHandler>();
builder.Services.AddScoped<GetCaseReviewChecklistHandler>();
builder.Services.AddScoped<ApproveCaseHandler>();
builder.Services.AddScoped<RejectCaseHandler>();
builder.Services.AddScoped<SuspendCaseHandler>();
builder.Services.AddScoped<ResumeCaseHandler>();
builder.Services.AddScoped<ConfirmRegistrationHandler>();
builder.Services.AddScoped<GetReviewDashboardHandler>();
builder.Services.AddScoped<ListCaseAuditHandler>();
builder.Services.AddScoped<CaseAuditRecorder>();
builder.Services.AddScoped<IssueResidenceCertificateHandler>();
builder.Services.AddScoped<IssueHouseholdCompositionHandler>();
builder.Services.AddScoped<ListOutboundNotificationsHandler>();
builder.Services.AddScoped<SearchMunicipalitiesHandler>();
builder.Services.AddScoped<SearchStreetsHandler>();

builder.Services.AddOpenApi();

var app = builder.Build();

await app.Services.InitializeDatabaseAsync();

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

app.MapOpenApi();

app.UseHealthCheckApiKeyProtection();
app.MapDefaultEndpoints();

app.Run();

public partial class Program;
