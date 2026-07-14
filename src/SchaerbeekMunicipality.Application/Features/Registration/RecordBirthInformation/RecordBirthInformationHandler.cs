using FluentValidation;
using SchaerbeekMunicipality.Domain.Identity;
using SchaerbeekMunicipality.Domain.Registration;

namespace SchaerbeekMunicipality.Application.Features.Registration.RecordBirthInformation;

public sealed class RecordBirthInformationHandler(
    RegistrationCaseGuard caseGuard,
    IRegistrationCaseRepository caseRepository,
    IPersonRepository personRepository,
    RegistrationExceptionEvaluator exceptionEvaluator,
    IValidator<RecordBirthInformationRequest> validator)
{
    public async Task<RecordBirthInformationResponse> Handle(
        RegistrationCaseId caseId,
        RecordBirthInformationRequest request,
        CancellationToken cancellationToken)
    {
        await validator.ValidateAndThrowAsync(request, cancellationToken);

        var registrationCase = await caseGuard.GetForEditAsync(
            caseId,
            nameof(RecordBirthInformation),
            cancellationToken);

        var person = await personRepository.GetForUpdateAsync(registrationCase.PersonId!.Value, cancellationToken)
                     ?? throw new KeyNotFoundException($"Person '{registrationCase.PersonId}' was not found.");

        registrationCase.RecordBirthInformation(
            person,
            new BirthInformationDetails(request.BirthPlace, request.BirthCountry));

        await exceptionEvaluator.EvaluateAndApplyAsync(registrationCase, cancellationToken);
        await caseRepository.SaveChangesAsync(cancellationToken);

        return new RecordBirthInformationResponse(
            caseId.Value,
            person.BirthPlace!,
            person.BirthCountry,
            registrationCase.Checklist.BirthEvidenceEstablished);
    }
}