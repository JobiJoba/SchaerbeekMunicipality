using SchaerbeekMunicipality.Application.Features.IdentityDocuments.AdvanceDocumentRequestStatus;
using SchaerbeekMunicipality.Application.Features.IdentityDocuments.AttachApplicantPhoto;
using SchaerbeekMunicipality.Application.Features.IdentityDocuments.CancelDocumentRequest;
using SchaerbeekMunicipality.Application.Features.IdentityDocuments.ClaimDocumentRequestCase;
using SchaerbeekMunicipality.Application.Features.IdentityDocuments.GetDocumentRequestCase;
using SchaerbeekMunicipality.Application.Features.IdentityDocuments.IssueDocument;
using SchaerbeekMunicipality.Application.Features.IdentityDocuments.ListDocumentRequestCases;
using SchaerbeekMunicipality.Application.Features.IdentityDocuments.OpenDocumentRequestCase;
using SchaerbeekMunicipality.Application.Features.IdentityDocuments.RecordFeePayment;
using SchaerbeekMunicipality.Application.Features.IdentityDocuments.ReleaseCaseLock;
using SchaerbeekMunicipality.Application.Features.IdentityDocuments.RemoveDocument;
using SchaerbeekMunicipality.Application.Features.IdentityDocuments.ResolveRegisteredPerson;

namespace SchaerbeekMunicipality.Web.Api.IdentityDocuments;

public interface IIdentityDocumentsApi
{
    Task<IReadOnlyList<DocumentRequestCaseListItem>> ListRequestsAsync(CancellationToken cancellationToken = default);

    Task<OpenDocumentRequestCaseResponse> OpenRequestAsync(
        OpenDocumentRequestCaseRequest request,
        CancellationToken cancellationToken = default);

    Task<ResolveRegisteredPersonResponse> ResolveRegisteredPersonAsync(
        ResolveRegisteredPersonRequest request,
        CancellationToken cancellationToken = default);

    Task<DocumentRequestCaseDetailDto> GetRequestAsync(Guid id, CancellationToken cancellationToken = default);

    Task<ClaimDocumentRequestCaseResponse> ClaimRequestAsync(Guid id, CancellationToken cancellationToken = default);

    Task<ClaimDocumentRequestCaseResponse?> TryAutoClaimRequestAsync(Guid id,
        CancellationToken cancellationToken = default);

    Task<ReleaseCaseLockResponse> ReleaseCaseLockAsync(Guid id, CancellationToken cancellationToken = default);

    Task<AttachApplicantPhotoResponse> AttachApplicantPhotoAsync(
        Guid id,
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

    Task<RecordFeePaymentResponse> RecordFeePaymentAsync(
        Guid id,
        RecordFeePaymentRequest request,
        CancellationToken cancellationToken = default);

    Task<AdvanceDocumentRequestStatusResponse> AdvanceStatusAsync(Guid id,
        CancellationToken cancellationToken = default);

    Task<IssueDocumentResponse> IssueDocumentAsync(Guid id, CancellationToken cancellationToken = default);

    Task<CancelDocumentRequestResponse> CancelRequestAsync(
        Guid id,
        CancelDocumentRequestRequest request,
        CancellationToken cancellationToken = default);
}