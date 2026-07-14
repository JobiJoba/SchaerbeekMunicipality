namespace SchaerbeekMunicipality.Domain.IdentityDocuments;

public interface IDocumentRequestCaseRepository
{
    Task<IReadOnlyList<DocumentRequestCase>> ListAsync(CancellationToken cancellationToken);

    Task<DocumentRequestCase?> GetByIdAsync(DocumentRequestCaseId id, CancellationToken cancellationToken);

    Task<int> CountAsync(CancellationToken cancellationToken);

    Task AddAsync(DocumentRequestCase documentRequestCase, CancellationToken cancellationToken);

    Task SaveChangesAsync(CancellationToken cancellationToken);
}