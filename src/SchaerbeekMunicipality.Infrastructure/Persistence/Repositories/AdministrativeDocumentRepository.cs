using Microsoft.EntityFrameworkCore;
using SchaerbeekMunicipality.Domain.Documents;
using SchaerbeekMunicipality.Domain.Registration;

namespace SchaerbeekMunicipality.Infrastructure.Persistence.Repositories;

internal sealed class AdministrativeDocumentRepository(MunicipalDbContext dbContext)
    : IAdministrativeDocumentRepository
{
    public async Task<IReadOnlyList<AdministrativeDocument>> ListByCaseIdAsync(
        RegistrationCaseId registrationCaseId,
        CancellationToken cancellationToken)
    {
        return await dbContext.AdministrativeDocuments
            .AsNoTracking()
            .Where(d => d.RegistrationCaseId == registrationCaseId)
            .OrderByDescending(d => d.UploadedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(AdministrativeDocument document, CancellationToken cancellationToken)
    {
        await dbContext.AdministrativeDocuments.AddAsync(document, cancellationToken);
    }
}
