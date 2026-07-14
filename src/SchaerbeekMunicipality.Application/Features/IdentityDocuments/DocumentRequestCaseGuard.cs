using SchaerbeekMunicipality.Application.Auth;
using SchaerbeekMunicipality.Domain.IdentityDocuments;

namespace SchaerbeekMunicipality.Application.Features.IdentityDocuments;

public sealed class DocumentRequestCaseGuard(
    IDocumentRequestCaseRepository repository,
    DocumentRequestCaseAuthorization authorization,
    ICurrentOfficer currentOfficer)
{
    public async Task<DocumentRequestCase> GetForEditAsync(
        DocumentRequestCaseId caseId,
        string operation,
        CancellationToken cancellationToken)
    {
        var documentRequestCase = await GetRequiredAsync(caseId, cancellationToken);
        authorization.EnsureCanEdit(currentOfficer, documentRequestCase, operation);
        return documentRequestCase;
    }

    public async Task<DocumentRequestCase> GetForViewAsync(
        DocumentRequestCaseId caseId,
        CancellationToken cancellationToken)
    {
        var documentRequestCase = await GetRequiredAsync(caseId, cancellationToken);
        authorization.EnsureCanView(currentOfficer);
        return documentRequestCase;
    }

    private async Task<DocumentRequestCase> GetRequiredAsync(
        DocumentRequestCaseId caseId,
        CancellationToken cancellationToken)
    {
        return await repository.GetByIdAsync(caseId, cancellationToken)
               ?? throw new KeyNotFoundException($"Document request case '{caseId}' was not found.");
    }
}