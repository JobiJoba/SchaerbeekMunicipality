using FluentValidation;
using SchaerbeekMunicipality.Domain.Identity;
using SchaerbeekMunicipality.Domain.Registration;

namespace SchaerbeekMunicipality.Web.Features.Registration.RecordIdentity;

public sealed class RecordIdentityHandler(
    IRegistrationCaseRepository caseRepository,
    IPersonRepository personRepository,
    IValidator<RecordIdentityRequest> validator)
{
    public async Task<RecordIdentityResponse> Handle(
        RegistrationCaseId caseId,
        RecordIdentityRequest request,
        CancellationToken cancellationToken)
    {
        await validator.ValidateAndThrowAsync(request, cancellationToken);

        var registrationCase = await caseRepository.GetByIdAsync(caseId, cancellationToken)
            ?? throw new KeyNotFoundException($"Registration case '{caseId}' was not found.");

        var identity = new IdentityDetails(
            request.GivenName,
            request.FamilyName,
            request.BirthDate,
            request.Nationality);

        var person = registrationCase.RecordIdentity(identity);
        registrationCase.RefreshRegisterDeterminability(request.Nationality);

        await personRepository.AddAsync(person, cancellationToken);
        await caseRepository.SaveChangesAsync(cancellationToken);

        return new RecordIdentityResponse(
            registrationCase.Id.Value,
            person.Id.Value,
            registrationCase.Checklist.IdentityEstablished);
    }
}
