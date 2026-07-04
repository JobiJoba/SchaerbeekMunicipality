using FluentValidation;
using SchaerbeekMunicipality.Domain.Identity;
using SchaerbeekMunicipality.Domain.NationalRegister;
using SchaerbeekMunicipality.Domain.Registration;

namespace SchaerbeekMunicipality.Web.Features.Registration.ConvertBisNumber;

public sealed class ConvertBisNumberHandler(
    IRegistrationCaseRepository caseRepository,
    IPersonRepository personRepository,
    INationalRegisterRepository nationalRegisterRepository)
{
    public async Task<ConvertBisNumberResponse> Handle(
        RegistrationCaseId caseId,
        CancellationToken cancellationToken)
    {
        var registrationCase = await caseRepository.GetByIdAsync(caseId, cancellationToken)
            ?? throw new KeyNotFoundException($"Registration case '{caseId}' was not found.");

        registrationCase.EnsureIntakeDataEditable(nameof(ConvertBisNumber));

        if (registrationCase.PersonId is not { } personId)
        {
            throw new InvalidRegistrationTransitionException(
                "Identity must be recorded before converting a BIS number.");
        }

        var person = await personRepository.GetForUpdateAsync(personId, cancellationToken)
            ?? throw new KeyNotFoundException($"Person '{personId}' was not found.");

        if (person.BisNumber is not { } bisNumber)
        {
            throw new InvalidRegistrationTransitionException(
                "This person does not have a BIS number to convert.");
        }

        if (person.NationalRegisterNumber is not null)
        {
            throw new InvalidRegistrationTransitionException(
                "This person already has a National Register number.");
        }

        var nationalRegisterNumber = NationalRegisterNumber.GenerateStub(person.BirthDate, 42);

        if (await personRepository.IsNationalRegisterNumberAssignedAsync(
                nationalRegisterNumber,
                cancellationToken))
        {
            throw new NationalRegisterConflictException(
                "Generated National Register number is already assigned. Retry the conversion.");
        }

        person.ConvertBisToNationalRegister(nationalRegisterNumber);

        if (person.LinkedRegisterRecordId is { } registerRecordId)
        {
            var registerPerson = await nationalRegisterRepository.GetByIdAsync(
                registerRecordId,
                cancellationToken);

            registerPerson?.AssignNationalRegisterNumber(nationalRegisterNumber);
            await nationalRegisterRepository.SaveChangesAsync(cancellationToken);
        }

        await caseRepository.SaveChangesAsync(cancellationToken);

        return new ConvertBisNumberResponse(
            registrationCase.Id.Value,
            person.Id.Value,
            bisNumber.Value,
            nationalRegisterNumber.Value);
    }
}
