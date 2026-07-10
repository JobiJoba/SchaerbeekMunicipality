using SchaerbeekMunicipality.Domain.ChangeOfAddress;
using SchaerbeekMunicipality.Domain.Registration;
using SchaerbeekMunicipality.Web.Auth;

namespace SchaerbeekMunicipality.Web.Features.ChangeOfAddress.ReleaseCaseLock;

public sealed record ReleaseCaseLockResponse(Guid CaseId, Guid? LockedByOfficerId);

public sealed class ReleaseCaseLockHandler(
    IChangeOfAddressCaseRepository caseRepository,
    ChangeOfAddressCaseAuthorization authorization,
    ICurrentOfficer currentOfficer)
{
    public async Task<ReleaseCaseLockResponse> Handle(
        ChangeOfAddressCaseId caseId,
        CancellationToken cancellationToken)
    {
        authorization.EnsureCanClaim(currentOfficer);

        var changeOfAddressCase = await caseRepository.GetByIdAsync(caseId, cancellationToken)
            ?? throw new KeyNotFoundException($"Change of address case '{caseId}' was not found.");

        var officerId = OfficerId.From(currentOfficer.OfficerId);
        changeOfAddressCase.ReleaseLock(officerId);
        await caseRepository.SaveChangesAsync(cancellationToken);

        return new ReleaseCaseLockResponse(
            changeOfAddressCase.Id.Value,
            changeOfAddressCase.LockedByOfficerId?.Value);
    }
}
