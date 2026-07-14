using SchaerbeekMunicipality.Application.Auth;
using SchaerbeekMunicipality.Domain.Registration;

namespace SchaerbeekMunicipality.Application.Features.Registration;

public sealed class RegistrationCaseGuard(
    IRegistrationCaseRepository repository,
    RegistrationCaseAuthorization authorization,
    ICurrentOfficer currentOfficer)
{
    public async Task<RegistrationCase> GetForEditAsync(
        RegistrationCaseId caseId,
        string operation,
        CancellationToken cancellationToken)
    {
        var registrationCase = await GetRequiredAsync(caseId, cancellationToken);
        authorization.EnsureCanEdit(currentOfficer, registrationCase, operation);
        return registrationCase;
    }

    public async Task<RegistrationCase> GetForViewAsync(
        RegistrationCaseId caseId,
        CancellationToken cancellationToken)
    {
        var registrationCase = await GetRequiredAsync(caseId, cancellationToken);
        authorization.EnsureCanView(currentOfficer);
        return registrationCase;
    }

    private async Task<RegistrationCase> GetRequiredAsync(
        RegistrationCaseId caseId,
        CancellationToken cancellationToken)
    {
        return await repository.GetByIdAsync(caseId, cancellationToken)
               ?? throw new KeyNotFoundException($"Registration case '{caseId}' was not found.");
    }
}