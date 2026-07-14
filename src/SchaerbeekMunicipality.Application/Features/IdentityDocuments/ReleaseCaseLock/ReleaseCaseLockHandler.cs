using SchaerbeekMunicipality.Application.Auth;
using SchaerbeekMunicipality.Domain.IdentityDocuments;
using SchaerbeekMunicipality.Domain.Registration;

namespace SchaerbeekMunicipality.Application.Features.IdentityDocuments.ReleaseCaseLock;

public sealed record ReleaseCaseLockResponse(Guid CaseId, Guid? LockedByOfficerId);

public sealed class ReleaseCaseLockHandler(
    IDocumentRequestCaseRepository caseRepository,
    DocumentRequestCaseAuthorization authorization,
    ICurrentOfficer currentOfficer)
{
    public async Task<ReleaseCaseLockResponse> Handle(
        DocumentRequestCaseId caseId,
        CancellationToken cancellationToken)
    {
        authorization.EnsureCanClaim(currentOfficer);

        var documentRequestCase = await caseRepository.GetByIdAsync(caseId, cancellationToken)
                                  ?? throw new KeyNotFoundException($"Document request case '{caseId}' was not found.");

        var officerId = OfficerId.From(currentOfficer.OfficerId);
        documentRequestCase.ReleaseLock(officerId);
        await caseRepository.SaveChangesAsync(cancellationToken);

        return new ReleaseCaseLockResponse(
            documentRequestCase.Id.Value,
            documentRequestCase.LockedByOfficerId?.Value);
    }
}