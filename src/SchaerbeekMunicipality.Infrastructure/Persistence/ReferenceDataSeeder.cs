using Microsoft.EntityFrameworkCore;
using SchaerbeekMunicipality.Domain.ReferenceData;

namespace SchaerbeekMunicipality.Infrastructure.Persistence;

internal static class ReferenceDataSeeder
{
    public static async Task SeedAsync(MunicipalDbContext dbContext, CancellationToken cancellationToken)
    {
        if (await dbContext.Municipalities.AnyAsync(cancellationToken)) return;

        var municipalities = new[]
        {
            MunicipalityReference.Create("1030", "Schaerbeek"),
            // Neighbouring communes kept for future flows (e.g. change of address); not offered during first registration intake.
            MunicipalityReference.Create("1000", "Bruxelles"),
            MunicipalityReference.Create("1040", "Etterbeek"),
            MunicipalityReference.Create("1210", "Saint-Josse-ten-Noode")
        };

        await dbContext.Municipalities.AddRangeAsync(municipalities, cancellationToken);

        var streets = new[]
        {
            StreetReference.Create(1, "1030", "Avenue Rogier"),
            StreetReference.Create(2, "1030", "Chaussée de Louvain"),
            StreetReference.Create(3, "1030", "Rue de Brabant"),
            StreetReference.Create(4, "1030", "Rue Josaphat"),
            StreetReference.Create(5, "1030", "Rue Vanderkindere"),
            StreetReference.Create(6, "1030", "Boulevard Lambermont"),
            StreetReference.Create(7, "1030", "Rue de la Chasse"),
            StreetReference.Create(8, "1030", "Avenue Huart Hamoir"),
            StreetReference.Create(9, "1030", "Rue Gallait"),
            StreetReference.Create(10, "1030", "Rue de Jérusalem")
        };

        await dbContext.Streets.AddRangeAsync(streets, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}