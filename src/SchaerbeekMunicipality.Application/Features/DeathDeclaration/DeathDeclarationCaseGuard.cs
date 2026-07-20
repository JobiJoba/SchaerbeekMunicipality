using SchaerbeekMunicipality.Application.Auth;
using SchaerbeekMunicipality.Domain.DeathDeclaration;

namespace SchaerbeekMunicipality.Application.Features.DeathDeclaration;

public sealed class DeathDeclarationCaseGuard(
    IDeathDeclarationCaseRepository repository,
    DeathDeclarationCaseAuthorization authorization,
    ICurrentOfficer currentOfficer)
{
    public async Task<DeathDeclarationCase> GetForEditAsync(
        DeathDeclarationCaseId caseId,
        string operation,
        CancellationToken cancellationToken)
    {
        var deathDeclarationCase = await GetRequiredAsync(caseId, cancellationToken);
        authorization.EnsureCanEdit(currentOfficer, deathDeclarationCase, operation);
        return deathDeclarationCase;
    }

    public async Task<DeathDeclarationCase> GetForViewAsync(
        DeathDeclarationCaseId caseId,
        CancellationToken cancellationToken)
    {
        var deathDeclarationCase = await GetRequiredAsync(caseId, cancellationToken);
        authorization.EnsureCanView(currentOfficer);
        return deathDeclarationCase;
    }

    private async Task<DeathDeclarationCase> GetRequiredAsync(
        DeathDeclarationCaseId caseId,
        CancellationToken cancellationToken)
    {
        return await repository.GetByIdAsync(caseId, cancellationToken)
               ?? throw new KeyNotFoundException($"Death declaration case '{caseId}' was not found.");
    }
}
