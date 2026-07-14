using SchaerbeekMunicipality.Application.Auth;
using SchaerbeekMunicipality.Domain.RegisterAmendment;

namespace SchaerbeekMunicipality.Application.Features.RegisterAmendment;

public sealed class RegisterAmendmentCaseGuard(
    IRegisterAmendmentCaseRepository repository,
    RegisterAmendmentCaseAuthorization authorization,
    ICurrentOfficer currentOfficer)
{
    public async Task<RegisterAmendmentCase> GetForEditAsync(
        RegisterAmendmentCaseId caseId,
        string operation,
        CancellationToken cancellationToken)
    {
        var amendmentCase = await GetRequiredAsync(caseId, cancellationToken);
        authorization.EnsureCanEdit(currentOfficer, amendmentCase, operation);
        return amendmentCase;
    }

    public async Task<RegisterAmendmentCase> GetForViewAsync(
        RegisterAmendmentCaseId caseId,
        CancellationToken cancellationToken)
    {
        var amendmentCase = await GetRequiredAsync(caseId, cancellationToken);
        authorization.EnsureCanView(currentOfficer);
        return amendmentCase;
    }

    private async Task<RegisterAmendmentCase> GetRequiredAsync(
        RegisterAmendmentCaseId caseId,
        CancellationToken cancellationToken)
    {
        return await repository.GetByIdAsync(caseId, cancellationToken)
               ?? throw new KeyNotFoundException($"Register amendment case '{caseId}' was not found.");
    }
}
