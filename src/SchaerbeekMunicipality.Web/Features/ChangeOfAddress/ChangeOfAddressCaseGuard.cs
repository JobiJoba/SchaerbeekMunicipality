using SchaerbeekMunicipality.Domain.ChangeOfAddress;
using SchaerbeekMunicipality.Web.Auth;

namespace SchaerbeekMunicipality.Web.Features.ChangeOfAddress;

public sealed class ChangeOfAddressCaseGuard(
    IChangeOfAddressCaseRepository repository,
    ChangeOfAddressCaseAuthorization authorization,
    ICurrentOfficer currentOfficer)
{
    public async Task<ChangeOfAddressCase> GetForEditAsync(
        ChangeOfAddressCaseId caseId,
        string operation,
        CancellationToken cancellationToken)
    {
        var changeOfAddressCase = await GetRequiredAsync(caseId, cancellationToken);
        authorization.EnsureCanEdit(currentOfficer, changeOfAddressCase, operation);
        return changeOfAddressCase;
    }

    public async Task<ChangeOfAddressCase> GetForViewAsync(
        ChangeOfAddressCaseId caseId,
        CancellationToken cancellationToken)
    {
        var changeOfAddressCase = await GetRequiredAsync(caseId, cancellationToken);
        authorization.EnsureCanView(currentOfficer);
        return changeOfAddressCase;
    }

    private async Task<ChangeOfAddressCase> GetRequiredAsync(
        ChangeOfAddressCaseId caseId,
        CancellationToken cancellationToken) =>
        await repository.GetByIdAsync(caseId, cancellationToken)
            ?? throw new KeyNotFoundException($"Change of address case '{caseId}' was not found.");
}
