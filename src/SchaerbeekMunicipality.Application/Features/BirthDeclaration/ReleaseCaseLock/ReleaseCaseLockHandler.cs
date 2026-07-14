using SchaerbeekMunicipality.Application.Auth;
using SchaerbeekMunicipality.Domain.BirthDeclaration;
using SchaerbeekMunicipality.Domain.Registration;

namespace SchaerbeekMunicipality.Application.Features.BirthDeclaration.ReleaseCaseLock;

public sealed record ReleaseCaseLockResponse(Guid CaseId, Guid? LockedByOfficerId);

public sealed class ReleaseCaseLockHandler(
    IBirthDeclarationCaseRepository caseRepository,
    BirthDeclarationCaseAuthorization authorization,
    ICurrentOfficer currentOfficer)
{
    public async Task<ReleaseCaseLockResponse> Handle(
        BirthDeclarationCaseId caseId,
        CancellationToken cancellationToken)
    {
        authorization.EnsureCanClaim(currentOfficer);

        var birthDeclarationCase = await caseRepository.GetByIdAsync(caseId, cancellationToken)
                                   ?? throw new KeyNotFoundException(
                                       $"Birth declaration case '{caseId}' was not found.");

        var officerId = OfficerId.From(currentOfficer.OfficerId);
        birthDeclarationCase.ReleaseLock(officerId);
        await caseRepository.SaveChangesAsync(cancellationToken);

        return new ReleaseCaseLockResponse(
            birthDeclarationCase.Id.Value,
            birthDeclarationCase.LockedByOfficerId?.Value);
    }
}