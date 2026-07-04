using Microsoft.EntityFrameworkCore;
using SchaerbeekMunicipality.Domain.NationalRegister;

namespace SchaerbeekMunicipality.Infrastructure.Persistence;

public static class NationalRegisterSeeder
{
    public static readonly NationalRegisterPersonId MarieLeclercId =
        NationalRegisterPersonId.From(Guid.Parse("aaaaaaaa-0001-4000-8000-000000000001"));

    public static readonly NationalRegisterPersonId AmelieBernardId =
        NationalRegisterPersonId.From(Guid.Parse("aaaaaaaa-0001-4000-8000-000000000002"));

    public static readonly NationalRegisterPersonId JeanDupontId =
        NationalRegisterPersonId.From(Guid.Parse("aaaaaaaa-0001-4000-8000-000000000003"));

    public static readonly NationalRegisterPersonId JacquesDupontId =
        NationalRegisterPersonId.From(Guid.Parse("aaaaaaaa-0001-4000-8000-000000000004"));

    public static readonly NationalRegisterPersonId SofiaNguyenId =
        NationalRegisterPersonId.From(Guid.Parse("aaaaaaaa-0001-4000-8000-000000000005"));

    public static async Task SeedAsync(MunicipalDbContext dbContext, CancellationToken cancellationToken)
    {
        if (await dbContext.NationalRegisterPersons.AnyAsync(cancellationToken))
        {
            return;
        }

        var records = new[]
        {
            NationalRegisterPerson.Create(
                MarieLeclercId,
                "Marie",
                "Leclerc",
                new DateOnly(1975, 1, 1),
                "Belgian",
                BisNumber.Create("75010112345"),
                null),
            NationalRegisterPerson.Create(
                AmelieBernardId,
                "Amélie",
                "Bernard",
                new DateOnly(1992, 3, 20),
                "French",
                BisNumber.Create("72032054321"),
                null),
            NationalRegisterPerson.Create(
                JeanDupontId,
                "Jean",
                "Dupont",
                new DateOnly(1985, 6, 12),
                "Belgian",
                null,
                NationalRegisterNumber.GenerateStub(new DateOnly(1985, 6, 12), 1)),
            NationalRegisterPerson.Create(
                JacquesDupontId,
                "J.",
                "Dupont",
                new DateOnly(1985, 6, 12),
                "Belgian",
                BisNumber.Create("75061298765"),
                null),
            NationalRegisterPerson.Create(
                SofiaNguyenId,
                "Sofia",
                "Nguyen",
                new DateOnly(2000, 11, 8),
                "Vietnamese",
                null,
                NationalRegisterNumber.GenerateStub(new DateOnly(2000, 11, 8), 1)),
        };

        await dbContext.NationalRegisterPersons.AddRangeAsync(records, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
