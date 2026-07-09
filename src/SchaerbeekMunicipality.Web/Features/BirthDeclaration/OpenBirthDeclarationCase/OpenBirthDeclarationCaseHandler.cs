using SchaerbeekMunicipality.Domain.BirthDeclaration;
using SchaerbeekMunicipality.Web.Auth;

namespace SchaerbeekMunicipality.Web.Features.BirthDeclaration.OpenBirthDeclarationCase;

public sealed class OpenBirthDeclarationCaseHandler(
    IBirthDeclarationCaseRepository repository,
    BirthDeclarationCaseAuthorization authorization,
    ICurrentOfficer currentOfficer,
    TimeProvider timeProvider)
{
    public async Task<OpenBirthDeclarationCaseResponse> Handle(CancellationToken cancellationToken)
    {
        authorization.EnsureCanCreate(currentOfficer);

        var birthDeclarationCase = BirthDeclarationCase.Open(timeProvider.GetUtcNow());
        await repository.AddAsync(birthDeclarationCase, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);

        return new OpenBirthDeclarationCaseResponse(
            birthDeclarationCase.Id.Value,
            birthDeclarationCase.Status.ToString(),
            birthDeclarationCase.OpenedAt);
    }
}
