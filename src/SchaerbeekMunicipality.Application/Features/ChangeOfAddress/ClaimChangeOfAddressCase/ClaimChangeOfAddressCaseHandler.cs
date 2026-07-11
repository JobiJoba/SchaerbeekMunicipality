using SchaerbeekMunicipality.Domain.ChangeOfAddress;
using SchaerbeekMunicipality.Domain.Registration;
using SchaerbeekMunicipality.Application.Auth;

namespace SchaerbeekMunicipality.Application.Features.ChangeOfAddress.ClaimChangeOfAddressCase;

public sealed record ClaimChangeOfAddressCaseResponse(
    Guid CaseId,
    Guid? AssignedOfficerId,
    Guid? LockedByOfficerId,
    DateTimeOffset? LockedAt,
    bool NewlyClaimed,
    bool CanEdit);

public sealed class ClaimChangeOfAddressCaseHandler(
    IChangeOfAddressCaseRepository caseRepository,
    ChangeOfAddressCaseAuthorization authorization,
    ICurrentOfficer currentOfficer,
    TimeProvider timeProvider)
{
    public async Task<ClaimChangeOfAddressCaseResponse?> TryAutoClaimAsync(
        ChangeOfAddressCaseId caseId,
        CancellationToken cancellationToken)
    {
        authorization.EnsureCanClaim(currentOfficer);

        var changeOfAddressCase = await caseRepository.GetByIdAsync(caseId, cancellationToken)
            ?? throw new KeyNotFoundException($"Change of address case '{caseId}' was not found.");

        authorization.EnsureCanView(currentOfficer);

        var officerId = OfficerId.From(currentOfficer.OfficerId);
        if (!authorization.ShouldAutoClaim(changeOfAddressCase, officerId))
        {
            return null;
        }

        return await ClaimCoreAsync(changeOfAddressCase, officerId, cancellationToken);
    }

    public async Task<ClaimChangeOfAddressCaseResponse> Handle(
        ChangeOfAddressCaseId caseId,
        CancellationToken cancellationToken)
    {
        authorization.EnsureCanClaim(currentOfficer);

        var changeOfAddressCase = await caseRepository.GetByIdAsync(caseId, cancellationToken)
            ?? throw new KeyNotFoundException($"Change of address case '{caseId}' was not found.");

        authorization.EnsureCanView(currentOfficer);

        var officerId = OfficerId.From(currentOfficer.OfficerId);
        return await ClaimCoreAsync(changeOfAddressCase, officerId, cancellationToken);
    }

    private async Task<ClaimChangeOfAddressCaseResponse> ClaimCoreAsync(
        ChangeOfAddressCase changeOfAddressCase,
        OfficerId officerId,
        CancellationToken cancellationToken)
    {
        var claimResult = changeOfAddressCase.Claim(officerId, timeProvider.GetUtcNow());
        await caseRepository.SaveChangesAsync(cancellationToken);

        var canEdit = authorization.CanEditCase(
            currentOfficer.Role,
            changeOfAddressCase,
            officerId);

        return new ClaimChangeOfAddressCaseResponse(
            changeOfAddressCase.Id.Value,
            changeOfAddressCase.AssignedOfficerId?.Value,
            changeOfAddressCase.LockedByOfficerId?.Value,
            changeOfAddressCase.LockedAt,
            claimResult is CaseClaimResult.NewlyClaimed or CaseClaimResult.Reclaimed,
            canEdit);
    }
}
