using Microsoft.EntityFrameworkCore;
using SchaerbeekMunicipality.Domain.Address;
using SchaerbeekMunicipality.Domain.Identity;
using SchaerbeekMunicipality.Domain.NationalRegister;

namespace SchaerbeekMunicipality.Infrastructure.Persistence;

/// <summary>
/// Seeds population-register persons for local demo flows (change of address, identity documents).
/// NR records come from <see cref="NationalRegisterSeeder"/>; this links them into the local register.
/// </summary>
public static class PopulationRegisterSeeder
{
    public static async Task SeedAsync(MunicipalDbContext dbContext, CancellationToken cancellationToken)
    {
        await SeedPersonIfMissingAsync(
            dbContext,
            NationalRegisterSeeder.JeanDupontId,
            BelgianAddress.Create("Rue de la Paix", "1", null, "1030", "Schaerbeek"),
            cancellationToken);

        await SeedPersonIfMissingAsync(
            dbContext,
            NationalRegisterSeeder.SofiaNguyenId,
            BelgianAddress.Create("Avenue Rogier", "42", null, "1030", "Schaerbeek"),
            cancellationToken);
    }

    private static async Task SeedPersonIfMissingAsync(
        MunicipalDbContext dbContext,
        NationalRegisterPersonId registerPersonId,
        BelgianAddress domicile,
        CancellationToken cancellationToken)
    {
        var registerPerson = await dbContext.NationalRegisterPersons
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == registerPersonId, cancellationToken);

        if (registerPerson is null || registerPerson.NationalRegisterNumber is null)
        {
            return;
        }

        var alreadyLinked = await dbContext.Persons
            .AsNoTracking()
            .AnyAsync(
                p => p.LinkedRegisterRecordId == registerPersonId ||
                     (registerPerson.NationalRegisterNumber != null &&
                      p.NationalRegisterNumber == registerPerson.NationalRegisterNumber),
                cancellationToken);

        if (alreadyLinked)
        {
            return;
        }

        var person = Person.CreateFromRegisterRecord(registerPerson);
        if (person.NationalRegisterNumber is null && registerPerson.NationalRegisterNumber is { } nr)
        {
            person.AssignNationalRegisterNumber(nr);
        }

        person.UpdateDomicile(domicile);
        await dbContext.Persons.AddAsync(person, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
