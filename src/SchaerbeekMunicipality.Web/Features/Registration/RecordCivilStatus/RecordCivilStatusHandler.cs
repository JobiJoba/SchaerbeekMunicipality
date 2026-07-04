using FluentValidation;
using SchaerbeekMunicipality.Domain.Identity;
using SchaerbeekMunicipality.Domain.Registration;

namespace SchaerbeekMunicipality.Web.Features.Registration.RecordCivilStatus;

public sealed class RecordCivilStatusHandler(
    IRegistrationCaseRepository caseRepository,
    IPersonRepository personRepository,
    IValidator<RecordCivilStatusRequest> validator)
{
    public async Task<RecordCivilStatusResponse> Handle(
        RegistrationCaseId caseId,
        RecordCivilStatusRequest request,
        CancellationToken cancellationToken)
    {
        await validator.ValidateAndThrowAsync(request, cancellationToken);

        var registrationCase = await caseRepository.GetByIdAsync(caseId, cancellationToken)
            ?? throw new KeyNotFoundException($"Registration case '{caseId}' was not found.");

        registrationCase.EnsureIntakeDataEditable(nameof(RecordCivilStatus));

        if (!registrationCase.Checklist.IdentityEstablished || registrationCase.PersonId is null)
        {
            throw new InvalidRegistrationTransitionException(
                "Identity must be recorded before civil status can be captured.");
        }

        var person = await personRepository.GetForUpdateAsync(registrationCase.PersonId.Value, cancellationToken)
            ?? throw new KeyNotFoundException($"Person '{registrationCase.PersonId}' was not found.");

        person.RecordCivilStatus(new CivilStatusDetails(
            request.Status,
            request.SpouseGivenName,
            request.SpouseFamilyName,
            request.MarriageDate,
            request.MarriagePlace));

        await caseRepository.SaveChangesAsync(cancellationToken);

        var civilStatus = person.CivilStatus!;

        return new RecordCivilStatusResponse(
            caseId.Value,
            civilStatus.Status,
            civilStatus.SpouseGivenName,
            civilStatus.SpouseFamilyName,
            civilStatus.MarriageDate,
            civilStatus.MarriagePlace);
    }
}
