using SchaerbeekMunicipality.Domain.NationalRegister;

namespace SchaerbeekMunicipality.Domain.Identity;

public interface IPersonRepository
{
    Task<Person?> GetByIdAsync(PersonId id, CancellationToken cancellationToken);

    Task<Person?> GetByRegisterRecordIdAsync(
        NationalRegisterPersonId registerPersonId,
        CancellationToken cancellationToken);

    Task<Person?> GetByNationalRegisterNumberAsync(
        NationalRegisterNumber nationalRegisterNumber,
        CancellationToken cancellationToken);

    Task<Person?> GetForUpdateAsync(PersonId id, CancellationToken cancellationToken);

    Task AddAsync(Person person, CancellationToken cancellationToken);

    Task<bool> IsRegisterRecordLinkedAsync(
        NationalRegisterPersonId registerPersonId,
        CancellationToken cancellationToken);

    Task<bool> IsNationalRegisterNumberAssignedAsync(
        NationalRegisterNumber nationalRegisterNumber,
        CancellationToken cancellationToken);
}
