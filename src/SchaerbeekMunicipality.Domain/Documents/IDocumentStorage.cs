namespace SchaerbeekMunicipality.Domain.Documents;

public interface IDocumentStorage
{
    Task<StoredDocument> SaveAsync(
        Stream content,
        string fileName,
        CancellationToken cancellationToken);

    Task DeleteAsync(string storagePath, CancellationToken cancellationToken);
}

public sealed record StoredDocument(string StoragePath, string ContentHash);
