using FluentValidation;
using SchaerbeekMunicipality.Domain.ChangeOfAddress;
using SchaerbeekMunicipality.Domain.Identity;
using SchaerbeekMunicipality.Domain.Registration;
using SchaerbeekMunicipality.Application.Auth;

namespace SchaerbeekMunicipality.Application.Features.ChangeOfAddress.OpenChangeOfAddressCase;

public sealed class OpenChangeOfAddressCaseHandler(
    IChangeOfAddressCaseRepository caseRepository,
    IPersonRepository personRepository,
    IRegistrationCaseRepository registrationCaseRepository,
    ChangeOfAddressCaseAuthorization authorization,
    ICurrentOfficer currentOfficer,
    IValidator<OpenChangeOfAddressCaseRequest> validator,
    TimeProvider timeProvider)
{
    public async Task<OpenChangeOfAddressCaseResponse> Handle(
        OpenChangeOfAddressCaseRequest request,
        CancellationToken cancellationToken)
    {
        authorization.EnsureCanCreate(currentOfficer);
        await validator.ValidateAndThrowAsync(request, cancellationToken);

        var personId = new PersonId(request.PersonId);
        var person = await personRepository.GetByIdAsync(personId, cancellationToken)
            ?? throw new KeyNotFoundException($"Person '{personId}' was not found.");

        if (person.NationalRegisterNumber is null)
        {
            throw new InvalidChangeOfAddressTransitionException(
                "Cannot open a change of address case for a person without a National Register number.");
        }

        var previousAddress = person.DomicileAddress;
        if (previousAddress is null)
        {
            var registrationCase = await registrationCaseRepository.GetLatestRegisteredByPersonIdAsync(
                personId,
                cancellationToken);
            previousAddress = registrationCase?.DeclaredAddress;
        }

        var changeOfAddressCase = ChangeOfAddressCase.Open(
            personId,
            previousAddress,
            timeProvider.GetUtcNow());

        await caseRepository.AddAsync(changeOfAddressCase, cancellationToken);
        await caseRepository.SaveChangesAsync(cancellationToken);

        return new OpenChangeOfAddressCaseResponse(
            changeOfAddressCase.Id.Value,
            person.Id.Value,
            changeOfAddressCase.Status.ToString(),
            changeOfAddressCase.OpenedAt);
    }
}
