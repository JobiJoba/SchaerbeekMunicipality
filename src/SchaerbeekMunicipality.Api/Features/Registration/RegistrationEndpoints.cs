using SchaerbeekMunicipality.Api.Features.Registration.ApproveCase;
using SchaerbeekMunicipality.Api.Features.Registration.AttachDocument;
using SchaerbeekMunicipality.Api.Features.Registration.ClaimRegistrationCase;
using SchaerbeekMunicipality.Api.Features.Registration.ConfirmRegistration;
using SchaerbeekMunicipality.Api.Features.Registration.ConvertBisNumber;
using SchaerbeekMunicipality.Api.Features.Registration.CorrectIdentity;
using SchaerbeekMunicipality.Api.Features.Registration.DeclareAddress;
using SchaerbeekMunicipality.Api.Features.Registration.DeclareReferenceAddress;
using SchaerbeekMunicipality.Api.Features.Registration.DownloadDocument;
using SchaerbeekMunicipality.Api.Features.Registration.GetCaseReviewChecklist;
using SchaerbeekMunicipality.Api.Features.Registration.GetRegistrationCase;
using SchaerbeekMunicipality.Api.Features.Registration.GetReviewDashboard;
using SchaerbeekMunicipality.Api.Features.Registration.IssueHouseholdComposition;
using SchaerbeekMunicipality.Api.Features.Registration.IssueResidenceCertificate;
using SchaerbeekMunicipality.Api.Features.Registration.LinkExistingPerson;
using SchaerbeekMunicipality.Api.Features.Registration.ListCaseAudit;
using SchaerbeekMunicipality.Api.Features.Registration.ListOutboundNotifications;
using SchaerbeekMunicipality.Api.Features.Registration.ListPendingPoliceVerifications;
using SchaerbeekMunicipality.Api.Features.Registration.ListRegistrationCases;
using SchaerbeekMunicipality.Api.Features.Registration.OpenRegistrationCase;
using SchaerbeekMunicipality.Api.Features.Registration.RecordBirthInformation;
using SchaerbeekMunicipality.Api.Features.Registration.RecordCivilStatus;
using SchaerbeekMunicipality.Api.Features.Registration.RecordHouseholdComposition;
using SchaerbeekMunicipality.Api.Features.Registration.RecordHousingSituation;
using SchaerbeekMunicipality.Api.Features.Registration.RecordIdentity;
using SchaerbeekMunicipality.Api.Features.Registration.RecordImmigrationDecision;
using SchaerbeekMunicipality.Api.Features.Registration.RecordPoliceResult;
using SchaerbeekMunicipality.Api.Features.Registration.RecordResidencePermit;
using SchaerbeekMunicipality.Api.Features.Registration.RejectCase;
using SchaerbeekMunicipality.Api.Features.Registration.ReleaseCaseLock;
using SchaerbeekMunicipality.Api.Features.Registration.RemoveDocument;
using SchaerbeekMunicipality.Api.Features.Registration.RequestPoliceVerification;
using SchaerbeekMunicipality.Api.Features.Registration.WaivePoliceVerification;
using SchaerbeekMunicipality.Api.Features.Registration.ResolveDuplicateInvestigation;
using SchaerbeekMunicipality.Api.Features.Registration.ResumeCase;
using SchaerbeekMunicipality.Api.Features.Registration.SearchNationalRegister;
using SchaerbeekMunicipality.Api.Features.Registration.SearchReferenceData;
using SchaerbeekMunicipality.Api.Features.Registration.SetResidenceCategory;
using SchaerbeekMunicipality.Api.Features.Registration.SuspendCase;

namespace SchaerbeekMunicipality.Api.Features.Registration;

public static class RegistrationEndpoints
{
    public static IEndpointRouteBuilder MapRegistrationEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/registration");

        group.MapGet("/cases", ListRegistrationCasesEndpoint.Handle)
            .WithName("ListRegistrationCases")
            .WithTags("Registration");

        group.MapPost("/cases", OpenRegistrationCaseEndpoint.Handle)
            .WithName("OpenRegistrationCase")
            .WithTags("Registration");

        group.MapGet("/cases/{id:guid}", GetRegistrationCaseEndpoint.Handle)
            .WithName("GetRegistrationCase")
            .WithTags("Registration");

        group.MapPost("/cases/{id:guid}/claim", ClaimRegistrationCaseEndpoint.Handle)
            .WithName("ClaimRegistrationCase")
            .WithTags("Registration");

        group.MapPost("/cases/{id:guid}/auto-claim", AutoClaimRegistrationCaseEndpoint.Handle)
            .WithName("AutoClaimRegistrationCase")
            .WithTags("Registration");

        group.MapPost("/cases/{id:guid}/release-lock", ReleaseCaseLockEndpoint.Handle)
            .WithName("ReleaseCaseLock")
            .WithTags("Registration");

        group.MapPost("/cases/{id:guid}/identity", RecordIdentityEndpoint.Handle)
            .WithName("RecordIdentity")
            .WithTags("Registration");

        group.MapPost("/cases/{id:guid}/identity/link", LinkExistingPersonEndpoint.Handle)
            .WithName("LinkExistingPerson")
            .WithTags("Registration");

        group.MapPost("/cases/{id:guid}/identity/convert-bis", ConvertBisNumberEndpoint.Handle)
            .WithName("ConvertBisNumber")
            .WithTags("Registration");

        group.MapGet("/national-register/search", SearchNationalRegisterEndpoint.Handle)
            .WithName("SearchNationalRegister")
            .WithTags("NationalRegister");

        group.MapPut("/cases/{id:guid}/identity", CorrectIdentityEndpoint.Handle)
            .WithName("CorrectIdentity")
            .WithTags("Registration");

        group.MapPost("/cases/{id:guid}/residence-category", SetResidenceCategoryEndpoint.Handle)
            .WithName("SetResidenceCategory")
            .WithTags("Registration");

        group.MapPost("/cases/{id:guid}/residence-permit", RecordResidencePermitEndpoint.Handle)
            .WithName("RecordResidencePermit")
            .WithTags("Registration");

        group.MapPost("/cases/{id:guid}/immigration-decision", RecordImmigrationDecisionEndpoint.Handle)
            .WithName("RecordImmigrationDecision")
            .WithTags("Registration");

        group.MapPost("/cases/{id:guid}/address", DeclareAddressEndpoint.Handle)
            .WithName("DeclareAddress")
            .WithTags("Registration");

        group.MapPost("/cases/{id:guid}/address/reference", DeclareReferenceAddressEndpoint.Handle)
            .WithName("DeclareReferenceAddress")
            .WithTags("Registration");

        group.MapPost("/cases/{id:guid}/housing-situation", RecordHousingSituationEndpoint.Handle)
            .WithName("RecordHousingSituation")
            .WithTags("Registration");

        group.MapPost("/cases/{id:guid}/household", RecordHouseholdCompositionEndpoint.Handle)
            .WithName("RecordHouseholdComposition")
            .WithTags("Registration");

        group.MapPost("/cases/{id:guid}/civil-status", RecordCivilStatusEndpoint.Handle)
            .WithName("RecordCivilStatus")
            .WithTags("Registration");

        group.MapPost("/cases/{id:guid}/birth-information", RecordBirthInformationEndpoint.Handle)
            .WithName("RecordBirthInformation")
            .WithTags("Registration");

        group.MapPost("/cases/{id:guid}/duplicate-investigation/resolve", ResolveDuplicateInvestigationEndpoint.Handle)
            .WithName("ResolveDuplicateInvestigation")
            .WithTags("Registration");

        group.MapGet("/municipalities", SearchReferenceDataEndpoints.ListMunicipalities)
            .WithName("ListMunicipalities")
            .WithTags("ReferenceData");

        group.MapGet("/streets", SearchReferenceDataEndpoints.SearchStreets)
            .WithName("SearchStreets")
            .WithTags("ReferenceData");

        group.MapPost("/cases/{id:guid}/documents", AttachDocumentEndpoint.Handle)
            .DisableAntiforgery()
            .WithName("AttachDocument")
            .WithTags("Registration");

        group.MapGet("/cases/{id:guid}/documents/{documentId:guid}", DownloadDocumentEndpoint.Handle)
            .WithName("DownloadDocument")
            .WithTags("Registration");

        group.MapDelete("/cases/{id:guid}/documents/{documentId:guid}", RemoveDocumentEndpoint.Handle)
            .WithName("RemoveDocument")
            .WithTags("Registration");

        group.MapPost("/cases/{id:guid}/police-verification", RequestPoliceVerificationEndpoint.Handle)
            .WithName("RequestPoliceVerification")
            .WithTags("PoliceVerification");

        group.MapPost("/cases/{id:guid}/police-verification/waive", WaivePoliceVerificationEndpoint.Handle)
            .WithName("WaivePoliceVerification")
            .WithTags("PoliceVerification");

        group.MapGet("/police-verifications/pending", ListPendingPoliceVerificationsEndpoint.Handle)
            .WithName("ListPendingPoliceVerifications")
            .WithTags("PoliceVerification");

        group.MapPost("/police-verifications/{requestId:guid}/result", RecordPoliceResultEndpoint.Handle)
            .WithName("RecordPoliceResult")
            .WithTags("PoliceVerification");

        group.MapGet("/cases/{id:guid}/review-checklist", GetCaseReviewChecklistEndpoint.Handle)
            .WithName("GetCaseReviewChecklist")
            .WithTags("Registration");

        group.MapPost("/cases/{id:guid}/approve", ApproveCaseEndpoint.Handle)
            .WithName("ApproveCase")
            .WithTags("Registration");

        group.MapPost("/cases/{id:guid}/reject", RejectCaseEndpoint.Handle)
            .WithName("RejectCase")
            .WithTags("Registration");

        group.MapPost("/cases/{id:guid}/suspend", SuspendCaseEndpoint.Handle)
            .WithName("SuspendCase")
            .WithTags("Registration");

        group.MapPost("/cases/{id:guid}/resume", ResumeCaseEndpoint.Handle)
            .WithName("ResumeCase")
            .WithTags("Registration");

        group.MapPost("/cases/{id:guid}/confirm-registration", ConfirmRegistrationEndpoint.Handle)
            .WithName("ConfirmRegistration")
            .WithTags("Registration");

        group.MapGet("/cases/{id:guid}/audit", ListCaseAuditEndpoint.Handle)
            .WithName("ListCaseAudit")
            .WithTags("Registration");

        group.MapGet("/review-dashboard", GetReviewDashboardEndpoint.Handle)
            .WithName("GetReviewDashboard")
            .WithTags("Registration");

        group.MapGet("/cases/{id:guid}/certificates/residence", IssueResidenceCertificateEndpoint.Handle)
            .WithName("IssueResidenceCertificate")
            .WithTags("Certificates");

        group.MapGet("/cases/{id:guid}/certificates/household-composition", IssueHouseholdCompositionEndpoint.Handle)
            .WithName("IssueHouseholdComposition")
            .WithTags("Certificates");

        group.MapGet("/outbound-notifications", ListOutboundNotificationsEndpoint.Handle)
            .WithName("ListOutboundNotifications")
            .WithTags("Notifications");

        return app;
    }
}