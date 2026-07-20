using SchaerbeekMunicipality.Application.Auth;
using SchaerbeekMunicipality.Domain.DeathDeclaration;
using SchaerbeekMunicipality.Domain.Registration;

namespace SchaerbeekMunicipality.Application.Features.DeathDeclaration.ReleaseCaseLock;

public sealed record ReleaseCaseLockResponse(Guid CaseId, Guid? LockedByOfficerId);

public sealed class ReleaseCaseLockHandler(
    IDeathDeclarationCaseRepository caseRepository,
    DeathDeclarationCaseAuthorization authorization,
    ICurrentOfficer currentOfficer)
{
    public async Task<ReleaseCaseLockResponse> Handle(
        DeathDeclarationCaseId caseId,
        CancellationToken cancellationToken)
    {
        authorization.EnsureCanClaim(currentOfficer);

        var deathDeclarationCase = await caseRepository.GetByIdAsync(caseId, cancellationToken)
                                   ?? throw new KeyNotFoundException(
                                       $"Death declaration case '{caseId}' was not found.");

        var officerId = OfficerId.From(currentOfficer.OfficerId);
        deathDeclarationCase.ReleaseLock(officerId);
        await caseRepository.SaveChangesAsync(cancellationToken);

        return new ReleaseCaseLockResponse(
            deathDeclarationCase.Id.Value,
            deathDeclarationCase.LockedByOfficerId?.Value);
    }
}
