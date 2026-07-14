using SchaerbeekMunicipality.Application.Auth;
using SchaerbeekMunicipality.Domain.BirthDeclaration;

namespace SchaerbeekMunicipality.Application.Features.BirthDeclaration;

public sealed class BirthDeclarationCaseGuard(
    IBirthDeclarationCaseRepository repository,
    BirthDeclarationCaseAuthorization authorization,
    ICurrentOfficer currentOfficer)
{
    public async Task<BirthDeclarationCase> GetForEditAsync(
        BirthDeclarationCaseId caseId,
        string operation,
        CancellationToken cancellationToken)
    {
        var birthDeclarationCase = await GetRequiredAsync(caseId, cancellationToken);
        authorization.EnsureCanEdit(currentOfficer, birthDeclarationCase, operation);
        return birthDeclarationCase;
    }

    public async Task<BirthDeclarationCase> GetForViewAsync(
        BirthDeclarationCaseId caseId,
        CancellationToken cancellationToken)
    {
        var birthDeclarationCase = await GetRequiredAsync(caseId, cancellationToken);
        authorization.EnsureCanView(currentOfficer);
        return birthDeclarationCase;
    }

    private async Task<BirthDeclarationCase> GetRequiredAsync(
        BirthDeclarationCaseId caseId,
        CancellationToken cancellationToken)
    {
        return await repository.GetByIdAsync(caseId, cancellationToken)
               ?? throw new KeyNotFoundException($"Birth declaration case '{caseId}' was not found.");
    }
}