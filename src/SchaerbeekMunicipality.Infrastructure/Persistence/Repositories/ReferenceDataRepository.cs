using Microsoft.EntityFrameworkCore;
using SchaerbeekMunicipality.Domain.ReferenceData;

namespace SchaerbeekMunicipality.Infrastructure.Persistence.Repositories;

internal sealed class ReferenceDataRepository(MunicipalDbContext dbContext) : IReferenceDataRepository
{
    public async Task<IReadOnlyList<MunicipalityReference>> ListMunicipalitiesAsync(
        string? search,
        CancellationToken cancellationToken)
    {
        var query = dbContext.Municipalities.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            query = query.Where(m =>
                m.Name.Contains(term) || m.PostalCode.Contains(term));
        }

        return await query
            .OrderBy(m => m.PostalCode)
            .Take(20)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<StreetReference>> SearchStreetsAsync(
        string postalCode,
        string? search,
        CancellationToken cancellationToken)
    {
        var query = dbContext.Streets
            .AsNoTracking()
            .Where(s => s.PostalCode == postalCode);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            query = query.Where(s => s.Name.Contains(term));
        }

        return await query
            .OrderBy(s => s.Name)
            .Take(20)
            .ToListAsync(cancellationToken);
    }
}
