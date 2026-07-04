using Microsoft.EntityFrameworkCore;
using SchaerbeekMunicipality.Domain.Identity;

namespace SchaerbeekMunicipality.Infrastructure.Persistence.Repositories;

internal sealed class PersonRepository(MunicipalDbContext dbContext) : IPersonRepository
{
    public Task<Person?> GetByIdAsync(PersonId id, CancellationToken cancellationToken)
    {
        return dbContext.Persons
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task AddAsync(Person person, CancellationToken cancellationToken)
    {
        await dbContext.Persons.AddAsync(person, cancellationToken);
    }
}
