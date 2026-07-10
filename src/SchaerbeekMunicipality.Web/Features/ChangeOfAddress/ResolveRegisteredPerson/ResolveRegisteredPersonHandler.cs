using SchaerbeekMunicipality.Domain.ChangeOfAddress;
using SchaerbeekMunicipality.Domain.Identity;
using SchaerbeekMunicipality.Domain.NationalRegister;

namespace SchaerbeekMunicipality.Web.Features.ChangeOfAddress.ResolveRegisteredPerson;

public sealed record ResolveRegisteredPersonRequest(Guid RegisterPersonId);

public sealed record ResolveRegisteredPersonResponse(
    Guid PersonId,
    string GivenName,
    string FamilyName,
    string NationalRegisterNumber);

public sealed class ResolveRegisteredPersonHandler(
    INationalRegisterRepository nationalRegisterRepository,
    IPersonRepository personRepository)
{
    public async Task<ResolveRegisteredPersonResponse> Handle(
        ResolveRegisteredPersonRequest request,
        CancellationToken cancellationToken)
    {
        var registerPersonId = NationalRegisterPersonId.From(request.RegisterPersonId);
        var registerPerson = await nationalRegisterRepository.GetByIdAsync(registerPersonId, cancellationToken)
            ?? throw new KeyNotFoundException($"National Register record '{registerPersonId}' was not found.");

        if (registerPerson.NationalRegisterNumber is null)
        {
            throw new InvalidChangeOfAddressTransitionException(
                "The selected person does not have a National Register number.");
        }

        var person = await personRepository.GetByRegisterRecordIdAsync(registerPersonId, cancellationToken);
        if (person is null && registerPerson.NationalRegisterNumber is { } nrNumber)
        {
            person = await personRepository.GetByNationalRegisterNumberAsync(nrNumber, cancellationToken);
        }

        if (person is null)
        {
            throw new InvalidChangeOfAddressTransitionException(
                "This person is not registered in the population register. Complete first registration before opening a change of address.");
        }

        if (person.NationalRegisterNumber is null)
        {
            throw new InvalidChangeOfAddressTransitionException(
                "Cannot open a change of address case for a person without a National Register number.");
        }

        return new ResolveRegisteredPersonResponse(
            person.Id.Value,
            person.GivenName,
            person.FamilyName,
            person.NationalRegisterNumber.Value.Value);
    }
}
