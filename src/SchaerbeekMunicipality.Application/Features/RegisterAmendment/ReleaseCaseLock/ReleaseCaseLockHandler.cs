using SchaerbeekMunicipality.Application.Auth;
using SchaerbeekMunicipality.Domain.RegisterAmendment;
using SchaerbeekMunicipality.Domain.Registration;

namespace SchaerbeekMunicipality.Application.Features.RegisterAmendment.ReleaseCaseLock;

public sealed record ReleaseCaseLockResponse(Guid CaseId, Guid? LockedByOfficerId);

public sealed class ReleaseCaseLockHandler(
    IRegisterAmendmentCaseRepository caseRepository,
    RegisterAmendmentCaseAuthorization authorization,
    ICurrentOfficer currentOfficer)
{
    public async Task<ReleaseCaseLockResponse> Handle(
        RegisterAmendmentCaseId caseId,
        CancellationToken cancellationToken)
    {
        authorization.EnsureCanClaim(currentOfficer);

        var amendmentCase = await caseRepository.GetByIdAsync(caseId, cancellationToken)
                            ?? throw new KeyNotFoundException(
                                $"Register amendment case '{caseId}' was not found.");

        amendmentCase.ReleaseLock(OfficerId.From(currentOfficer.OfficerId));
        await caseRepository.SaveChangesAsync(cancellationToken);

        return new ReleaseCaseLockResponse(
            amendmentCase.Id.Value,
            amendmentCase.LockedByOfficerId?.Value);
    }
}
