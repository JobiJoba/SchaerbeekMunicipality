using SchaerbeekMunicipality.Domain.Registration;

namespace SchaerbeekMunicipality.Web.Features.Registration.ListRegistrationCases;

public sealed record RegistrationCaseSummary(Guid Id, string Status);

public sealed class ListRegistrationCasesHandler(IRegistrationCaseRepository repository)
{
    public async Task<IReadOnlyList<RegistrationCaseSummary>> Handle(CancellationToken cancellationToken)
    {
        var cases = await repository.ListAsync(cancellationToken);

        return cases
            .Select(c => new RegistrationCaseSummary(c.Id.Value, c.Status.ToString()))
            .ToList();
    }
}
