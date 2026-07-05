using SchaerbeekMunicipality.Domain.Registration;
using SchaerbeekMunicipality.Web.Auth;

namespace SchaerbeekMunicipality.Web.Features.Registration.ClaimRegistrationCase;

public sealed record ClaimRegistrationCaseResponse(
    Guid CaseId,
    Guid? AssignedOfficerId,
    Guid? LockedByOfficerId,
    DateTimeOffset? LockedAt,
    bool NewlyClaimed,
    bool CanEdit);

public sealed class ClaimRegistrationCaseHandler(
    IRegistrationCaseRepository caseRepository,
    RegistrationCaseAuthorization authorization,
    CaseAuditRecorder auditRecorder,
    ICurrentOfficer currentOfficer,
    TimeProvider timeProvider)
{
    public async Task<ClaimRegistrationCaseResponse?> TryAutoClaimAsync(
        RegistrationCaseId caseId,
        CancellationToken cancellationToken)
    {
        authorization.EnsureCanClaim(currentOfficer);

        var registrationCase = await caseRepository.GetByIdAsync(caseId, cancellationToken)
            ?? throw new KeyNotFoundException($"Registration case '{caseId}' was not found.");

        authorization.EnsureCanView(currentOfficer);

        var officerId = OfficerId.From(currentOfficer.OfficerId);
        if (!authorization.ShouldAutoClaim(registrationCase, officerId))
        {
            return null;
        }

        return await ClaimCoreAsync(registrationCase, officerId, cancellationToken);
    }

    public async Task<ClaimRegistrationCaseResponse> Handle(
        RegistrationCaseId caseId,
        CancellationToken cancellationToken)
    {
        authorization.EnsureCanClaim(currentOfficer);

        var registrationCase = await caseRepository.GetByIdAsync(caseId, cancellationToken)
            ?? throw new KeyNotFoundException($"Registration case '{caseId}' was not found.");

        authorization.EnsureCanView(currentOfficer);

        var officerId = OfficerId.From(currentOfficer.OfficerId);
        return await ClaimCoreAsync(registrationCase, officerId, cancellationToken);
    }

    private async Task<ClaimRegistrationCaseResponse> ClaimCoreAsync(
        RegistrationCase registrationCase,
        OfficerId officerId,
        CancellationToken cancellationToken)
    {
        var claimResult = registrationCase.Claim(officerId, timeProvider.GetUtcNow());

        if (claimResult is CaseClaimResult.NewlyClaimed or CaseClaimResult.Reclaimed)
        {
            await auditRecorder.RecordAsync(
                registrationCase.Id,
                CaseAuditAction.CaseAssigned,
                claimResult == CaseClaimResult.Reclaimed ? "Case reclaimed" : null,
                cancellationToken);
        }

        await caseRepository.SaveChangesAsync(cancellationToken);

        var canEdit = authorization.CanEditCase(
            currentOfficer.Role,
            registrationCase,
            officerId);

        return new ClaimRegistrationCaseResponse(
            registrationCase.Id.Value,
            registrationCase.AssignedOfficerId?.Value,
            registrationCase.LockedByOfficerId?.Value,
            registrationCase.LockedAt,
            claimResult is CaseClaimResult.NewlyClaimed or CaseClaimResult.Reclaimed,
            canEdit);
    }
}
