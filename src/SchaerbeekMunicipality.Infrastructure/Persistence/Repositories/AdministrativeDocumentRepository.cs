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
        var documents = await dbContext.AdministrativeDocuments
            .AsNoTracking()
            .Where(d => d.RegistrationCaseId == registrationCaseId)
            .ToListAsync(cancellationToken);

        return documents
            .OrderByDescending(d => d.UploadedAt)
            .ToList();
    }

    public async Task AddAsync(AdministrativeDocument document, CancellationToken cancellationToken)
    {
        await dbContext.AdministrativeDocuments.AddAsync(document, cancellationToken);
    }

    public Task<AdministrativeDocument?> GetByIdAsync(
        AdministrativeDocumentId id,
        CancellationToken cancellationToken)
    {
        return dbContext.AdministrativeDocuments
            .FirstOrDefaultAsync(d => d.Id == id, cancellationToken);
    }

    public void Remove(AdministrativeDocument document)
    {
        dbContext.AdministrativeDocuments.Remove(document);
    }
}
