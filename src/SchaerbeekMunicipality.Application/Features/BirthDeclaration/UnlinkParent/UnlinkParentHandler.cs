using SchaerbeekMunicipality.Domain.BirthDeclaration;
using SchaerbeekMunicipality.Domain.Identity;

namespace SchaerbeekMunicipality.Application.Features.BirthDeclaration.UnlinkParent;

public sealed record UnlinkParentResponse(Guid CaseId, bool AtLeastOneParentLinked);

public sealed class UnlinkParentHandler(
    BirthDeclarationCaseGuard caseGuard,
    IBirthDeclarationCaseRepository caseRepository)
{
    public async Task<UnlinkParentResponse> Handle(
        BirthDeclarationCaseId caseId,
        PersonId personId,
        CancellationToken cancellationToken)
    {
        var birthDeclarationCase = await caseGuard.GetForEditAsync(
            caseId,
            nameof(UnlinkParent),
            cancellationToken);

        birthDeclarationCase.UnlinkParent(personId);
        await caseRepository.SaveChangesAsync(cancellationToken);

        return new UnlinkParentResponse(
            birthDeclarationCase.Id.Value,
            birthDeclarationCase.Checklist.AtLeastOneParentLinked);
    }
}