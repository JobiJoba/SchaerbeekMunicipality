using SchaerbeekMunicipality.Domain.Registration;

namespace SchaerbeekMunicipality.Web.Features.Registration.ListRegistrationCases;

public sealed record RegistrationCaseSummary(
    Guid Id,
    RegistrationCaseStatus Status,
    VisitReason VisitReason,
    DateTimeOffset OpenedAt,
    bool IdentityEstablished);

public sealed class ListRegistrationCasesHandler(IRegistrationCaseRepository repository)
{
    public async Task<IReadOnlyList<RegistrationCaseSummary>> Handle(CancellationToken cancellationToken)
    {
        var cases = await repository.ListAsync(cancellationToken);

        return cases
            .Select(c => new RegistrationCaseSummary(
                c.Id.Value,
                c.Status,
                c.VisitReason,
                c.OpenedAt,
                c.Checklist.IdentityEstablished))
            .ToList();
    }
}
