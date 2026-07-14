using Microsoft.EntityFrameworkCore;
using SchaerbeekMunicipality.Domain.IdentityDocuments;

namespace SchaerbeekMunicipality.Infrastructure.Persistence.Repositories;

internal sealed class DocumentRequestCaseRepository(MunicipalDbContext dbContext) : IDocumentRequestCaseRepository
{
    public async Task<IReadOnlyList<DocumentRequestCase>> ListAsync(CancellationToken cancellationToken)
    {
        var results = await dbContext.DocumentRequestCases
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return results
            .OrderByDescending(c => c.RequestedAt)
            .ToList();
    }

    public Task<DocumentRequestCase?> GetByIdAsync(DocumentRequestCaseId id, CancellationToken cancellationToken)
    {
        return dbContext.DocumentRequestCases
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public Task<int> CountAsync(CancellationToken cancellationToken)
    {
        return dbContext.DocumentRequestCases.CountAsync(cancellationToken);
    }

    public async Task AddAsync(DocumentRequestCase documentRequestCase, CancellationToken cancellationToken)
    {
        await dbContext.DocumentRequestCases.AddAsync(documentRequestCase, cancellationToken);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        return dbContext.SaveChangesAsync(cancellationToken);
    }
}