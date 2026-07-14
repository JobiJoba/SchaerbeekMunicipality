using SchaerbeekMunicipality.Application.Auth;
using SchaerbeekMunicipality.Application.Features.RegisterAmendment.ApplyRegisterAmendment;
using SchaerbeekMunicipality.Application.Features.RegisterAmendment.ApproveRegisterAmendment;
using SchaerbeekMunicipality.Application.Features.RegisterAmendment.AttachDocument;
using SchaerbeekMunicipality.Application.Features.RegisterAmendment.ClaimRegisterAmendmentCase;
using SchaerbeekMunicipality.Application.Features.RegisterAmendment.GetRegisterAmendmentCase;
using SchaerbeekMunicipality.Application.Features.RegisterAmendment.ListRegisterAmendmentCases;
using SchaerbeekMunicipality.Application.Features.RegisterAmendment.OpenRegisterAmendmentCase;
using SchaerbeekMunicipality.Application.Features.RegisterAmendment.RecordProposedAmendment;
using SchaerbeekMunicipality.Application.Features.RegisterAmendment.RejectRegisterAmendment;
using SchaerbeekMunicipality.Application.Features.RegisterAmendment.ReleaseCaseLock;
using SchaerbeekMunicipality.Application.Features.RegisterAmendment.SubmitRegisterAmendmentForReview;
using SchaerbeekMunicipality.Domain.Documents;

namespace SchaerbeekMunicipality.Web.Api.RegisterAmendment;

public sealed class RegisterAmendmentApiClient(HttpClient httpClient, ICurrentOfficer currentOfficer)
    : MunicipalApiClientBase(httpClient, currentOfficer), IRegisterAmendmentApi
{
    private const string BasePath = "/api/register-amendments";

    public Task<IReadOnlyList<RegisterAmendmentCaseListItem>> ListCasesAsync(
        CancellationToken cancellationToken = default)
    {
        return GetJsonAsync<IReadOnlyList<RegisterAmendmentCaseListItem>>($"{BasePath}/cases", cancellationToken);
    }

    public Task<OpenRegisterAmendmentCaseResponse> OpenCaseAsync(
        OpenRegisterAmendmentCaseRequest request,
        CancellationToken cancellationToken = default)
    {
        return PostJsonAsync<OpenRegisterAmendmentCaseRequest, OpenRegisterAmendmentCaseResponse>(
            $"{BasePath}/cases",
            request,
            cancellationToken);
    }

    public Task<RegisterAmendmentCaseDetailDto> GetCaseAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return GetJsonAsync<RegisterAmendmentCaseDetailDto>($"{BasePath}/cases/{id}", cancellationToken);
    }

    public Task<ClaimRegisterAmendmentCaseResponse> ClaimCaseAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return PostJsonAsync<ClaimRegisterAmendmentCaseResponse>($"{BasePath}/cases/{id}/claim", cancellationToken);
    }

    public Task<ClaimRegisterAmendmentCaseResponse?> TryAutoClaimCaseAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return PostJsonOptionalAsync<ClaimRegisterAmendmentCaseResponse>(
            $"{BasePath}/cases/{id}/auto-claim",
            cancellationToken);
    }

    public Task<ReleaseCaseLockResponse> ReleaseCaseLockAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return PostJsonAsync<ReleaseCaseLockResponse>($"{BasePath}/cases/{id}/release-lock", cancellationToken);
    }

    public Task<RecordProposedAmendmentResponse> RecordProposedChangesAsync(
        Guid id,
        RecordProposedAmendmentRequest request,
        CancellationToken cancellationToken = default)
    {
        return PutJsonAsync<RecordProposedAmendmentRequest, RecordProposedAmendmentResponse>(
            $"{BasePath}/cases/{id}/proposed-changes",
            request,
            cancellationToken);
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

    public Task RemoveDocumentAsync(Guid id, Guid documentId, CancellationToken cancellationToken = default)
    {
        return DeleteJsonAsync<object>($"{BasePath}/cases/{id}/documents/{documentId}", cancellationToken);
    }

    public Task<SubmitRegisterAmendmentForReviewResponse> SubmitForReviewAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return PostJsonAsync<SubmitRegisterAmendmentForReviewResponse>(
            $"{BasePath}/cases/{id}/submit",
            cancellationToken);
    }

    public Task<ApproveRegisterAmendmentResponse> ApproveAsync(
        Guid id,
        ApproveRegisterAmendmentRequest request,
        CancellationToken cancellationToken = default)
    {
        return PostJsonAsync<ApproveRegisterAmendmentRequest, ApproveRegisterAmendmentResponse>(
            $"{BasePath}/cases/{id}/approve",
            request,
            cancellationToken);
    }

    public Task<RejectRegisterAmendmentResponse> RejectAsync(
        Guid id,
        RejectRegisterAmendmentRequest request,
        CancellationToken cancellationToken = default)
    {
        return PostJsonAsync<RejectRegisterAmendmentRequest, RejectRegisterAmendmentResponse>(
            $"{BasePath}/cases/{id}/reject",
            request,
            cancellationToken);
    }

    public Task<ApplyRegisterAmendmentResponse> ApplyAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return PostJsonAsync<ApplyRegisterAmendmentResponse>($"{BasePath}/cases/{id}/apply", cancellationToken);
    }
}
