using SchaerbeekMunicipality.Domain.IdentityDocuments;
using SchaerbeekMunicipality.Domain.Registration;
using SchaerbeekMunicipality.Application.Auth;

namespace SchaerbeekMunicipality.Application.Features.IdentityDocuments.ClaimDocumentRequestCase;

public sealed record ClaimDocumentRequestCaseResponse(
    Guid CaseId,
    Guid? AssignedOfficerId,
    Guid? LockedByOfficerId,
    DateTimeOffset? LockedAt,
    bool NewlyClaimed,
    bool CanEdit);

public sealed class ClaimDocumentRequestCaseHandler(
    IDocumentRequestCaseRepository caseRepository,
    DocumentRequestCaseAuthorization authorization,
    ICurrentOfficer currentOfficer,
    TimeProvider timeProvider)
{
    public async Task<ClaimDocumentRequestCaseResponse?> TryAutoClaimAsync(
        DocumentRequestCaseId caseId,
        CancellationToken cancellationToken)
    {
        authorization.EnsureCanClaim(currentOfficer);

        var documentRequestCase = await caseRepository.GetByIdAsync(caseId, cancellationToken)
            ?? throw new KeyNotFoundException($"Document request case '{caseId}' was not found.");

        authorization.EnsureCanView(currentOfficer);

        var officerId = OfficerId.From(currentOfficer.OfficerId);
        if (!authorization.ShouldAutoClaim(documentRequestCase, officerId))
        {
            return null;
        }

        return await ClaimCoreAsync(documentRequestCase, officerId, cancellationToken);
    }

    public async Task<ClaimDocumentRequestCaseResponse> Handle(
        DocumentRequestCaseId caseId,
        CancellationToken cancellationToken)
    {
        authorization.EnsureCanClaim(currentOfficer);

        var documentRequestCase = await caseRepository.GetByIdAsync(caseId, cancellationToken)
            ?? throw new KeyNotFoundException($"Document request case '{caseId}' was not found.");

        authorization.EnsureCanView(currentOfficer);

        var officerId = OfficerId.From(currentOfficer.OfficerId);
        return await ClaimCoreAsync(documentRequestCase, officerId, cancellationToken);
    }

    private async Task<ClaimDocumentRequestCaseResponse> ClaimCoreAsync(
        DocumentRequestCase documentRequestCase,
        OfficerId officerId,
        CancellationToken cancellationToken)
    {
        var claimResult = documentRequestCase.Claim(officerId, timeProvider.GetUtcNow());
        await caseRepository.SaveChangesAsync(cancellationToken);

        var canEdit = authorization.CanEditCase(
            currentOfficer.Role,
            documentRequestCase,
            officerId);

        return new ClaimDocumentRequestCaseResponse(
            documentRequestCase.Id.Value,
            documentRequestCase.AssignedOfficerId?.Value,
            documentRequestCase.LockedByOfficerId?.Value,
            documentRequestCase.LockedAt,
            claimResult is CaseClaimResult.NewlyClaimed or CaseClaimResult.Reclaimed,
            canEdit);
    }
}
