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

public interface IRegisterAmendmentApi
{
    Task<IReadOnlyList<RegisterAmendmentCaseListItem>> ListCasesAsync(CancellationToken cancellationToken = default);

    Task<OpenRegisterAmendmentCaseResponse> OpenCaseAsync(
        OpenRegisterAmendmentCaseRequest request,
        CancellationToken cancellationToken = default);

    Task<RegisterAmendmentCaseDetailDto> GetCaseAsync(Guid id, CancellationToken cancellationToken = default);

    Task<ClaimRegisterAmendmentCaseResponse> ClaimCaseAsync(Guid id, CancellationToken cancellationToken = default);

    Task<ClaimRegisterAmendmentCaseResponse?> TryAutoClaimCaseAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<ReleaseCaseLockResponse> ReleaseCaseLockAsync(Guid id, CancellationToken cancellationToken = default);

    Task<RecordProposedAmendmentResponse> RecordProposedChangesAsync(
        Guid id,
        RecordProposedAmendmentRequest request,
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

    Task RemoveDocumentAsync(Guid id, Guid documentId, CancellationToken cancellationToken = default);

    Task<SubmitRegisterAmendmentForReviewResponse> SubmitForReviewAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<ApproveRegisterAmendmentResponse> ApproveAsync(
        Guid id,
        ApproveRegisterAmendmentRequest request,
        CancellationToken cancellationToken = default);

    Task<RejectRegisterAmendmentResponse> RejectAsync(
        Guid id,
        RejectRegisterAmendmentRequest request,
        CancellationToken cancellationToken = default);

    Task<ApplyRegisterAmendmentResponse> ApplyAsync(Guid id, CancellationToken cancellationToken = default);
}
