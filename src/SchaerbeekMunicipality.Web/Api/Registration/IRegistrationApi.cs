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

public interface IRegistrationApi
{
    Task<IReadOnlyList<RegistrationCaseSummary>> ListCasesAsync(CancellationToken cancellationToken = default);

    Task<OpenRegistrationCaseResponse> OpenCaseAsync(
        OpenRegistrationCaseRequest request,
        CancellationToken cancellationToken = default);

    Task<RegistrationCaseDetailDto> GetCaseAsync(Guid id, CancellationToken cancellationToken = default);

    Task<ClaimRegistrationCaseResponse> ClaimCaseAsync(Guid id, CancellationToken cancellationToken = default);

    Task<ClaimRegistrationCaseResponse?> TryAutoClaimCaseAsync(Guid id, CancellationToken cancellationToken = default);

    Task<ReleaseCaseLockResponse> ReleaseCaseLockAsync(Guid id, CancellationToken cancellationToken = default);

    Task<RecordIdentityResponse> RecordIdentityAsync(
        Guid id,
        RecordIdentityRequest request,
        CancellationToken cancellationToken = default);

    Task<LinkExistingPersonResponse> LinkExistingPersonAsync(
        Guid id,
        LinkExistingPersonRequest request,
        CancellationToken cancellationToken = default);

    Task<ConvertBisNumberResponse> ConvertBisNumberAsync(Guid id, CancellationToken cancellationToken = default);

    Task<SearchNationalRegisterResponse> SearchNationalRegisterAsync(
        SearchNationalRegisterRequest request,
        CancellationToken cancellationToken = default);

    Task<CorrectIdentityResponse> CorrectIdentityAsync(
        Guid id,
        RecordIdentityRequest request,
        CancellationToken cancellationToken = default);

    Task<SetResidenceCategoryResponse> SetResidenceCategoryAsync(
        Guid id,
        SetResidenceCategoryRequest request,
        CancellationToken cancellationToken = default);

    Task<RecordResidencePermitResponse> RecordResidencePermitAsync(
        Guid id,
        RecordResidencePermitRequest request,
        CancellationToken cancellationToken = default);

    Task<RecordImmigrationDecisionResponse> RecordImmigrationDecisionAsync(
        Guid id,
        RecordImmigrationDecisionRequest request,
        CancellationToken cancellationToken = default);

    Task<DeclareAddressResponse> DeclareAddressAsync(
        Guid id,
        DeclareAddressRequest request,
        CancellationToken cancellationToken = default);

    Task<DeclareReferenceAddressResponse> DeclareReferenceAddressAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<RecordHousingSituationResponse> RecordHousingSituationAsync(
        Guid id,
        RecordHousingSituationRequest request,
        CancellationToken cancellationToken = default);

    Task<RecordHouseholdCompositionResponse> RecordHouseholdCompositionAsync(
        Guid id,
        RecordHouseholdCompositionRequest request,
        CancellationToken cancellationToken = default);

    Task<RecordCivilStatusResponse> RecordCivilStatusAsync(
        Guid id,
        RecordCivilStatusRequest request,
        CancellationToken cancellationToken = default);

    Task<RecordBirthInformationResponse> RecordBirthInformationAsync(
        Guid id,
        RecordBirthInformationRequest request,
        CancellationToken cancellationToken = default);

    Task<ResolveDuplicateInvestigationResponse> ResolveDuplicateInvestigationAsync(
        Guid id,
        ResolveDuplicateInvestigationRequest request,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<MunicipalityDto>> ListMunicipalitiesAsync(
        string? search = null,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<StreetDto>> SearchStreetsAsync(
        string postalCode,
        string? search = null,
        CancellationToken cancellationToken = default);

    Task<AttachDocumentResponse> AttachDocumentAsync(
        Guid id,
        DocumentType documentType,
        Stream fileStream,
        string fileName,
        CancellationToken cancellationToken = default);

    Task<Stream> DownloadDocumentAsync(
        Guid id,
        Guid documentId,
        CancellationToken cancellationToken = default);

    Task<RemoveDocumentResponse> RemoveDocumentAsync(
        Guid id,
        Guid documentId,
        CancellationToken cancellationToken = default);

    Task<RequestPoliceVerificationResponse> RequestPoliceVerificationAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<WaivePoliceVerificationResponse> WaivePoliceVerificationAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<ListPendingPoliceVerificationsResponse> ListPendingPoliceVerificationsAsync(
        CancellationToken cancellationToken = default);

    Task<RecordPoliceResultResponse> RecordPoliceResultAsync(
        Guid requestId,
        RecordPoliceResultRequest request,
        CancellationToken cancellationToken = default);

    Task<GetCaseReviewChecklistResponse> GetCaseReviewChecklistAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<ApproveCaseResponse> ApproveCaseAsync(
        Guid id,
        ApproveCaseRequest request,
        CancellationToken cancellationToken = default);

    Task<RejectCaseResponse> RejectCaseAsync(
        Guid id,
        RejectCaseRequest request,
        CancellationToken cancellationToken = default);

    Task<SuspendCaseResponse> SuspendCaseAsync(
        Guid id,
        SuspendCaseRequest request,
        CancellationToken cancellationToken = default);

    Task<ResumeCaseResponse> ResumeCaseAsync(Guid id, CancellationToken cancellationToken = default);

    Task<ConfirmRegistrationResponse> ConfirmRegistrationAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<ListCaseAuditResponse> ListCaseAuditAsync(Guid id, CancellationToken cancellationToken = default);

    Task<GetReviewDashboardResponse> GetReviewDashboardAsync(CancellationToken cancellationToken = default);

    Task<string> IssueResidenceCertificateAsync(Guid id, CancellationToken cancellationToken = default);

    Task<string> IssueHouseholdCompositionCertificateAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<ListOutboundNotificationsResponse> ListOutboundNotificationsAsync(
        Guid? caseId = null,
        CancellationToken cancellationToken = default);
}