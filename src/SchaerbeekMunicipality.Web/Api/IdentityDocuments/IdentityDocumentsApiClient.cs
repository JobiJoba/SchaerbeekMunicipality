using SchaerbeekMunicipality.Application.Auth;
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

public sealed class IdentityDocumentsApiClient(HttpClient httpClient, ICurrentOfficer currentOfficer)
    : MunicipalApiClientBase(httpClient, currentOfficer), IIdentityDocumentsApi
{
    private const string BasePath = "/api/identity-documents";

    public Task<IReadOnlyList<DocumentRequestCaseListItem>> ListRequestsAsync(
        CancellationToken cancellationToken = default)
    {
        return GetJsonAsync<IReadOnlyList<DocumentRequestCaseListItem>>($"{BasePath}/requests", cancellationToken);
    }

    public Task<OpenDocumentRequestCaseResponse> OpenRequestAsync(
        OpenDocumentRequestCaseRequest request,
        CancellationToken cancellationToken = default)
    {
        return PostJsonAsync<OpenDocumentRequestCaseRequest, OpenDocumentRequestCaseResponse>(
            $"{BasePath}/requests",
            request,
            cancellationToken);
    }

    public Task<ResolveRegisteredPersonResponse> ResolveRegisteredPersonAsync(
        ResolveRegisteredPersonRequest request,
        CancellationToken cancellationToken = default)
    {
        return PostJsonAsync<ResolveRegisteredPersonRequest, ResolveRegisteredPersonResponse>(
            $"{BasePath}/resolve-person",
            request,
            cancellationToken);
    }

    public Task<DocumentRequestCaseDetailDto> GetRequestAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return GetJsonAsync<DocumentRequestCaseDetailDto>($"{BasePath}/requests/{id}", cancellationToken);
    }

    public Task<ClaimDocumentRequestCaseResponse> ClaimRequestAsync(Guid id,
        CancellationToken cancellationToken = default)
    {
        return PostJsonAsync<ClaimDocumentRequestCaseResponse>($"{BasePath}/requests/{id}/claim", cancellationToken);
    }

    public Task<ClaimDocumentRequestCaseResponse?> TryAutoClaimRequestAsync(Guid id,
        CancellationToken cancellationToken = default)
    {
        return PostJsonOptionalAsync<ClaimDocumentRequestCaseResponse>($"{BasePath}/requests/{id}/auto-claim",
            cancellationToken);
    }

    public Task<ReleaseCaseLockResponse> ReleaseCaseLockAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return PostJsonAsync<ReleaseCaseLockResponse>($"{BasePath}/requests/{id}/release-lock", cancellationToken);
    }

    public Task<AttachApplicantPhotoResponse> AttachApplicantPhotoAsync(
        Guid id,
        Stream fileStream,
        string fileName,
        CancellationToken cancellationToken = default)
    {
        return PostMultipartFileAsync<AttachApplicantPhotoResponse>(
            $"{BasePath}/requests/{id}/photo",
            fileStream,
            fileName,
            cancellationToken);
    }

    public Task<Stream> DownloadDocumentAsync(
        Guid id,
        Guid documentId,
        CancellationToken cancellationToken = default)
    {
        return DownloadStreamAsync($"{BasePath}/requests/{id}/documents/{documentId}", cancellationToken);
    }

    public Task<RemoveDocumentResponse> RemoveDocumentAsync(
        Guid id,
        Guid documentId,
        CancellationToken cancellationToken = default)
    {
        return DeleteJsonAsync<RemoveDocumentResponse>($"{BasePath}/requests/{id}/documents/{documentId}",
            cancellationToken);
    }

    public Task<RecordFeePaymentResponse> RecordFeePaymentAsync(
        Guid id,
        RecordFeePaymentRequest request,
        CancellationToken cancellationToken = default)
    {
        return PostJsonAsync<RecordFeePaymentRequest, RecordFeePaymentResponse>(
            $"{BasePath}/requests/{id}/fee-payment",
            request,
            cancellationToken);
    }

    public Task<AdvanceDocumentRequestStatusResponse> AdvanceStatusAsync(Guid id,
        CancellationToken cancellationToken = default)
    {
        return PostJsonAsync<AdvanceDocumentRequestStatusResponse>($"{BasePath}/requests/{id}/advance",
            cancellationToken);
    }

    public Task<IssueDocumentResponse> IssueDocumentAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return PostJsonAsync<IssueDocumentResponse>($"{BasePath}/requests/{id}/issue", cancellationToken);
    }

    public Task<CancelDocumentRequestResponse> CancelRequestAsync(
        Guid id,
        CancelDocumentRequestRequest request,
        CancellationToken cancellationToken = default)
    {
        return PostJsonAsync<CancelDocumentRequestRequest, CancelDocumentRequestResponse>(
            $"{BasePath}/requests/{id}/cancel",
            request,
            cancellationToken);
    }
}