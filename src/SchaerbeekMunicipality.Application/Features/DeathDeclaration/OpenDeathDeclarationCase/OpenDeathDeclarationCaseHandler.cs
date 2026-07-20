using FluentValidation;
using SchaerbeekMunicipality.Application.Auth;
using SchaerbeekMunicipality.Domain.DeathDeclaration;
using SchaerbeekMunicipality.Domain.Identity;

namespace SchaerbeekMunicipality.Application.Features.DeathDeclaration.OpenDeathDeclarationCase;

public sealed class OpenDeathDeclarationCaseHandler(
    IDeathDeclarationCaseRepository caseRepository,
    IPersonRepository personRepository,
    DeathDeclarationCaseAuthorization authorization,
    ICurrentOfficer currentOfficer,
    IValidator<OpenDeathDeclarationCaseRequest> validator,
    TimeProvider timeProvider)
{
    public async Task<OpenDeathDeclarationCaseResponse> Handle(
        OpenDeathDeclarationCaseRequest request,
        CancellationToken cancellationToken)
    {
        authorization.EnsureCanCreate(currentOfficer);
        await validator.ValidateAndThrowAsync(request, cancellationToken);

        var personId = new PersonId(request.PersonId);
        var person = await personRepository.GetByIdAsync(personId, cancellationToken)
                     ?? throw new KeyNotFoundException($"Person '{personId}' was not found.");

        if (person.NationalRegisterNumber is null)
            throw new InvalidDeathDeclarationTransitionException(
                "Cannot open a death declaration case for a person without a National Register number.");

        if (person.IsDeceased)
            throw new InvalidDeathDeclarationTransitionException(
                "This person is already recorded as deceased.");

        var existing = await caseRepository.GetActiveByPersonIdAsync(personId, cancellationToken);
        if (existing is not null)
            throw new InvalidDeathDeclarationTransitionException(
                "An open death declaration case already exists for this person.");

        var deathDeclarationCase = DeathDeclarationCase.Open(personId, timeProvider.GetUtcNow());

        await caseRepository.AddAsync(deathDeclarationCase, cancellationToken);
        await caseRepository.SaveChangesAsync(cancellationToken);

        return new OpenDeathDeclarationCaseResponse(
            deathDeclarationCase.Id.Value,
            person.Id.Value,
            deathDeclarationCase.Status.ToString(),
            deathDeclarationCase.OpenedAt);
    }
}
