using Microsoft.EntityFrameworkCore;
using SchaerbeekMunicipality.Domain.Identity;
using SchaerbeekMunicipality.Domain.NationalRegister;

namespace SchaerbeekMunicipality.Infrastructure.Persistence.Repositories;

internal sealed class PersonRepository(MunicipalDbContext dbContext) : IPersonRepository
{
    public Task<Person?> GetByIdAsync(PersonId id, CancellationToken cancellationToken)
    {
        return dbContext.Persons
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public Task<Person?> GetByRegisterRecordIdAsync(
        NationalRegisterPersonId registerPersonId,
        CancellationToken cancellationToken)
    {
        return dbContext.Persons
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.LinkedRegisterRecordId == registerPersonId, cancellationToken);
    }

    public Task<Person?> GetByNationalRegisterNumberAsync(
        NationalRegisterNumber nationalRegisterNumber,
        CancellationToken cancellationToken)
    {
        return dbContext.Persons
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.NationalRegisterNumber == nationalRegisterNumber, cancellationToken);
    }

    public Task<Person?> GetForUpdateAsync(PersonId id, CancellationToken cancellationToken)
    {
        return dbContext.Persons
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task AddAsync(Person person, CancellationToken cancellationToken)
    {
        await dbContext.Persons.AddAsync(person, cancellationToken);
    }

    public Task<bool> IsRegisterRecordLinkedAsync(
        NationalRegisterPersonId registerPersonId,
        CancellationToken cancellationToken)
    {
        return dbContext.Persons
            .AsNoTracking()
            .AnyAsync(p => p.LinkedRegisterRecordId == registerPersonId, cancellationToken);
    }

    public Task<bool> IsNationalRegisterNumberAssignedAsync(
        NationalRegisterNumber nationalRegisterNumber,
        CancellationToken cancellationToken)
    {
        return dbContext.Persons
            .AsNoTracking()
            .AnyAsync(p => p.NationalRegisterNumber == nationalRegisterNumber, cancellationToken);
    }
}
