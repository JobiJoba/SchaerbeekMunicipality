using FluentValidation;
using SchaerbeekMunicipality.Domain.Identity;
using SchaerbeekMunicipality.Domain.Registration;

namespace SchaerbeekMunicipality.Web.Features.Registration.RecordIdentity;

public sealed class RecordIdentityHandler(
    RegistrationCaseGuard caseGuard,
    IRegistrationCaseRepository caseRepository,
    IPersonRepository personRepository,
    RegistrationExceptionEvaluator exceptionEvaluator,
    IValidator<RecordIdentityRequest> validator)
{
    public async Task<RecordIdentityResponse> Handle(
        RegistrationCaseId caseId,
        RecordIdentityRequest request,
        CancellationToken cancellationToken)
    {
        await validator.ValidateAndThrowAsync(request, cancellationToken);

        var registrationCase = await caseGuard.GetForEditAsync(
            caseId,
            nameof(RecordIdentity),
            cancellationToken);

        var identity = new IdentityDetails(
            request.GivenName,
            request.FamilyName,
            request.BirthDate,
            request.Nationality);

        var person = registrationCase.RecordIdentity(identity);
        registrationCase.RefreshRegisterDeterminability(request.Nationality);

        await personRepository.AddAsync(person, cancellationToken);
        await exceptionEvaluator.EvaluateAndApplyAsync(registrationCase, cancellationToken, person);
        await caseRepository.SaveChangesAsync(cancellationToken);

        return new RecordIdentityResponse(
            registrationCase.Id.Value,
            person.Id.Value,
            registrationCase.Checklist.IdentityEstablished);
    }
}
