using SchaerbeekMunicipality.Application.Auth;
using SchaerbeekMunicipality.Domain.DeathDeclaration;
using SchaerbeekMunicipality.Domain.Registration;

namespace SchaerbeekMunicipality.Application.Features.DeathDeclaration.ClaimDeathDeclarationCase;

public sealed record ClaimDeathDeclarationCaseResponse(
    Guid CaseId,
    Guid? AssignedOfficerId,
    Guid? LockedByOfficerId,
    DateTimeOffset? LockedAt,
    bool NewlyClaimed,
    bool CanEdit);

public sealed class ClaimDeathDeclarationCaseHandler(
    IDeathDeclarationCaseRepository caseRepository,
    DeathDeclarationCaseAuthorization authorization,
    ICurrentOfficer currentOfficer,
    TimeProvider timeProvider)
{
    public async Task<ClaimDeathDeclarationCaseResponse?> TryAutoClaimAsync(
        DeathDeclarationCaseId caseId,
        CancellationToken cancellationToken)
    {
        authorization.EnsureCanClaim(currentOfficer);

        var deathDeclarationCase = await caseRepository.GetByIdAsync(caseId, cancellationToken)
                                   ?? throw new KeyNotFoundException(
                                       $"Death declaration case '{caseId}' was not found.");

        authorization.EnsureCanView(currentOfficer);

        var officerId = OfficerId.From(currentOfficer.OfficerId);
        if (!authorization.ShouldAutoClaim(deathDeclarationCase, officerId)) return null;

        return await ClaimCoreAsync(deathDeclarationCase, officerId, cancellationToken);
    }

    public async Task<ClaimDeathDeclarationCaseResponse> Handle(
        DeathDeclarationCaseId caseId,
        CancellationToken cancellationToken)
    {
        authorization.EnsureCanClaim(currentOfficer);

        var deathDeclarationCase = await caseRepository.GetByIdAsync(caseId, cancellationToken)
                                   ?? throw new KeyNotFoundException(
                                       $"Death declaration case '{caseId}' was not found.");

        authorization.EnsureCanView(currentOfficer);

        var officerId = OfficerId.From(currentOfficer.OfficerId);
        return await ClaimCoreAsync(deathDeclarationCase, officerId, cancellationToken);
    }

    private async Task<ClaimDeathDeclarationCaseResponse> ClaimCoreAsync(
        DeathDeclarationCase deathDeclarationCase,
        OfficerId officerId,
        CancellationToken cancellationToken)
    {
        var claimResult = deathDeclarationCase.Claim(officerId, timeProvider.GetUtcNow());
        await caseRepository.SaveChangesAsync(cancellationToken);

        var canEdit = authorization.CanEditCase(
            currentOfficer.Role,
            deathDeclarationCase,
            officerId);

        return new ClaimDeathDeclarationCaseResponse(
            deathDeclarationCase.Id.Value,
            deathDeclarationCase.AssignedOfficerId?.Value,
            deathDeclarationCase.LockedByOfficerId?.Value,
            deathDeclarationCase.LockedAt,
            claimResult is CaseClaimResult.NewlyClaimed or CaseClaimResult.Reclaimed,
            canEdit);
    }
}
