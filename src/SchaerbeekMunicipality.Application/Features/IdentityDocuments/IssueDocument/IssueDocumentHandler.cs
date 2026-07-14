using SchaerbeekMunicipality.Domain.IdentityDocuments;

namespace SchaerbeekMunicipality.Application.Features.IdentityDocuments.IssueDocument;

public sealed record IssueDocumentResponse(
    Guid CaseId,
    DocumentRequestCaseStatus Status,
    string IssuedDocumentNumber,
    DateTimeOffset IssuedAt);

public sealed class IssueDocumentHandler(
    DocumentRequestCaseGuard caseGuard,
    IDocumentRequestCaseRepository caseRepository,
    TimeProvider timeProvider)
{
    public async Task<IssueDocumentResponse> Handle(
        DocumentRequestCaseId caseId,
        CancellationToken cancellationToken)
    {
        var documentRequestCase = await caseGuard.GetForEditAsync(
            caseId,
            nameof(IssueDocument),
            cancellationToken);

        var documentNumber = await DocumentNumberGenerator.NextAsync(caseRepository, timeProvider, cancellationToken);
        var issuedAt = timeProvider.GetUtcNow();

        documentRequestCase.Issue(documentNumber, issuedAt);
        await caseRepository.SaveChangesAsync(cancellationToken);

        return new IssueDocumentResponse(
            documentRequestCase.Id.Value,
            documentRequestCase.Status,
            documentRequestCase.IssuedDocumentNumber!,
            documentRequestCase.IssuedAt!.Value);
    }
}