using SchaerbeekMunicipality.Application.Auth;
using SchaerbeekMunicipality.Domain.BirthDeclaration;
using SchaerbeekMunicipality.Domain.Registration;

namespace SchaerbeekMunicipality.Application.Features.BirthDeclaration.ClaimBirthDeclarationCase;

public sealed record ClaimBirthDeclarationCaseResponse(
    Guid CaseId,
    Guid? AssignedOfficerId,
    Guid? LockedByOfficerId,
    DateTimeOffset? LockedAt,
    bool NewlyClaimed,
    bool CanEdit);

public sealed class ClaimBirthDeclarationCaseHandler(
    IBirthDeclarationCaseRepository caseRepository,
    BirthDeclarationCaseAuthorization authorization,
    ICurrentOfficer currentOfficer,
    TimeProvider timeProvider)
{
    public async Task<ClaimBirthDeclarationCaseResponse?> TryAutoClaimAsync(
        BirthDeclarationCaseId caseId,
        CancellationToken cancellationToken)
    {
        authorization.EnsureCanClaim(currentOfficer);

        var birthDeclarationCase = await caseRepository.GetByIdAsync(caseId, cancellationToken)
                                   ?? throw new KeyNotFoundException(
                                       $"Birth declaration case '{caseId}' was not found.");

        authorization.EnsureCanView(currentOfficer);

        var officerId = OfficerId.From(currentOfficer.OfficerId);
        if (!authorization.ShouldAutoClaim(birthDeclarationCase, officerId)) return null;

        return await ClaimCoreAsync(birthDeclarationCase, officerId, cancellationToken);
    }

    public async Task<ClaimBirthDeclarationCaseResponse> Handle(
        BirthDeclarationCaseId caseId,
        CancellationToken cancellationToken)
    {
        authorization.EnsureCanClaim(currentOfficer);

        var birthDeclarationCase = await caseRepository.GetByIdAsync(caseId, cancellationToken)
                                   ?? throw new KeyNotFoundException(
                                       $"Birth declaration case '{caseId}' was not found.");

        authorization.EnsureCanView(currentOfficer);

        var officerId = OfficerId.From(currentOfficer.OfficerId);
        return await ClaimCoreAsync(birthDeclarationCase, officerId, cancellationToken);
    }

    private async Task<ClaimBirthDeclarationCaseResponse> ClaimCoreAsync(
        BirthDeclarationCase birthDeclarationCase,
        OfficerId officerId,
        CancellationToken cancellationToken)
    {
        var claimResult = birthDeclarationCase.Claim(officerId, timeProvider.GetUtcNow());
        await caseRepository.SaveChangesAsync(cancellationToken);

        var canEdit = authorization.CanEditCase(
            currentOfficer.Role,
            birthDeclarationCase,
            officerId);

        return new ClaimBirthDeclarationCaseResponse(
            birthDeclarationCase.Id.Value,
            birthDeclarationCase.AssignedOfficerId?.Value,
            birthDeclarationCase.LockedByOfficerId?.Value,
            birthDeclarationCase.LockedAt,
            claimResult is CaseClaimResult.NewlyClaimed or CaseClaimResult.Reclaimed,
            canEdit);
    }
}