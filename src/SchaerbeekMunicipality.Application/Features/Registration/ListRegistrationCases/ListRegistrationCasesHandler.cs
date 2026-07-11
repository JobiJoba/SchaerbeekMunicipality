using SchaerbeekMunicipality.Domain.Registration;
using SchaerbeekMunicipality.Application.Auth;

namespace SchaerbeekMunicipality.Application.Features.Registration.ListRegistrationCases;

public sealed record RegistrationCaseSummary(
    Guid Id,
    RegistrationCaseStatus Status,
    VisitReason VisitReason,
    DateTimeOffset OpenedAt,
    Guid? AssignedOfficerId,
    Guid? LockedByOfficerId,
    bool IdentityEstablished,
    bool LegalResidenceEstablished,
    bool AddressDeclared);

public sealed class ListRegistrationCasesHandler(
    IRegistrationCaseRepository repository,
    RegistrationCaseAuthorization authorization,
    ICurrentOfficer currentOfficer)
{
    public async Task<IReadOnlyList<RegistrationCaseSummary>> Handle(CancellationToken cancellationToken)
    {
        authorization.EnsureCanList(currentOfficer);

        var cases = await repository.ListAsync(cancellationToken);

        return cases
            .Select(c => new RegistrationCaseSummary(
                c.Id.Value,
                c.Status,
                c.VisitReason,
                c.OpenedAt,
                c.AssignedOfficerId?.Value,
                c.LockedByOfficerId?.Value,
                c.Checklist.IdentityEstablished,
                c.Checklist.LegalResidenceEstablished,
                c.Checklist.AddressDeclared))
            .ToList();
    }
}
