using SchaerbeekMunicipality.Application.Auth;
using SchaerbeekMunicipality.Domain.RegisterAmendment;
using SchaerbeekMunicipality.Domain.Registration;

namespace SchaerbeekMunicipality.Application.Features.RegisterAmendment.ClaimRegisterAmendmentCase;

public sealed record ClaimRegisterAmendmentCaseResponse(
    Guid CaseId,
    Guid? AssignedOfficerId,
    Guid? LockedByOfficerId,
    DateTimeOffset? LockedAt,
    bool NewlyClaimed,
    bool CanEdit);

public sealed class ClaimRegisterAmendmentCaseHandler(
    IRegisterAmendmentCaseRepository caseRepository,
    RegisterAmendmentCaseAuthorization authorization,
    ICurrentOfficer currentOfficer,
    TimeProvider timeProvider)
{
    public async Task<ClaimRegisterAmendmentCaseResponse?> TryAutoClaimAsync(
        RegisterAmendmentCaseId caseId,
        CancellationToken cancellationToken)
    {
        authorization.EnsureCanClaim(currentOfficer);

        var amendmentCase = await caseRepository.GetByIdAsync(caseId, cancellationToken)
                            ?? throw new KeyNotFoundException(
                                $"Register amendment case '{caseId}' was not found.");

        authorization.EnsureCanView(currentOfficer);

        var officerId = OfficerId.From(currentOfficer.OfficerId);
        if (!authorization.ShouldAutoClaim(amendmentCase, officerId)) return null;

        return await ClaimCoreAsync(amendmentCase, officerId, cancellationToken);
    }

    public async Task<ClaimRegisterAmendmentCaseResponse> Handle(
        RegisterAmendmentCaseId caseId,
        CancellationToken cancellationToken)
    {
        authorization.EnsureCanClaim(currentOfficer);

        var amendmentCase = await caseRepository.GetByIdAsync(caseId, cancellationToken)
                            ?? throw new KeyNotFoundException(
                                $"Register amendment case '{caseId}' was not found.");

        authorization.EnsureCanView(currentOfficer);

        var officerId = OfficerId.From(currentOfficer.OfficerId);
        return await ClaimCoreAsync(amendmentCase, officerId, cancellationToken);
    }

    private async Task<ClaimRegisterAmendmentCaseResponse> ClaimCoreAsync(
        RegisterAmendmentCase amendmentCase,
        OfficerId officerId,
        CancellationToken cancellationToken)
    {
        var claimResult = amendmentCase.Claim(officerId, timeProvider.GetUtcNow());
        await caseRepository.SaveChangesAsync(cancellationToken);

        var canEdit = authorization.CanEditCase(
            currentOfficer.Role,
            amendmentCase,
            officerId);

        return new ClaimRegisterAmendmentCaseResponse(
            amendmentCase.Id.Value,
            amendmentCase.AssignedOfficerId?.Value,
            amendmentCase.LockedByOfficerId?.Value,
            amendmentCase.LockedAt,
            claimResult is CaseClaimResult.NewlyClaimed or CaseClaimResult.Reclaimed,
            canEdit);
    }
}
