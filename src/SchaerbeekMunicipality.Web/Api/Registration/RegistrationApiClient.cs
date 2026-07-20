using SchaerbeekMunicipality.Application.Auth;
using SchaerbeekMunicipality.Application.Features.Registration.ApproveCase;
using SchaerbeekMunicipality.Application.Features.Registration.AttachDocument;
using SchaerbeekMunicipality.Application.Features.Registration.ClaimRegistrationCase;
using SchaerbeekMunicipality.Application.Features.Registration.ConfirmRegistration;
using SchaerbeekMunicipality.Application.Features.Registration.ConvertBisNumber;
using SchaerbeekMunicipality.Application.Features.Registration.CorrectIdentity;
using SchaerbeekMunicipality.Application.Features.Registration.DeclareAddress;
using SchaerbeekMunicipality.Application.Features.Registration.DeclareReferenceAddress;
using SchaerbeekMunicipality.Application.Features.Registration.GetCaseReviewChecklist;
using SchaerbeekMunicipality.Application.Features.Registration.GetRegistrationCase;
using SchaerbeekMunicipality.Application.Features.Registration.GetReviewDashboard;
using SchaerbeekMunicipality.Application.Features.Registration.LinkExistingPerson;
using SchaerbeekMunicipality.Application.Features.Registration.ListCaseAudit;
using SchaerbeekMunicipality.Application.Features.Registration.ListOutboundNotifications;
using SchaerbeekMunicipality.Application.Features.Registration.ListPendingPoliceVerifications;
using SchaerbeekMunicipality.Application.Features.Registration.ListRegistrationCases;
using SchaerbeekMunicipality.Application.Features.Registration.OpenRegistrationCase;
using SchaerbeekMunicipality.Application.Features.Registration.RecordBirthInformation;
using SchaerbeekMunicipality.Application.Features.Registration.RecordCivilStatus;
using SchaerbeekMunicipality.Application.Features.Registration.RecordHouseholdComposition;
using SchaerbeekMunicipality.Application.Features.Registration.RecordHousingSituation;
using SchaerbeekMunicipality.Application.Features.Registration.RecordIdentity;
using SchaerbeekMunicipality.Application.Features.Registration.RecordImmigrationDecision;
using SchaerbeekMunicipality.Application.Features.Registration.RecordPoliceResult;
using SchaerbeekMunicipality.Application.Features.Registration.RecordResidencePermit;
using SchaerbeekMunicipality.Application.Features.Registration.RejectCase;
using SchaerbeekMunicipality.Application.Features.Registration.ReleaseCaseLock;
using SchaerbeekMunicipality.Application.Features.Registration.RemoveDocument;
using SchaerbeekMunicipality.Application.Features.Registration.RequestPoliceVerification;
using SchaerbeekMunicipality.Application.Features.Registration.WaivePoliceVerification;
using SchaerbeekMunicipality.Application.Features.Registration.ResolveDuplicateInvestigation;
using SchaerbeekMunicipality.Application.Features.Registration.ResumeCase;
using SchaerbeekMunicipality.Application.Features.Registration.SearchNationalRegister;
using SchaerbeekMunicipality.Application.Features.Registration.SearchReferenceData;
using SchaerbeekMunicipality.Application.Features.Registration.SetResidenceCategory;
using SchaerbeekMunicipality.Application.Features.Registration.SuspendCase;
using SchaerbeekMunicipality.Domain.Documents;

namespace SchaerbeekMunicipality.Web.Api.Registration;

public sealed class RegistrationApiClient(HttpClient httpClient, ICurrentOfficer currentOfficer)
    : MunicipalApiClientBase(httpClient, currentOfficer), IRegistrationApi
{
    private const string BasePath = "/api/registration";

    public Task<IReadOnlyList<RegistrationCaseSummary>> ListCasesAsync(CancellationToken cancellationToken = default)
    {
        return GetJsonAsync<IReadOnlyList<RegistrationCaseSummary>>($"{BasePath}/cases", cancellationToken);
    }

    public Task<OpenRegistrationCaseResponse> OpenCaseAsync(
        OpenRegistrationCaseRequest request,
        CancellationToken cancellationToken = default)
    {
        return PostJsonAsync<OpenRegistrationCaseRequest, OpenRegistrationCaseResponse>(
            $"{BasePath}/cases",
            request,
            cancellationToken);
    }

    public Task<RegistrationCaseDetailDto> GetCaseAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return GetJsonAsync<RegistrationCaseDetailDto>($"{BasePath}/cases/{id}", cancellationToken);
    }

    public Task<ClaimRegistrationCaseResponse> ClaimCaseAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return PostJsonAsync<ClaimRegistrationCaseResponse>($"{BasePath}/cases/{id}/claim", cancellationToken);
    }

    public Task<ClaimRegistrationCaseResponse?> TryAutoClaimCaseAsync(Guid id,
        CancellationToken cancellationToken = default)
    {
        return PostJsonOptionalAsync<ClaimRegistrationCaseResponse>($"{BasePath}/cases/{id}/auto-claim",
            cancellationToken);
    }

    public Task<ReleaseCaseLockResponse> ReleaseCaseLockAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return PostJsonAsync<ReleaseCaseLockResponse>($"{BasePath}/cases/{id}/release-lock", cancellationToken);
    }

    public Task<RecordIdentityResponse> RecordIdentityAsync(
        Guid id,
        RecordIdentityRequest request,
        CancellationToken cancellationToken = default)
    {
        return PostJsonAsync<RecordIdentityRequest, RecordIdentityResponse>(
            $"{BasePath}/cases/{id}/identity",
            request,
            cancellationToken);
    }

    public Task<LinkExistingPersonResponse> LinkExistingPersonAsync(
        Guid id,
        LinkExistingPersonRequest request,
        CancellationToken cancellationToken = default)
    {
        return PostJsonAsync<LinkExistingPersonRequest, LinkExistingPersonResponse>(
            $"{BasePath}/cases/{id}/identity/link",
            request,
            cancellationToken);
    }

    public Task<ConvertBisNumberResponse> ConvertBisNumberAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return PostJsonAsync<ConvertBisNumberResponse>($"{BasePath}/cases/{id}/identity/convert-bis",
            cancellationToken);
    }

    public Task<SearchNationalRegisterResponse> SearchNationalRegisterAsync(
        SearchNationalRegisterRequest request,
        CancellationToken cancellationToken = default)
    {
        var query = BuildQuery(
            ("GivenName", request.GivenName),
            ("FamilyName", request.FamilyName),
            ("BirthDate", request.BirthDate?.ToString("O")),
            ("Page", request.Page.ToString()),
            ("PageSize", request.PageSize.ToString()),
            ("ExcludeDeceased", request.ExcludeDeceased ? "true" : null));

        return GetJsonAsync<SearchNationalRegisterResponse>($"{BasePath}/national-register/search{query}",
            cancellationToken);
    }

    public Task<CorrectIdentityResponse> CorrectIdentityAsync(
        Guid id,
        RecordIdentityRequest request,
        CancellationToken cancellationToken = default)
    {
        return PutJsonAsync<RecordIdentityRequest, CorrectIdentityResponse>(
            $"{BasePath}/cases/{id}/identity",
            request,
            cancellationToken);
    }

    public Task<SetResidenceCategoryResponse> SetResidenceCategoryAsync(
        Guid id,
        SetResidenceCategoryRequest request,
        CancellationToken cancellationToken = default)
    {
        return PostJsonAsync<SetResidenceCategoryRequest, SetResidenceCategoryResponse>(
            $"{BasePath}/cases/{id}/residence-category",
            request,
            cancellationToken);
    }

    public Task<RecordResidencePermitResponse> RecordResidencePermitAsync(
        Guid id,
        RecordResidencePermitRequest request,
        CancellationToken cancellationToken = default)
    {
        return PostJsonAsync<RecordResidencePermitRequest, RecordResidencePermitResponse>(
            $"{BasePath}/cases/{id}/residence-permit",
            request,
            cancellationToken);
    }

    public Task<RecordImmigrationDecisionResponse> RecordImmigrationDecisionAsync(
        Guid id,
        RecordImmigrationDecisionRequest request,
        CancellationToken cancellationToken = default)
    {
        return PostJsonAsync<RecordImmigrationDecisionRequest, RecordImmigrationDecisionResponse>(
            $"{BasePath}/cases/{id}/immigration-decision",
            request,
            cancellationToken);
    }

    public Task<DeclareAddressResponse> DeclareAddressAsync(
        Guid id,
        DeclareAddressRequest request,
        CancellationToken cancellationToken = default)
    {
        return PostJsonAsync<DeclareAddressRequest, DeclareAddressResponse>(
            $"{BasePath}/cases/{id}/address",
            request,
            cancellationToken);
    }

    public Task<DeclareReferenceAddressResponse> DeclareReferenceAddressAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return PostJsonAsync<DeclareReferenceAddressResponse>(
            $"{BasePath}/cases/{id}/address/reference",
            cancellationToken);
    }

    public Task<RecordHousingSituationResponse> RecordHousingSituationAsync(
        Guid id,
        RecordHousingSituationRequest request,
        CancellationToken cancellationToken = default)
    {
        return PostJsonAsync<RecordHousingSituationRequest, RecordHousingSituationResponse>(
            $"{BasePath}/cases/{id}/housing-situation",
            request,
            cancellationToken);
    }

    public Task<RecordHouseholdCompositionResponse> RecordHouseholdCompositionAsync(
        Guid id,
        RecordHouseholdCompositionRequest request,
        CancellationToken cancellationToken = default)
    {
        return PostJsonAsync<RecordHouseholdCompositionRequest, RecordHouseholdCompositionResponse>(
            $"{BasePath}/cases/{id}/household",
            request,
            cancellationToken);
    }

    public Task<RecordCivilStatusResponse> RecordCivilStatusAsync(
        Guid id,
        RecordCivilStatusRequest request,
        CancellationToken cancellationToken = default)
    {
        return PostJsonAsync<RecordCivilStatusRequest, RecordCivilStatusResponse>(
            $"{BasePath}/cases/{id}/civil-status",
            request,
            cancellationToken);
    }

    public Task<RecordBirthInformationResponse> RecordBirthInformationAsync(
        Guid id,
        RecordBirthInformationRequest request,
        CancellationToken cancellationToken = default)
    {
        return PostJsonAsync<RecordBirthInformationRequest, RecordBirthInformationResponse>(
            $"{BasePath}/cases/{id}/birth-information",
            request,
            cancellationToken);
    }

    public Task<ResolveDuplicateInvestigationResponse> ResolveDuplicateInvestigationAsync(
        Guid id,
        ResolveDuplicateInvestigationRequest request,
        CancellationToken cancellationToken = default)
    {
        return PostJsonAsync<ResolveDuplicateInvestigationRequest, ResolveDuplicateInvestigationResponse>(
            $"{BasePath}/cases/{id}/duplicate-investigation/resolve",
            request,
            cancellationToken);
    }

    public Task<IReadOnlyList<MunicipalityDto>> ListMunicipalitiesAsync(
        string? search = null,
        CancellationToken cancellationToken = default)
    {
        var query = BuildQuery(("search", search));
        return GetJsonAsync<IReadOnlyList<MunicipalityDto>>($"{BasePath}/municipalities{query}", cancellationToken);
    }

    public Task<IReadOnlyList<StreetDto>> SearchStreetsAsync(
        string postalCode,
        string? search = null,
        CancellationToken cancellationToken = default)
    {
        var query = BuildQuery(("postalCode", postalCode), ("search", search));
        return GetJsonAsync<IReadOnlyList<StreetDto>>($"{BasePath}/streets{query}", cancellationToken);
    }

    public Task<AttachDocumentResponse> AttachDocumentAsync(
        Guid id,
        DocumentType documentType,
        Stream fileStream,
        string fileName,
        CancellationToken cancellationToken = default)
    {
        var query = BuildQuery(("documentType", documentType.ToString()));
        return PostMultipartFileAsync<AttachDocumentResponse>(
            $"{BasePath}/cases/{id}/documents{query}",
            fileStream,
            fileName,
            cancellationToken);
    }

    public Task<Stream> DownloadDocumentAsync(
        Guid id,
        Guid documentId,
        CancellationToken cancellationToken = default)
    {
        return DownloadStreamAsync($"{BasePath}/cases/{id}/documents/{documentId}", cancellationToken);
    }

    public Task<RemoveDocumentResponse> RemoveDocumentAsync(
        Guid id,
        Guid documentId,
        CancellationToken cancellationToken = default)
    {
        return DeleteJsonAsync<RemoveDocumentResponse>($"{BasePath}/cases/{id}/documents/{documentId}",
            cancellationToken);
    }

    public Task<RequestPoliceVerificationResponse> RequestPoliceVerificationAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return PostJsonAsync<RequestPoliceVerificationResponse>($"{BasePath}/cases/{id}/police-verification",
            cancellationToken);
    }

    public Task<WaivePoliceVerificationResponse> WaivePoliceVerificationAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return PostJsonAsync<WaivePoliceVerificationResponse>(
            $"{BasePath}/cases/{id}/police-verification/waive",
            cancellationToken);
    }

    public Task<ListPendingPoliceVerificationsResponse> ListPendingPoliceVerificationsAsync(
        CancellationToken cancellationToken = default)
    {
        return GetJsonAsync<ListPendingPoliceVerificationsResponse>($"{BasePath}/police-verifications/pending",
            cancellationToken);
    }

    public Task<RecordPoliceResultResponse> RecordPoliceResultAsync(
        Guid requestId,
        RecordPoliceResultRequest request,
        CancellationToken cancellationToken = default)
    {
        return PostJsonAsync<RecordPoliceResultRequest, RecordPoliceResultResponse>(
            $"{BasePath}/police-verifications/{requestId}/result",
            request,
            cancellationToken);
    }

    public Task<GetCaseReviewChecklistResponse> GetCaseReviewChecklistAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return GetJsonAsync<GetCaseReviewChecklistResponse>($"{BasePath}/cases/{id}/review-checklist",
            cancellationToken);
    }

    public Task<ApproveCaseResponse> ApproveCaseAsync(
        Guid id,
        ApproveCaseRequest request,
        CancellationToken cancellationToken = default)
    {
        return PostJsonAsync<ApproveCaseRequest, ApproveCaseResponse>(
            $"{BasePath}/cases/{id}/approve",
            request,
            cancellationToken);
    }

    public Task<RejectCaseResponse> RejectCaseAsync(
        Guid id,
        RejectCaseRequest request,
        CancellationToken cancellationToken = default)
    {
        return PostJsonAsync<RejectCaseRequest, RejectCaseResponse>(
            $"{BasePath}/cases/{id}/reject",
            request,
            cancellationToken);
    }

    public Task<SuspendCaseResponse> SuspendCaseAsync(
        Guid id,
        SuspendCaseRequest request,
        CancellationToken cancellationToken = default)
    {
        return PostJsonAsync<SuspendCaseRequest, SuspendCaseResponse>(
            $"{BasePath}/cases/{id}/suspend",
            request,
            cancellationToken);
    }

    public Task<ResumeCaseResponse> ResumeCaseAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return PostJsonAsync<ResumeCaseResponse>($"{BasePath}/cases/{id}/resume", cancellationToken);
    }

    public Task<ConfirmRegistrationResponse> ConfirmRegistrationAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return PostJsonAsync<ConfirmRegistrationResponse>($"{BasePath}/cases/{id}/confirm-registration",
            cancellationToken);
    }

    public Task<ListCaseAuditResponse> ListCaseAuditAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return GetJsonAsync<ListCaseAuditResponse>($"{BasePath}/cases/{id}/audit", cancellationToken);
    }

    public Task<GetReviewDashboardResponse> GetReviewDashboardAsync(CancellationToken cancellationToken = default)
    {
        return GetJsonAsync<GetReviewDashboardResponse>($"{BasePath}/review-dashboard", cancellationToken);
    }

    public Task<string> IssueResidenceCertificateAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return GetStringAsync($"{BasePath}/cases/{id}/certificates/residence", cancellationToken);
    }

    public Task<string> IssueHouseholdCompositionCertificateAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return GetStringAsync($"{BasePath}/cases/{id}/certificates/household-composition", cancellationToken);
    }

    public Task<ListOutboundNotificationsResponse> ListOutboundNotificationsAsync(
        Guid? caseId = null,
        CancellationToken cancellationToken = default)
    {
        var query = BuildQuery(("caseId", caseId?.ToString()));
        return GetJsonAsync<ListOutboundNotificationsResponse>($"{BasePath}/outbound-notifications{query}",
            cancellationToken);
    }
}