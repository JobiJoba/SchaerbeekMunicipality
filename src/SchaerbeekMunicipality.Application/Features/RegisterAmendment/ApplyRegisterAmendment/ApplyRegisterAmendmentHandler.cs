using SchaerbeekMunicipality.Application.Auth;
using SchaerbeekMunicipality.Domain.Identity;
using SchaerbeekMunicipality.Domain.RegisterAmendment;
using SchaerbeekMunicipality.Domain.Registration;

namespace SchaerbeekMunicipality.Application.Features.RegisterAmendment.ApplyRegisterAmendment;

public sealed record ApplyRegisterAmendmentResponse(
    Guid CaseId,
    Guid PersonId,
    string Status,
    string ChangeSummary,
    DateTimeOffset AppliedAt);

public sealed class ApplyRegisterAmendmentHandler(
    RegisterAmendmentCaseGuard caseGuard,
    IRegisterAmendmentCaseRepository caseRepository,
    IPersonRepository personRepository,
    RegisterAmendmentCaseAuthorization authorization,
    ICurrentOfficer currentOfficer,
    TimeProvider timeProvider)
{
    public async Task<ApplyRegisterAmendmentResponse> Handle(
        RegisterAmendmentCaseId caseId,
        CancellationToken cancellationToken)
    {
        authorization.EnsureCanApprove(currentOfficer);

        var amendmentCase = await caseGuard.GetForEditAsync(
            caseId,
            nameof(ApplyRegisterAmendment),
            cancellationToken);

        var person = await personRepository.GetForUpdateAsync(amendmentCase.PersonId, cancellationToken)
                     ?? throw new KeyNotFoundException($"Person '{amendmentCase.PersonId}' was not found.");

        var appliedAt = timeProvider.GetUtcNow();
        var details = amendmentCase.Apply(
            person,
            OfficerId.From(currentOfficer.OfficerId),
            appliedAt);

        await caseRepository.SaveChangesAsync(cancellationToken);

        return new ApplyRegisterAmendmentResponse(
            amendmentCase.Id.Value,
            person.Id.Value,
            amendmentCase.Status.ToString(),
            details.ChangeSummary,
            appliedAt);
    }
}
