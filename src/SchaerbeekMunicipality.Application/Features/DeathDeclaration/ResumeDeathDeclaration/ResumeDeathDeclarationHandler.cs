using SchaerbeekMunicipality.Application.Auth;
using SchaerbeekMunicipality.Domain.DeathDeclaration;

namespace SchaerbeekMunicipality.Application.Features.DeathDeclaration.ResumeDeathDeclaration;

public sealed record ResumeDeathDeclarationResponse(Guid CaseId, string Status);

public sealed class ResumeDeathDeclarationHandler(
    DeathDeclarationCaseGuard caseGuard,
    IDeathDeclarationCaseRepository caseRepository,
    ICurrentOfficer currentOfficer)
{
    public async Task<ResumeDeathDeclarationResponse> Handle(
        DeathDeclarationCaseId caseId,
        CancellationToken cancellationToken)
    {
        if (!currentOfficer.CanApproveRegistration)
            throw new UnauthorizedAccessException("Only population officers can resume death declaration cases.");

        var deathDeclarationCase = await caseGuard.GetForEditAsync(
            caseId,
            nameof(ResumeDeathDeclaration),
            cancellationToken);

        deathDeclarationCase.ResumeFromSuspension();
        await caseRepository.SaveChangesAsync(cancellationToken);

        return new ResumeDeathDeclarationResponse(
            deathDeclarationCase.Id.Value,
            deathDeclarationCase.Status.ToString());
    }
}
