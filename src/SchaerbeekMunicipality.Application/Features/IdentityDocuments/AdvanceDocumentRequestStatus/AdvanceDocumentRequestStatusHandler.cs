using SchaerbeekMunicipality.Domain.IdentityDocuments;

namespace SchaerbeekMunicipality.Application.Features.IdentityDocuments.AdvanceDocumentRequestStatus;

public sealed record AdvanceDocumentRequestStatusResponse(
    Guid CaseId,
    DocumentRequestCaseStatus Status);

public sealed class AdvanceDocumentRequestStatusHandler(
    DocumentRequestCaseGuard caseGuard,
    IDocumentRequestCaseRepository caseRepository)
{
    public async Task<AdvanceDocumentRequestStatusResponse> Handle(
        DocumentRequestCaseId caseId,
        CancellationToken cancellationToken)
    {
        var documentRequestCase = await caseGuard.GetForEditAsync(
            caseId,
            nameof(AdvanceDocumentRequestStatus),
            cancellationToken);

        documentRequestCase.AdvanceStatus();
        await caseRepository.SaveChangesAsync(cancellationToken);

        return new AdvanceDocumentRequestStatusResponse(
            documentRequestCase.Id.Value,
            documentRequestCase.Status);
    }
}