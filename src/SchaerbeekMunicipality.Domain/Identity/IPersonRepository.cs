namespace SchaerbeekMunicipality.Domain.Identity;

public interface IPersonRepository
{
    Task<Person?> GetByIdAsync(PersonId id, CancellationToken cancellationToken);

    Task<Person?> GetForUpdateAsync(PersonId id, CancellationToken cancellationToken);

    Task AddAsync(Person person, CancellationToken cancellationToken);
}
