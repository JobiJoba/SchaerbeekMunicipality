using SchaerbeekMunicipality.Application.Auth;
using SchaerbeekMunicipality.Domain.BirthDeclaration;

namespace SchaerbeekMunicipality.Application.Features.BirthDeclaration.ResumeBirthDeclaration;

public sealed record ResumeBirthDeclarationResponse(Guid CaseId, string Status);

public sealed class ResumeBirthDeclarationHandler(
    BirthDeclarationCaseGuard caseGuard,
    IBirthDeclarationCaseRepository caseRepository,
    ICurrentOfficer currentOfficer)
{
    public async Task<ResumeBirthDeclarationResponse> Handle(
        BirthDeclarationCaseId caseId,
        CancellationToken cancellationToken)
    {
        if (!currentOfficer.CanApproveRegistration)
            throw new UnauthorizedAccessException("Only population officers can resume birth declaration cases.");

        var birthDeclarationCase = await caseGuard.GetForEditAsync(
            caseId,
            nameof(ResumeBirthDeclaration),
            cancellationToken);

        birthDeclarationCase.ResumeFromSuspension();
        await caseRepository.SaveChangesAsync(cancellationToken);

        return new ResumeBirthDeclarationResponse(
            birthDeclarationCase.Id.Value,
            birthDeclarationCase.Status.ToString());
    }
}